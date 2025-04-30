using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    [Header("Health Stats")]
    [SerializeField] public int maxShield = 3;
    [SerializeField] public int currentShield = 0;
    [SerializeField] public int maxHealth = 3;
    [SerializeField] public int currentHealth;

    [Header("Movement Stats")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] public int maxStamina = 3;
    [SerializeField] public int startingStamina = 3;
    [SerializeField] public int staminaRefreshRate = 10;

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

    // Add shield functionality
    public void AddShield(int amount)
    {
        currentShield = Mathf.Min(currentShield + amount, maxShield);
    }

    public bool HasShield()
    {
        return currentShield > 0;
    }

    // Getter methods to get base stat values
    public float GetBaseSpeed()
    {
        return 3f;
    }

    public int GetBaseStaminaRefreshRate()
    {
        return 3;
    }
}
