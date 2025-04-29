using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossHealthBarManager : MonoBehaviour
{
    [SerializeField] private Slider bossHealthSlider;  // Assign your slider object here
    private EnemyHealth bossEnemyHealth;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Indoor_5")  // Your boss room scene name
        {
            Invoke(nameof(FindBoss), 0.5f); // small delay to let objects spawn
        }
        else
        {
            bossHealthSlider.gameObject.SetActive(false);
        }
    }

    private void FindBoss()
    {
        bossEnemyHealth = FindObjectOfType<EnemyHealth>();  // Find the boss
        if (bossEnemyHealth != null && bossEnemyHealth.IsBoss())
        {
            bossHealthSlider.gameObject.SetActive(true);
            bossHealthSlider.maxValue = bossEnemyHealth.MaxHealth;
            bossHealthSlider.value = bossEnemyHealth.CurrentHealth;
        }
    }

    private void Update()
    {
         if (bossEnemyHealth != null)
        {
            bossHealthSlider.value = bossEnemyHealth.CurrentHealth;

            
            if (bossEnemyHealth.CurrentHealth <= 0)
            {
                bossHealthSlider.gameObject.SetActive(false);
                bossEnemyHealth = null; 
            }
        }
    }
}
