using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }
    public LootSlots[] lootSlots;
    
    // Add event that gets triggered when inventory changes
    public static event Action OnInventoryChanged;
    
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
            return;
        }
        
        bool inventoryChanged = false;
        
        foreach (var slot in lootSlots)
        {
            // Make sure loot slot is not null
            if (slot == null)
            {
                continue;
            }

            // If slot has same LootSO, add to quantity
            if (slot.lootSO == lootSO)
            {
                slot.quantity += quantity;
                inventoryChanged = true;
                
                // Only update UI if inventory is currently visible
                if (IsPauseMenuActive())
                {
                    slot.UpdateLootUI();
                }
                
                // Trigger event for hotbar update
                if (inventoryChanged && OnInventoryChanged != null)
                {
                    OnInventoryChanged();
                }
                
                return;
            }
        }

        foreach (var slot in lootSlots)
        {
            // Make sure loot slot is not null
            if (slot == null)
            {
                continue;
            }

            // If slot empty, add LootSO and quantity
            if (slot.lootSO == null)
            {
                slot.lootSO = lootSO;
                slot.quantity = quantity;
                inventoryChanged = true;
                
                // Only update UI if the inventory is currently visible
                if (IsPauseMenuActive())
                {
                    slot.UpdateLootUI();
                }
                
                // Trigger event for hotbar update
                if (inventoryChanged && OnInventoryChanged != null)
                {
                    OnInventoryChanged();
                }
                
                return;
            }
        }   
    }
    
    // Method to remove items from inventory
    public void RemoveItem(LootSO lootSO, int amount = 1)
    {
        if (lootSO == null) return;
        
        bool inventoryChanged = false;
        
        foreach (var slot in lootSlots)
        {
            if (slot == null) continue;
            
            if (slot.lootSO == lootSO)
            {
                // Reduce quantity
                slot.quantity -= amount;
                inventoryChanged = true;
                
                // If quantity reached 0, clear slot
                if (slot.quantity <= 0)
                {
                    slot.lootSO = null;
                    slot.quantity = 0;
                }
                
                // Update UI if inventory open
                if (IsPauseMenuActive())
                {
                    slot.UpdateLootUI();
                }
                
                break;
            }
        }
        
        // Trigger event for hotbar update
        if (inventoryChanged && OnInventoryChanged != null)
        {
            OnInventoryChanged();
        }
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
        
        // Also refresh hotbar if it exists
        if (HotbarManager.Instance != null)
        {
            HotbarManager.Instance.RefreshAllHotbarSlots();
        }
    }
}