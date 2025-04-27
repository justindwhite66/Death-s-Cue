using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statValue;
    private TMP_Text[] statTexts;

    private void Awake()
    {
        // Cache TMP_Text components to avoid GetComponent calls every frame
        statTexts = new TMP_Text[statValue.Length];
        for (int i = 0; i < statValue.Length; i++)
        {
            statTexts[i] = statValue[i].GetComponent<TMP_Text>();
        }
    }

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
        statTexts[0].text = StatsManager.Instance.maxHealth.ToString();
    }

    public void UpdateCurrentHealth()
    {
        statTexts[1].text = StatsManager.Instance.currentHealth.ToString();
    }

    public void UpdateMoveSpeed()
    {
        statTexts[2].text = StatsManager.Instance.moveSpeed.ToString();
    }

    public void UpdateStaminaRefreshRate()
    {
        statTexts[3].text = StatsManager.Instance.staminaRefreshRate.ToString();
    }

    public void UpdateAllStats()
    {
        UpdateMaxHealth();
        UpdateCurrentHealth();
        UpdateMoveSpeed();
        UpdateStaminaRefreshRate();
    }
}
