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
        UpdateMoveSpeed();
    }

    public void UpdateMoveSpeed()
    {
        statValue[0].GetComponent<TMP_Text>().text = StatsManager.Instance.moveSpeed.ToString();
    }
}
