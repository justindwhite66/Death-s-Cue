using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;




public class PlayerHealth : Singleton<PlayerHealth>
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private int currentHealth;
    public bool isDead {get; private set;}
    public bool isTeleporting{get; private set;}
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    private Slider healthSlider;

    const string HEALTH_TEXT = "Health Slider";
    const string TOWN_TEXT = "Town_Scene";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    void Start()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthSlider();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        EnemyAi enemy = collision.gameObject.GetComponent<EnemyAi>();

        if (enemy && !isTeleporting){
            TakeDamage(1, collision.transform);
        }
    }
    public void TakeDamage(int damageAmount, Transform hitTransform){
        if (PlayerController.Instance != null && PlayerController.Instance.IsInvincible()){ return;}
        if (!canTakeDamage) {return ;}

        ScreenShakeManager.Instance.ShakeScreen();
        knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        currentHealth -= damageAmount;
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
        if (currentHealth <= 0 && !isDead){
            isDead = true;
            Destroy(ActiveWeapon.Instance.gameObject);
            currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }
    private IEnumerator DeathLoadSceneRoutine(){
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        Stamina.Instance.ReplenshStaminaOnDeath();
        SceneManager.LoadScene(TOWN_TEXT);
    }

    private void UpdateHealthSlider(){
        if (healthSlider == null){
            healthSlider = GameObject.Find(HEALTH_TEXT).GetComponent<Slider>();
        }
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}