using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class LootSlots : MonoBehaviour
{
    public LootSO lootSO;
    public int quantity;

    public Image lootIcon;
    public TMP_Text quantityText;

    public void UpdateLootUI()
    {
        if (lootSO != null)
        {
            lootIcon.sprite = lootSO.lootIcon;
            lootIcon.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            lootIcon.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }
}
