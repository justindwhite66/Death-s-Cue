using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statValue;

    private void Start()
    {
        // Initialize the stats panel with the current stats
        UpdateMaxHealth();
        UpdateCurrentHealth();
        UpdateMoveSpeed();
    }

    private void Update()
    {
        // Update the stats panel in real-time
        UpdateMaxHealth();
        UpdateCurrentHealth();
        UpdateMoveSpeed();
    }

    public void UpdateMaxHealth()
    {
        statValue[0].GetComponent<TMP_Text>().text = StatsManager.Instance.maxHealth.ToString();
    }

    public void UpdateCurrentHealth()
    {
        statValue[1].GetComponent<TMP_Text>().text = StatsManager.Instance.currentHealth.ToString();
    }

    public void UpdateMoveSpeed()
    {
        statValue[2].GetComponent<TMP_Text>().text = StatsManager.Instance.moveSpeed.ToString();
    }
}
