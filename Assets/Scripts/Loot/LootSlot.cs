using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootSlot : MonoBehaviour
{
    public LootSO lootSO;
    public int quantity;

    public Image lootImage;
    public TMP_Text quantityText;

    public void UpdateUI()
    {
        if (lootSO != null)
        {
            lootImage.sprite = lootSO.lootIcon;
            lootImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            lootImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }
}