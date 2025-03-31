using System.Collections;
using UnityEngine;

public class TeleportField : MonoBehaviour
{
    [SerializeField] private float fieldRadius = 8f;
    [SerializeField] private float fadeDelay = 3f; // Time before fade starts
    [SerializeField] private LayerMask restrictedLayersMask;
    [SerializeField] private GameObject smallTeleportPrefab;
    [SerializeField] private bool isSmallField = false;
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

public bool IsValidTeleportLocation(Vector3 position)
{
    float checkRadius = 0.1f; // Small detection radius

    // Check if the target position has a restricted layer (Water or Foreground)
    Collider2D restrictedTile = Physics2D.OverlapCircle(position, checkRadius, restrictedLayersMask);

    return restrictedTile == null; // Valid if NO restricted tile is detected
}


private void OnTriggerExit2D(Collider2D other) {
    Projectile projectile = other.GetComponent<Projectile>();
    if (projectile != null){
        if (projectile.TrackingCancelled){
            return;
        }
        
        StartCoroutine(CheckProjectileCollision(projectile));
    }
}
private void OnTriggerEnter2D(Collider2D other)
{
    Projectile projectile = other.GetComponent<Projectile>();

    if (projectile != null && isSmallField)
    {
        
        projectile.TrackingCancelled = true; // NEW: Cancel tracking
    }
}

private IEnumerator CheckProjectileCollision(Projectile projectile)
{
    // Wait until the projectile is destroyed
    while (projectile != null)
    {
        yield return null;
    }

    if (ProjectileManager.Instance.HitIndestructible)
    {
         SpawnSmallField(ProjectileManager.Instance.LastDestroyedProjectilePosition);
    }
       
    
  
}


private void SpawnSmallField(Vector3 position)
{
    if (smallTeleportPrefab != null)
    {
        
        GameObject newField = Instantiate(smallTeleportPrefab, position, Quaternion.identity);

        TeleportField fieldComponent = newField.GetComponent<TeleportField>();
        if (fieldComponent != null){
            fieldComponent.isSmallField = true;
        }
    }
    
}

    private IEnumerator FadeAndDestroy()
{
    yield return new WaitForSeconds(fadeDelay);
    spriteFade.StartFade();
    yield return new WaitForSeconds(spriteFade.FadeTime);

    // Check if this is the MAIN teleport field before spawning a new one
    if (TeleportationManager.Instance.currentField == this.gameObject)
    {
        TeleportationManager.Instance.SpawnTeleportField();
    }

    Destroy(gameObject);
}

    public float GetRadius()
    {
        return fieldRadius;
    }

    public bool IsValidTeleportLocation(Vector3 position, LayerMask restrictedLayersMask)
    {
        float checkRadius = 0.1f;
        Collider2D restrictedTile = Physics2D.OverlapCircle(position, checkRadius, restrictedLayersMask);
        return restrictedTile == null;
    }
}