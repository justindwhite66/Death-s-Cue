using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthThreshold : MonoBehaviour
{
   [Header("Gate Threshold & Trigger")]
    [Range(0.01f, 1f)]
    [SerializeField] private float healthGatePercent = 0.5f;
    private bool gateActivated = false;

    [Header("Special Attack Settings")]
    [SerializeField] private float timeBetweenBursts = 1f;
    [SerializeField] private GameObject specialBulletPrefab;
    [SerializeField] private int projectileCount = 12;
    [SerializeField] private float specialBulletSpeed = 8f;
    [SerializeField] private float startingDistance = 2f;

    private void Start() {
        gateActivated = false;
    }
    public void CheckGate(int currentHealth, int maxHealth){
        Debug.Log($"[BossGate] Current: {currentHealth}, Max: {maxHealth}");
        if (gateActivated){
            return;
        } 
        float healthPercent = (float)currentHealth / maxHealth;


        if (healthPercent <= healthGatePercent){
            gateActivated = true;
            StartCoroutine(SpecialAttackRoutine());
        }
    }
    private IEnumerator SpecialAttackRoutine(){
        while (true){
            FirePattern();
            yield return new WaitForSeconds(timeBetweenBursts);
        }
    }
    private void FirePattern()
    {
       float angleStep = 360f / projectileCount;
        float angle = 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector3 direction = new Vector3(x, y, 0f).normalized;

            
            Vector3 spawnPos = transform.position + direction * startingDistance;

           
            GameObject bullet = Instantiate(specialBulletPrefab, spawnPos, Quaternion.identity);
            bullet.transform.right = direction;

            if (bullet.TryGetComponent(out Projectile projectile))
            {
                projectile.UpdateMoveSpeed(specialBulletSpeed);
                projectile.SetIsEnemyProjectile(true);
            }

            angle += angleStep;
        }
    }
}
