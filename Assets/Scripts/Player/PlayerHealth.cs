using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerHealth : Singleton<PlayerHealth>
{
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;
    [SerializeField] private GameObject bossHealthBar;

    public bool isDead {get; private set;}
    public bool isTeleporting{get; private set;}
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;

    const string SCENE_CHANGE = "Title";
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
        StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
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

        if (StatsManager.Instance.HasShield())
        {
            // Damage shield instead of health
            StatsManager.Instance.currentShield -= damageAmount;

            // Play shield impact effect
            StartCoroutine(flash.ShieldFlashRoutine());
            knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);

            StartCoroutine(DamageRecoveryRoutine());
            return;
        }

        // If player has no shield, damage health
        ScreenShakeManager.Instance.ShakeScreen();
        knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.HealthFlashRoutine());
        canTakeDamage = false;
        StatsManager.Instance.currentHealth -= damageAmount;
        StartCoroutine(DamageRecoveryRoutine());

        CheckPlayerDeath();
    }
   


    public void HealDamage(){
        if (StatsManager.Instance.currentHealth < StatsManager.Instance.maxHealth){
        StatsManager.Instance.currentHealth += 1;
        }
    }

    private IEnumerator DamageRecoveryRoutine(){
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void CheckPlayerDeath(){
        if (StatsManager.Instance.currentHealth <= 0 && !isDead){
            isDead = true;
            if (bossHealthBar != null)
        {
            Destroy(bossHealthBar);
        }
            Destroy(ActiveWeapon.Instance.gameObject);
            StatsManager.Instance.currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }
    private IEnumerator DeathLoadSceneRoutine(){
        yield return new WaitForSeconds(2f);
        
        // Reset player data BEFORE destroying player
        if (DataManager.Instance != null)
        {
            DataManager.Instance.ResetAllPlayerData();
        }
        
        // Reset stamina
        if (Stamina.Instance != null)
        {
            Stamina.Instance.ReplenshStaminaOnDeath();
        }
        
        // Load scene before destroying the player
        SceneManager.LoadSceneAsync(SCENE_CHANGE);
        
        Destroy(gameObject);
    }
}