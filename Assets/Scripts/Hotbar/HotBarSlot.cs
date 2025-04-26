using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text keyText; // Shows "Z", "X", or "C"
    public TMP_Text quantityText; // Add this for showing quantity
    
    private LootSO assignedItem;
    private int quantity = 0;
    
    // Initialize with key letter
    public void SetKeyText(string keyLetter)
    {
        if (keyText != null)
        {
            keyText.text = keyLetter;
        }
    }
    
    // Assign an item to this hotbar slot
    public void AssignItem(LootSO item)
    {
        assignedItem = item;
        
        // Find the corresponding inventory slot to get the quantity
        LootManager lootManager = LootManager.Instance;
        if (lootManager != null)
        {
            foreach (var slot in lootManager.lootSlots)
            {
                if (slot.lootSO == item)
                {
                    quantity = slot.quantity;
                    UpdateUI();
                    return;
                }
            }
        }
        
        // If we didn't find the item in inventory, use default quantity of 1
        quantity = 1;
        UpdateUI();
    }
    
    // Update the UI display
    public void UpdateUI()
    {
        if (assignedItem != null && quantity > 0)
        {
            if (itemIcon != null)
            {
                itemIcon.sprite = assignedItem.lootIcon;
                itemIcon.gameObject.SetActive(true);
            }
            
            if (quantityText != null)
            {
                quantityText.text = quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }
        }
        else
        {
            ClearSlot();
        }
    }
    
    // Refresh quantity from inventory
    public void RefreshQuantityFromInventory()
    {
        if (assignedItem == null) return;
        
        LootManager lootManager = LootManager.Instance;
        if (lootManager != null)
        {
            bool foundInInventory = false;
            
            foreach (var slot in lootManager.lootSlots)
            {
                if (slot.lootSO == assignedItem)
                {
                    quantity = slot.quantity;
                    foundInInventory = true;
                    UpdateUI();
                    break;
                }
            }
            
            // If item is no longer in inventory, clear the slot
            if (!foundInInventory)
            {
                ClearSlot();
            }
        }
    }
    
    // Clear this hotbar slot
    public void ClearSlot()
    {
        assignedItem = null;
        quantity = 0;
        
        // Update UI
        if (itemIcon != null)
        {
            itemIcon.gameObject.SetActive(false);
        }
        
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(false);
        }
    }
    
    // Check if slot has an item
    public bool HasItem()
    {
        return assignedItem != null && quantity > 0;
    }
    
    // Use the item in this slot
    public void UseItem()
    {
        if (assignedItem != null && quantity > 0)
        {
            Debug.Log($"Used item: {assignedItem.lootName}, Remaining: {quantity-1}");
            
            // Find the corresponding inventory slot and reduce quantity
            LootManager lootManager = LootManager.Instance;
            if (lootManager != null)
            {
                foreach (var slot in lootManager.lootSlots)
                {
                    if (slot.lootSO == assignedItem)
                    {
                        // Reduce quantity in inventory
                        slot.quantity--;
                        
                        // Update local quantity
                        quantity = slot.quantity;
                        
                        // Apply item effect (this would be your custom logic based on item type)
                        ApplyItemEffect(assignedItem);
                        
                        // Update inventory UI if open
                        if (IsInventoryOpen())
                        {
                            slot.UpdateLootUI();
                        }
                        
                        // Update hotbar UI
                        UpdateUI();
                        
                        // If quantity reached 0, clear the slot in inventory too
                        if (quantity <= 0)
                        {
                            slot.lootSO = null;
                            slot.quantity = 0;
                            
                            if (IsInventoryOpen())
                            {
                                slot.UpdateLootUI();
                            }
                            
                            ClearSlot();
                        }
                        
                        return;
                    }
                }
            }
            
            // If item not found in inventory but still in hotbar, just decrease local count
            quantity--;
            if (quantity <= 0)
            {
                ClearSlot();
            }
            else
            {
                UpdateUI();
            }
        }
    }
    
    // Apply effect based on item type
    private void ApplyItemEffect(LootSO item)
    {
        // This is where you'd implement different effects based on item properties
        // For example:
        
        // Health potion
        
    }
    
    // Check if inventory is open
    private bool IsInventoryOpen()
    {
        PauseMenuManager pauseMenu = FindObjectOfType<PauseMenuManager>();
        return pauseMenu != null && pauseMenu.IsLootPanelActive();
    }
}