using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    


public class PlayerHealth : Singleton<PlayerHealth>
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    private Slider healthSlider;

    const string HEALTH_TEXT = "Health Slider";

    protected override void Awake()
    {
        base.Awake();
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAi enemy = collision.gameObject.GetComponent<EnemyAi>();

        if (enemy){
            TakeDamage(1, collision.transform);
        }
    }
    public void TakeDamage(int damageAmount, Transform hitTransform){
        if (!canTakeDamage) {return ;}

        ScreenShakeManager.Instance.ShakeScreen();
        knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        currentHealth -= damageAmount;
        Debug.Log(currentHealth);
        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckPlayerDeath();
    }

    public void HealDamage(){
        if (currentHealth < maxHealth){
        currentHealth += 1;
        UpdateHealthSlider();
        }
    }

    private IEnumerator DamageRecoveryRoutine(){
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void CheckPlayerDeath(){
        if (currentHealth <= 0){
            currentHealth = 0;
            Debug.Log("Player Death");
        }
    }

    private void UpdateHealthSlider(){
        if (healthSlider == null){
            healthSlider = GameObject.Find(HEALTH_TEXT).GetComponent<Slider>();
        }
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}
