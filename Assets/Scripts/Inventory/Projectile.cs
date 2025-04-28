using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 22f;
    [SerializeField] private GameObject smallTeleportPrefab;
    [SerializeField] private GameObject particleOnHitPrefabVFX;
    [SerializeField] private bool isEnemyProjectile = false;
    [SerializeField] private float projectileRange = 10f;
    [SerializeField] private float collisionIgnoreTime = 1f;
    public bool bornInsideField {get; set;} = false;
    public bool IsReflectedProjectile {get; set;} = false;
     

    private Vector3 startPosition;
    private Collider2D projectileCollider;

    public bool passedThroughField = false;

    private void Start() {
        startPosition = transform.position;
        projectileCollider = GetComponent<Collider2D>();

        if (projectileCollider != null){
            projectileCollider.enabled = false;
            Invoke(nameof(EnableCollider), collisionIgnoreTime);
        }
    }

    private void Update()
    {
        MoveProjectile();
        DetectFireDistance();
    }

    public void UpdateProjectileRange(float projectileRange){
        this.projectileRange = projectileRange;
    }

    public void UpdateMoveSpeed(float moveSpeed){
        this.moveSpeed = moveSpeed;
    }
    private void EnableCollider(){
        if (projectileCollider != null){
            projectileCollider.enabled = true;
        }
    }

private void OnTriggerEnter2D(Collider2D other)
{
    EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
    Indestructible indestructible = other.GetComponent<Indestructible>();
    PlayerHealth player = other.GetComponent<PlayerHealth>();

    if (!other.isTrigger && (enemyHealth || indestructible || player))
    {

        bool shouldSpawn = false;

        if (IsReflectedProjectile ||(!IsEnemyProjectile() && bornInsideField))
    
        {
            shouldSpawn = true;
        }
        

        if (shouldSpawn && smallTeleportPrefab != null)
        {
            
            GameObject newField = Instantiate(smallTeleportPrefab, transform.position, Quaternion.identity);
            TeleportField fieldComponent = newField.GetComponent<TeleportField>();
            if (fieldComponent != null)
            {
                fieldComponent.isSmallField = true;
            }
            
        }

        // Only mark impact as recorded AFTER confirming the correct collision logic
        if ((player && isEnemyProjectile) || (enemyHealth && !isEnemyProjectile))
        {
            player?.TakeDamage(1, transform);
            enemyHealth?.TakeDamage(1);
            Instantiate(particleOnHitPrefabVFX, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        

        // Now that all conditions have been checked, set impact as recorded
        Instantiate(particleOnHitPrefabVFX, transform.position, transform.rotation);
        DestroyProjectile();
    }
}

    private void DetectFireDistance() {
        if (Vector3.Distance(transform.position, startPosition) > projectileRange) {
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }

    private void DestroyProjectile()
{
    if (ProjectileManager.Instance != null)
    {
        // Only update if HitIndestructible is NOT already set to true
        if (!ProjectileManager.Instance.HitIndestructible)
        {
            
            ProjectileManager.Instance.SetLastDestroyedProjectile(transform.position, false);
        }
    }

    Destroy(gameObject);
}

    public bool IsEnemyProjectile(){
        return isEnemyProjectile;
    }

    public void SetIsEnemyProjectile(bool value){
        isEnemyProjectile = value;
    }
    public void InitializeAsReflected()
{
    IsReflectedProjectile = true;
}
}