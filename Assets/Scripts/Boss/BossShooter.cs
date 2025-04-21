using System.Collections;
using UnityEngine;

public class BossShooter : MonoBehaviour, IEnemy
{
    [Header("Runtime Pattern Settings")]
    [SerializeField] private GameObject bulletPrefab;
    private float bulletMoveSpeed = 10f;
    private int burstCount = 1;
    private float timeBetweenBursts = 1f;
    private float restTime = 1f;
    private int projectilesPerBurst = 1;
    private float angleSpread = 0f;
    private float startingDistance = 0.1f;
    private bool stagger = false;
    private bool oscillate = false;

    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;
    private bool isShooting = false;
    readonly int ATTACK_HASH = Animator.StringToHash("IsAttacking");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack()
    {
        if (!isShooting)
            StartCoroutine(ShootRoutine());
    }

    public void ConfigurePattern(float speed, int bursts, float burstDelay, float rest, int countPerBurst, float spread, float spawnDistance, bool useStagger, bool useOscillate)
    {
        bulletMoveSpeed = Mathf.Max(speed, 0.1f);
        burstCount = Mathf.Max(bursts, 1);
        timeBetweenBursts = Mathf.Max(burstDelay, 0.1f);
        restTime = Mathf.Max(rest, 0.1f);
        projectilesPerBurst = Mathf.Max(countPerBurst, 1);
        angleSpread = spread;
        startingDistance = Mathf.Max(spawnDistance, 0.1f);
        stagger = useStagger;
        oscillate = useOscillate && stagger;
    }

    private IEnumerator ShootRoutine()
    {
        myAnimator.SetBool(ATTACK_HASH, true);
        isShooting = true;

        float startAngle, currentAngle, angleStep, endAngle;
        float timeBetweenProjectiles = stagger ? timeBetweenBursts / projectilesPerBurst : 0f;

        for (int i = 0; i < burstCount; i++)
        {
            if (!oscillate || i % 2 == 0)
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
            else
            {
                TargetConeOfInfluence(out startAngle, out currentAngle, out angleStep, out endAngle);
                currentAngle = endAngle;
                endAngle = startAngle;
                startAngle = currentAngle;
                angleStep *= -1;
            }

            for (int j = 0; j < projectilesPerBurst; j++)
            {
                Vector2 pos = FindBulletSpawnPos(currentAngle);
                GameObject newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                newBullet.transform.right = newBullet.transform.position - transform.position;

                if (newBullet.TryGetComponent(out Projectile projectile))
                {
                    projectile.UpdateMoveSpeed(bulletMoveSpeed);
                    projectile.SetIsEnemyProjectile(true);
                }

                currentAngle += angleStep;

                if (stagger)
                    yield return new WaitForSeconds(timeBetweenProjectiles);
            }

            if (!stagger)
                yield return new WaitForSeconds(timeBetweenBursts);
        }

        myAnimator.SetBool(ATTACK_HASH, false);
        yield return new WaitForSeconds(restTime);
        isShooting = false;
    }

    private void TargetConeOfInfluence(out float startAngle, out float currentAngle, out float angleStep, out float endAngle)
    {
        Vector2 dir = PlayerController.Instance.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float halfSpread = angleSpread / 2;

        startAngle = angle - halfSpread;
        endAngle = angle + halfSpread;
        currentAngle = startAngle;
        angleStep = (angleSpread != 0 && projectilesPerBurst > 1) ? angleSpread / (projectilesPerBurst - 1) : 0f;
    }

    private Vector2 FindBulletSpawnPos(float angle)
    {
        float x = transform.position.x + startingDistance * Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = transform.position.y + startingDistance * Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector2(x, y);
    }
}
