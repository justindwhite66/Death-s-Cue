using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryCollider : MonoBehaviour
{
[SerializeField] private GameObject reflectedProjectilePrefab;
    private Sword sword;
 
  private void Start() {
    sword = ActiveWeapon.Instance.CurrentActiveWeapon as Sword;
  }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (sword == null || !sword.IsParrying()) return;

        Projectile projectile = collision.GetComponent<Projectile>();
        if (projectile != null && projectile.IsEnemyProjectile()){
            Vector3 spawnPosition = projectile.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; 
            Vector3 reflectedDirection = (mousePos - spawnPosition).normalized;
            Destroy(projectile.gameObject);
            GameObject reflected = Instantiate(reflectedProjectilePrefab, spawnPosition, Quaternion.identity);
            reflected.transform.right = reflectedDirection;
        }
    }
}
