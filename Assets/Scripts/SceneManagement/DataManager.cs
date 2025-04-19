using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    private bool isResettingStats = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Call this when player dies or quits
    public void ResetAllPlayerData()
    {
        Debug.Log("Resetting all player data");
        isResettingStats = true;
        
        // Reset StatsManager
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
            StatsManager.Instance.moveSpeed = 1f; // Reset to default speed
            StatsManager.Instance.staminaRefreshRate = 3; // Reset to default refresh rate
        }
        
        // Reset Stamina
        if (Stamina.Instance != null)
        {
            Stamina.Instance.ReplenshStaminaOnDeath();
        }
        
        isResettingStats = false;
        Debug.Log("Player data reset complete");
    }
    
    // Helper method to check if currently resetting
    public bool IsResettingStats()
    {
        return isResettingStats;
    }
}