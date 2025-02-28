using System.Collections;
using UnityEngine;

public class TeleportField : MonoBehaviour
{
    [SerializeField] private float fieldRadius = 8f;
    [SerializeField] private float fadeDelay = 3f; // Time before fade starts
    private bool playerInside = true;
    private bool isFading = false;
    private SpriteFade spriteFade;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        spriteFade = GetComponent<SpriteFade>();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer > fieldRadius && playerInside)
        {
            // Player has exited the field, start fading
            playerInside = false;
            fadeCoroutine = StartCoroutine(FadeAndDestroy());
        }
        else if (distanceToPlayer <= fieldRadius && !playerInside)
        {
            // Player re-entered the field, cancel fade if it's running
            playerInside = true;
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
                isFading = false;

                // Restore full opacity if it was fading
                spriteFade.ResetSpriteAlpha();
            }
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(fadeDelay); // Wait before fading

        if (!playerInside) // Ensure player is still outside
        {
            isFading = true;
            yield return StartCoroutine(spriteFade.SlowFadeRoutine()); // Start fade animation

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
