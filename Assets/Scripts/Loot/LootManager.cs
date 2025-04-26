using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }
    public LootSlots[] lootSlots;
    
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
        if (lootSO == null)
        {
            Debug.LogError("LootSO is null");
            return;
        }
        
        foreach (var slot in lootSlots)
        {
            // make sure the loot slot is not null
            if (slot == null)
            {
                Debug.LogError("LootSlot is null");
                continue;
            }

            // If this slot has the same LootSO, add to the quantity
            if (slot.lootSO == lootSO)
            {
                slot.quantity += quantity;
                // Only update UI if the inventory is currently visible
                if (IsPauseMenuActive())
                {
                    slot.UpdateLootUI();
                }
                return;
            }
        }

        foreach (var slot in lootSlots)
        {
            // make sure the loot slot is not null
            if (slot == null)
            {
                Debug.LogError("LootSlot is null");
                continue;
            }

            // If this slot is empty, add the LootSO and quantity
            if (slot.lootSO == null)
            {
                slot.lootSO = lootSO;
                slot.quantity = quantity;
                // Only update UI if the inventory is currently visible
                if (IsPauseMenuActive())
                {
                    slot.UpdateLootUI();
                }
                return;
            }
        }
        
        // inventory is full
        Debug.Log("Inventory is full");
    }
    
    // Check if pause menu is active
    private bool IsPauseMenuActive()
    {
        PauseMenuManager pauseMenu = FindObjectOfType<PauseMenuManager>();
        return pauseMenu != null && pauseMenu.IsLootPanelActive();
    }
    
    // Method to update all UI elements
    public void UpdateAllLootUI()
    {
        foreach (var slot in lootSlots)
        {
            if (slot != null)
            {
                slot.UpdateLootUI();
            }
        }
    }
}