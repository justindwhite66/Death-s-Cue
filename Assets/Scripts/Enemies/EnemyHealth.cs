using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
   [SerializeField] private int startingHealth = 3;
   [SerializeField] private GameObject deathVFXPrefab;

   private int currentHealth;
   private Knockback knockback;
   private Flash flash;
   
   private void Awake(){
    knockback = GetComponent<Knockback>();
    flash = GetComponent<Flash>();
   }
   private void Start(){
    currentHealth = startingHealth;
   }

   public void TakeDamage(int damage){
    currentHealth -= damage;
    knockback.GetKnockedBack(PlayerController.Instance.transform, 15f);
    StartCoroutine(flash.FlashRoutine());
    StartCoroutine(CheckDetectDeathRoutine());
    
   }

   private IEnumerator CheckDetectDeathRoutine(){
    yield return new WaitForSeconds(flash.GetRestoreMatTime());
    DetectDeath();
   }

   private void DetectDeath(){
    if (currentHealth <= 0){
        Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
   }
}
