using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    // Serialized field allows you to set the scene name in the Inspector
    [SerializeField] private string sceneToLoad = "Indoor_1";
    
    public void PlayGame()
    {
        // Load the specified scene by name instead of index
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
