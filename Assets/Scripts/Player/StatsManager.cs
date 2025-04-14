using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    //[Header("Health Stats")]
    //[SerializeField] public int maxShield;
    //[SerializeField] public int currentShield;
    [SerializeField] public int maxHealth = 3;
    [SerializeField] public int currentHealth;

    [Header("Movement Stats")]
    [SerializeField] public float moveSpeed = 1f;

    //[Header("Combat Stats")]
    //[SerializeField] public float teleportCooldownTime;

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
}
