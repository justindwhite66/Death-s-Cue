using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // Singleton pattern
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] musicTracks; // Array of music tracks for different scenes
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private float crossFadeDuration = 1.0f;
    
    private AudioSource crossFadeSource; // Secondary source for crossfading
    private int currentSceneIndex = -1;
    private bool isCrossFading = false;
    
    private void Awake()
    {
        // Implement singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize secondary audio source for crossfading
            crossFadeSource = gameObject.AddComponent<AudioSource>();
            crossFadeSource.loop = true;
            crossFadeSource.volume = 0;
            crossFadeSource.playOnAwake = false;
            
            // Register for scene load events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        // Unregister from scene load events when destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int newSceneIndex = scene.buildIndex;
        
        // Check if we're in the title scene
        if (scene.name == "Title")
        {
            PlayMusic(menuMusic);
            return;
        }
        
        // If we have a track for this scene, play it
        if (newSceneIndex < musicTracks.Length && musicTracks[newSceneIndex] != null)
        {
            PlayMusic(musicTracks[newSceneIndex]);
        }
        
        currentSceneIndex = newSceneIndex;
    }
    
    public void PlayMusic(AudioClip newTrack)
    {
        // If we're already playing this track, do nothing
        if (musicSource.clip == newTrack && musicSource.isPlaying)
        {
            return;
        }
        
        // If we're already cross-fading, stop that coroutine
        if (isCrossFading)
        {
            StopAllCoroutines();
            isCrossFading = false;
        }
        
        // If we're not playing anything yet, just start playing
        if (musicSource.clip == null || !musicSource.isPlaying)
        {
            musicSource.clip = newTrack;
            musicSource.Play();
            return;
        }
        
        // Otherwise, crossfade to the new track
        StartCoroutine(CrossFadeMusic(newTrack));
    }
    
    private IEnumerator CrossFadeMusic(AudioClip newTrack)
    {
        isCrossFading = true;
        
        // Set up the crossfade source with the new track
        crossFadeSource.clip = newTrack;
        crossFadeSource.volume = 0;
        crossFadeSource.Play();
        
        float startVolume = musicSource.volume;
        
        // Fade out the current track and fade in the new track
        for (float t = 0; t < crossFadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / crossFadeDuration);
            crossFadeSource.volume = Mathf.Lerp(0, startVolume, t / crossFadeDuration);
            yield return null;
        }
        
        // Swap the audio sources
        AudioSource temp = musicSource;
        musicSource = crossFadeSource;
        crossFadeSource = temp;
        
        // Stop the old track
        crossFadeSource.Stop();
        crossFadeSource.clip = null;
        crossFadeSource.volume = 0;
        
        isCrossFading = false;
    }
    
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }
}