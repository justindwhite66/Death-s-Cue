using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text keyText; // Shows "Z", "X", or "C"
    public TMP_Text quantityText;
    
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
    
    // Assign item to hotbar slot
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
        
        // If item not found in inventory, set quantity to 1
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
            
            // If item no longer in inventory, clear slot
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
            // Find corresponding inventory slot and reduce quantity
            LootManager lootManager = LootManager.Instance;
            if (lootManager != null)
            {
                foreach (var slot in lootManager.lootSlots)
                {
                    if (slot.lootSO == assignedItem)
                    {
                        // Reduce quantity in inventory
                        slot.quantity--;
                        
                        quantity = slot.quantity;
                        
                        // Apply item effect
                        ApplyItemEffect(assignedItem);
                        
                        if (IsInventoryOpen())
                        {
                            slot.UpdateLootUI();
                        }
                        
                        UpdateUI();
                        
                        // If quantity reached 0, clear slot in inventory too
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
            
            // Item not in inventory but still in hotbar, decrease local count
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
        if (item == null) return;

        // Handle healing or damage items
        if (item.currentHealth != 0)
        {
            int currentHealth = StatsManager.Instance.currentHealth;
            int maxHealth = StatsManager.Instance.maxHealth;
            
            if (item.currentHealth > 0)
            {
                // Calculate how much healing can be applied
                int healthToAdd = Mathf.Min(
                    item.currentHealth, 
                    maxHealth - currentHealth
                );
                
                // Apply the healing (except overheal)
                if (healthToAdd > 0)
                {
                    StatsManager.Instance.currentHealth += healthToAdd;
                }
            }
            else
            {
                // Apply damage (negative health)
                StatsManager.Instance.currentHealth += item.currentHealth;
            }
        }
        
        // Handle move speed boost items
        if (item.moveSpeed != 0)
        {
            // Add modifier to tracking dictionary
            if (!activeFloatModifiers.ContainsKey("moveSpeed"))
            {
                activeFloatModifiers["moveSpeed"] = new List<float>();
            }
            activeFloatModifiers["moveSpeed"].Add(item.moveSpeed);

            // Apply move speed boost
            float currentMoveSpeed = StatsManager.Instance.moveSpeed;
            float newMoveSpeed = currentMoveSpeed + item.moveSpeed;
            
            // Ensure move speed doesn't fall below 0.5
            newMoveSpeed = Mathf.Max(0.5f, newMoveSpeed);
            
            StatsManager.Instance.moveSpeed = newMoveSpeed;
            
            // If temporary effect, start coroutine to remove after duration
            if (item.duration > 0)
            {
                // Store reference to the coroutine if stop needed
                StartCoroutine(ResetFloatStatAfterDelay(
                    "moveSpeed", 
                    item.moveSpeed, 
                    item.duration
                ));
            }
        }
        
        // Handle stamina refresh rate items
        if (item.staminaRefreshRate != 0)
        {
            // Add modifier to tracking dictionary
            if (!activeIntModifiers.ContainsKey("staminaRefresh"))
            {
                activeIntModifiers["staminaRefresh"] = new List<int>();
            }
            var refreshKey = "staminaRefresh";
            activeIntModifiers[refreshKey].Add(item.staminaRefreshRate);
            
            // Apply modifier
            int currentRate = StatsManager.Instance.staminaRefreshRate;
            int newRate = currentRate + item.staminaRefreshRate;
            // Ensure it doesn't go below 1
            newRate = Mathf.Max(1, newRate); 
            
            StatsManager.Instance.staminaRefreshRate = newRate;
            
            // Restart stamina refresh routine with updated rate
            if (Stamina.Instance != null)
            {
                Stamina.Instance.RestartStaminaRefreshRoutine();
            }
            
            // If temporary effect, start coroutine to remove it
            if (item.duration > 0)
            {
                StartCoroutine(ResetIntStatAfterDelay(
                    refreshKey,
                    item.staminaRefreshRate, 
                    item.duration
                ));
            }
        }
        
        UpdateStatsUI();
    }

    // Track active stat modifiers in HotbarSlot class
    private static Dictionary<string, List<float>> activeFloatModifiers = 
        new Dictionary<string, List<float>>();
    private static Dictionary<string, List<int>> activeIntModifiers = 
        new Dictionary<string, List<int>>();
    private static Dictionary<string, List<Coroutine>> activeCoroutines = 
        new Dictionary<string, List<Coroutine>>();

    // Modified coroutine to reset float stat after delay
    private IEnumerator ResetFloatStatAfterDelay(
        string statType, 
        float boostAmount, 
        float duration
    )
    {
        // If duration is 0 or negative, effect is permanent
        if (duration <= 0) yield break;
        
        // Track modifier
        if (!activeFloatModifiers.ContainsKey(statType))
        {
            activeFloatModifiers[statType] = new List<float>();
        }
        activeFloatModifiers[statType].Add(boostAmount);
        
        // Store coroutine reference
        if (!activeCoroutines.ContainsKey(statType))
        {
            activeCoroutines[statType] = new List<Coroutine>();
        }
        
        yield return new WaitForSeconds(duration);
        
        if (statType == "moveSpeed")
        {
            float currentSpeed = StatsManager.Instance.moveSpeed;
            float newSpeed = currentSpeed - boostAmount;
            StatsManager.Instance.moveSpeed = Mathf.Max(0.5f, newSpeed);
        }
        
        // Remove modifier from tracking
        if (activeFloatModifiers.ContainsKey(statType))
        {
            activeFloatModifiers[statType].Remove(boostAmount);
        }
        
        UpdateStatsUI();
    }

    // Modified coroutine to reset int stat after delay
    private IEnumerator ResetIntStatAfterDelay(
        string statType, 
        int boostAmount, 
        float duration
    )
    {
        if (duration <= 0) yield break;
        
        // Track modifier
        if (!activeIntModifiers.ContainsKey(statType))
        {
            activeIntModifiers[statType] = new List<int>();
        }
        activeIntModifiers[statType].Add(boostAmount);
        
        // Store coroutine reference
        if (!activeCoroutines.ContainsKey(statType))
        {
            activeCoroutines[statType] = new List<Coroutine>();
        }
        
        yield return new WaitForSeconds(duration);
        
        if (statType == "staminaRefresh")
        {
            int currentRefresh = StatsManager.Instance.staminaRefreshRate;
            int newRefresh = currentRefresh - boostAmount;
            StatsManager.Instance.staminaRefreshRate = Mathf.Max(1, newRefresh);
            
            // Restart the stamina refresh routine with new rate
            if (Stamina.Instance != null)
            {
                Stamina.Instance.RestartStaminaRefreshRoutine();
            }
        }
        
        // Remove modifier from tracking
        if (activeIntModifiers.ContainsKey(statType))
        {
            activeIntModifiers[statType].Remove(boostAmount);
        }
        
        UpdateStatsUI();
    }

    // Add public static method to reapply all active modifiers
    public static void ReapplyAllActiveModifiers()
    {
        // Get base values from StatsManager
        float baseSpeed = StatsManager.Instance.GetBaseSpeed();
        int baseRefresh = StatsManager.Instance.GetBaseStaminaRefreshRate();
                
        // Set back to base values first
        StatsManager.Instance.moveSpeed = baseSpeed;
        StatsManager.Instance.staminaRefreshRate = baseRefresh;
        
        // Apply all active modifiers from scratch
        if (activeFloatModifiers.ContainsKey("moveSpeed"))
        {
            foreach (float modifier in activeFloatModifiers["moveSpeed"])
            {
                StatsManager.Instance.moveSpeed += modifier;
            }
        }
        
        // Apply stamina refresh rate modifiers
        var refreshKey = "staminaRefresh";
        if (activeIntModifiers.ContainsKey(refreshKey))
        {
            foreach (int modifier in activeIntModifiers[refreshKey])
            {
                StatsManager.Instance.staminaRefreshRate += modifier;
            }
            
            // Restart stamina refresh routine with updated rate
            if (Stamina.Instance != null)
            {
                Stamina.Instance.RestartStaminaRefreshRoutine();
            }
        }
        
        UpdateStatsUI();
    }

    private static void UpdateStatsUI()
    {
        // Find the StatsUI component if it exists
        StatsUI statsUI = FindObjectOfType<StatsUI>();
        if (statsUI != null)
        {
            statsUI.UpdateAllStats();
        }
    }
    
    // Check if inventory is open
    private bool IsInventoryOpen()
    {
        PauseMenuManager pauseMenu = FindObjectOfType<PauseMenuManager>();
        return pauseMenu != null && pauseMenu.IsLootPanelActive();
    }
}