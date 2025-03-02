using System.Collections;
using UnityEngine;

public class TeleportField : MonoBehaviour
{
    [SerializeField] private float fieldRadius = 8f;
    [SerializeField] private float fadeDelay = 3f; // Time before fade starts
    [SerializeField] private LayerMask baseLayerMask;
    [SerializeField] private LayerMask restrictedLayersMask;
    private bool playerInside = true;
    private SpriteFade spriteFade;
    private Coroutine fadeCoroutine;
    private Camera mainCamera;

    private void Start()
    {
        spriteFade = GetComponent<SpriteFade>();
        StartCoroutine(AssignCameraAfterSceneLoad());
    }

    private IEnumerator AssignCameraAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f); // Wait briefly after scene loads
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Reassign camera if it's null
            if (mainCamera == null) return; // Prevents errors
        }

        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer > fieldRadius && playerInside)
        {
            // Player has exited the field, start fading
            playerInside = false;
            fadeCoroutine = StartCoroutine(FadeAndDestroy());
        }
        else if (distanceToPlayer <= fieldRadius && !playerInside)
        {
            // Player re-entered the field, cancel fade
            playerInside = true;
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
            spriteFade.StopFade(); // Restore visibility
        }
    }

    public bool IsValidTeleportLocation(Vector3 position){
        Collider2D baseTile = Physics2D.OverlapPoint(position, baseLayerMask);

        Collider2D restrictedTile = Physics2D.OverlapPoint(position, restrictedLayersMask);

        return baseTile != null && restrictedTile == null;
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(fadeDelay); // Wait before fading

        if (!playerInside) // Ensure player is still outside
        {
            spriteFade.StartFade(); // Start fade animation
            yield return new WaitForSeconds(spriteFade.FadeTime); // Wait for fade duration

            if (!playerInside) // Check again before destroying
            {
                Destroy(gameObject);
                TeleportationManager.Instance.SpawnTeleportField(); // Spawn new field
            }
        }
    }

    public float GetRadius()
    {
        return fieldRadius;
    }
}
