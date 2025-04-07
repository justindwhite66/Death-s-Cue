using System.Collections;
using System.Collections.Generic;
// using TMPro;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    // Menu
    public GameObject LootMenu;
    private bool menuActivated;

    // Gold
    // public int gold;
    // public TMP_Text goldText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("ToggleLootMenu") && menuActivated)
        {
            Time.timeScale = 1f; // Resume the game
            LootMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetButtonDown("ToggleLootMenu") && !menuActivated)
        {
            Time.timeScale = 0f; // Pause the game
            LootMenu.SetActive(true);
            menuActivated = true;
        }
    }

    private void OnEnable()
    {
        Loot.OnLootPickup += AddLoot;
    }

    private void OnDisable()
    {
        Loot.OnLootPickup -= AddLoot;
    }

    public void AddLoot(LootSO lootSO, int quantity)
    {
        // if (lootSO.isGold)
        // {
        //     gold += quantity;
        //     goldText.text = gold.ToString();
        //     return;
        // }
        // else

        //UpdateUI();
        Inventory.Instance.AddLoot(lootSO, quantity);
    }
}
