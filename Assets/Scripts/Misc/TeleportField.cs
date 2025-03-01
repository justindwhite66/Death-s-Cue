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

            }
            spriteFade.StopFade();
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(fadeDelay); // Wait before fading

        if (!playerInside) // Ensure player is still outside
        {
            spriteFade.StartFade();
            yield return new WaitForSeconds(spriteFade.fadeTime); // Start fade animation

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
