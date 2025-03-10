using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvinciblityDamageCollider : MonoBehaviour
{
    [SerializeField] private int damageAmount = 3;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth)){
            enemyHealth.TakeDamage(damageAmount);
        }
    }
    
}
