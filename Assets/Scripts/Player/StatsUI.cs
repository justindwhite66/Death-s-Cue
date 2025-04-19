using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statValue;

    private void Start()
    {
        UpdateAllStats();
    }

    private void Update()
    {
        UpdateAllStats();
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

    public void UpdateStaminaRefreshRate()
    {
        statValue[3].GetComponent<TMP_Text>().text = StatsManager.Instance.staminaRefreshRate.ToString();
    }

    public void UpdateAllStats()
    {
        UpdateMaxHealth();
        UpdateCurrentHealth();
        UpdateMoveSpeed();
        UpdateStaminaRefreshRate();
    }
}
