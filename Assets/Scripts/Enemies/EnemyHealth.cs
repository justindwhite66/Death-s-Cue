using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EnemyHealth : MonoBehaviour
{
   [SerializeField] private int startingHealth = 3;
   [SerializeField] private GameObject deathVFXPrefab;
   [SerializeField] private float knockBackThrust = 15f;
   [SerializeField] private Slider slider;
   [SerializeField] private bool isBoss = false;

   private int currentHealth;
   private Knockback knockback;
   private Flash flash;
   private BossHealthThreshold bossHealthThreshold;

   public int CurrentHealth => currentHealth;
   public int MaxHealth => startingHealth;
   
   private void Awake(){
    knockback = GetComponent<Knockback>();
    flash = GetComponent<Flash>();
    bossHealthThreshold = GetComponent<BossHealthThreshold>();
   }
   private void Start(){
    currentHealth = startingHealth;
    
    if (isBoss && slider != null){
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Indoor_5"){

        slider.gameObject.SetActive(true);
        slider.maxValue = startingHealth;
        slider.value = currentHealth;
        }
        else{
            slider.gameObject.SetActive(false);
        }
    }

   }

   public void TakeDamage(int damage){
    currentHealth -= damage;
    if (isBoss && slider != null){
        slider.value = currentHealth;
    }
    if (bossHealthThreshold != null){
        bossHealthThreshold.CheckGate(currentHealth, startingHealth);
    }
    knockback.GetKnockedBack(PlayerController.Instance.transform, knockBackThrust);
    StartCoroutine(flash.HealthFlashRoutine());
    StartCoroutine(CheckDetectDeathRoutine());
    
   }

   private IEnumerator CheckDetectDeathRoutine(){
    yield return new WaitForSeconds(flash.GetRestoreMatTime());
    DetectDeath();
   }

   private void DetectDeath(){
    if (currentHealth <= 0){

        if (isBoss && slider != null){
            slider.gameObject.SetActive(false);
        }
        Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        GetComponent<PickUpSpawner>().DropItems();
        Destroy(gameObject);
    }
   }
}
