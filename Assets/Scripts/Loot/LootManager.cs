using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq; 

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }
    public LootSlots[] lootSlots;
    
    public static event Action OnInventoryChanged;
    
    // Data structure to store slot data
    private List<SlotData> persistentSlotData = new List<SlotData>();

    // Simple class to store slot information
    [System.Serializable]
    private class SlotData
    {
        public LootSO item;
        public int quantity;
        
        public SlotData(LootSO item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }
    }

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
        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        Loot.OnLootPickup -= AddLoot;
        // Unsubscribe from scene change events
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void AddLoot(LootSO lootSO, int quantity)
    {
        if (lootSO == null)
        {
            Debug.LogError("Attempted to add null loot item");
            return;
        }
        
        Debug.Log($"Adding {quantity}x {lootSO.name} to inventory");
        bool inventoryChanged = false;
        
        // First, check for existing slots with this item
        foreach (var slot in lootSlots)
        {
            if (slot == null) continue;

            if (slot.lootSO == lootSO)
            {
                slot.quantity += quantity;
                inventoryChanged = true;
                Debug.Log($"Added to existing slot, new quantity: {slot.quantity}");
                
                // Update UI if inventory is currently visible
                if (IsPauseMenuActive())
                {
                    slot.UpdateLootUI();
                }
                
                // Trigger event for hotbar update
                if (OnInventoryChanged != null)
                {
                    OnInventoryChanged();
                }
                
                return;
            }
        }

        // If no existing slot has this item, find an empty slot
        foreach (var slot in lootSlots)
        {
            if (slot == null) continue;

            if (slot.lootSO == null)
            {
                slot.lootSO = lootSO;
                slot.quantity = quantity;
                inventoryChanged = true;
                Debug.Log($"Added to empty slot: {lootSO.name}, quantity: {quantity}");
                
                // Update UI if inventory is currently visible
                if (IsPauseMenuActive())
                {
                    slot.UpdateLootUI();
                }
                
                // Trigger event for hotbar update
                if (OnInventoryChanged != null)
                {
                    OnInventoryChanged();
                }
                
                return;
            }
        }
        
        Debug.LogWarning($"No available slot for item: {lootSO.name}");
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
                
                // Update persistent data in DataManager
                if (DataManager.Instance != null)
                {
                    DataManager.Instance.RemoveUsedItem(lootSO, amount);
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
    
    // Completely reset and rebuild the inventory UI from the DataManager's persistent data
    public void RebuildInventoryFromPersistentData()
    {
        Debug.Log("Rebuilding inventory UI from persistent data");
        
        // First ensure all UI components exist and are initialized
        foreach (var slot in lootSlots)
        {
            if (slot == null) continue;

            // Make sure UI components exist
            if (slot.lootIcon == null)
                slot.lootIcon = slot.GetComponentInChildren<Image>(true);
            
            if (slot.quantityText == null)
                slot.quantityText = slot.GetComponentInChildren<TMP_Text>(true);
            
            if (slot.selectionOutline == null)
            {
                // Try to find selection outline
                var outlineImg = slot.transform.Find("SelectionOutline")?.GetComponent<Image>();
                if (outlineImg != null)
                    slot.selectionOutline = outlineImg;
            }
            
            // Reset slot data
            slot.lootSO = null;
            slot.quantity = 0;
        }
        
        // Then ask DataManager to refill the slots with persistent data
        if (DataManager.Instance != null)
        {
            DataManager.Instance.RestoreInventory();
            
            // Force UI update for all slots
            UpdateAllLootUI();
        }
        else
        {
            Debug.LogError("DataManager.Instance is null when trying to rebuild inventory");
        }
    }

    // Enhanced version of UpdateAllLootUI
    public void UpdateAllLootUI()
    {
        Debug.Log($"UpdateAllLootUI called - updating {lootSlots.Length} slots");
        
        foreach (var slot in lootSlots)
        {
            if (slot != null)
            {
                try
                {
                    // Make sure the components are found
                    if (slot.lootIcon == null)
                    {
                        // Find the Image component on the first child that has one
                        slot.lootIcon = slot.transform.GetComponentsInChildren<Image>(true)
                            .FirstOrDefault(img => img.gameObject != slot.gameObject);
                    }
                    
                    if (slot.quantityText == null)
                    {
                        // Find first Text component in children
                        slot.quantityText = slot.GetComponentInChildren<TMP_Text>(true);
                    }
                    
                    // Now update the UI with the new references
                    if (slot.lootSO != null && slot.quantity > 0)
                    {
                        if (slot.lootIcon != null)
                        {
                            slot.lootIcon.sprite = slot.lootSO.lootIcon;
                            slot.lootIcon.gameObject.SetActive(true);
                        }
                        
                        if (slot.quantityText != null)
                        {
                            slot.quantityText.text = slot.quantity.ToString();
                        }
                        
                        Debug.Log($"Updated UI for slot with {slot.lootSO.name} x{slot.quantity}");
                    }
                    else
                    {
                        if (slot.lootIcon != null)
                        {
                            slot.lootIcon.gameObject.SetActive(false);
                        }
                        
                        if (slot.quantityText != null)
                        {
                            slot.quantityText.text = "";
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error updating slot UI: {e.Message}");
                }
            }
        }
        
        // Also refresh hotbar if it exists
        if (HotbarManager.Instance != null)
        {
            HotbarManager.Instance.RefreshAllHotbarSlots();
        }
    }
    
    // Create a simulated pickup for each item in persistent inventory
    public void SimulatePickupsFromPersistentData()
    {
        Debug.Log("Simulating pickups for all persistent inventory items");
        
        // Clear existing inventory first
        foreach (var slot in lootSlots)
        {
            if (slot != null)
            {
                slot.lootSO = null;
                slot.quantity = 0;
            }
        }
        
        // Get persistent inventory from DataManager
        if (DataManager.Instance != null)
        {
            // Ask DataManager for a copy of the persistent inventory
            List<DataManager.LootItemInfo> persistentItems = DataManager.Instance.GetPersistentInventoryCopy();
            
            foreach (var item in persistentItems)
            {
                if (item.lootItem != null)
                {
                    Debug.Log($"Simulating pickup of {item.quantity}x {item.lootItem.name}");
                    
                    // Direct assignment to an empty slot instead of using AddLoot
                    bool assigned = false;
                    foreach (var slot in lootSlots)
                    {
                        if (slot != null && slot.lootSO == null)
                        {
                            // Directly populate the slot
                            slot.lootSO = item.lootItem;
                            slot.quantity = item.quantity;
                            assigned = true;
                            break;
                        }
                    }
                    
                    if (!assigned)
                    {
                        Debug.LogWarning($"No empty slot found for {item.lootItem.name}");
                    }
                }
            }
            
            // Force UI update for ALL slots - this is crucial
            foreach (var slot in lootSlots)
            {
                if (slot != null)
                {
                    slot.UpdateLootUI();
                }
            }
            
            // Also refresh hotbar
            if (HotbarManager.Instance != null)
            {
                HotbarManager.Instance.RefreshAllHotbarSlots();
            }
            
            Debug.Log("Pickup simulation complete");
        }
        else
        {
            Debug.LogError("DataManager.Instance is null when trying to simulate pickups");
        }
    }

    // Save the current slots data to our persistent structure
    private void SaveSlotsState()
    {
        persistentSlotData.Clear();
        
        foreach (var slot in lootSlots)
        {
            if (slot != null)
            {
                persistentSlotData.Add(new SlotData(slot.lootSO, slot.quantity));
            }
        }
        
        Debug.Log($"Saved {persistentSlotData.Count} slot states");
    }

    // Restore slots from our persistent data structure
    private void RestoreSlotsState()
    {
        Debug.Log($"Restoring {persistentSlotData.Count} slot states");
        
        // Make sure we have data to restore
        if (persistentSlotData.Count == 0)
        {
            Debug.Log("No slot data to restore");
            return;
        }
        
        // Ensure we don't try to restore more slots than we have
        int slotCount = Mathf.Min(lootSlots.Length, persistentSlotData.Count);
        
        for (int i = 0; i < slotCount; i++)
        {
            if (lootSlots[i] != null)
            {
                lootSlots[i].lootSO = persistentSlotData[i].item;
                lootSlots[i].quantity = persistentSlotData[i].quantity;
            }
        }
        
        // Update UI if needed
        UpdateAllLootUI();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Save the state of all slots before the scene unloads
        Debug.Log($"Scene {scene.name} unloading - saving slot state");
        SaveSlotsState();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Restore slot state after a scene loads
        Debug.Log($"Scene {scene.name} loaded - restoring slot state");
        RestoreSlotsState();
    }

    // Call this method when loot panel is opened
    public void OnLootPanelOpened()
    {
        Debug.Log("Loot panel opened - restoring slot data and updating UI");
        
        // Give time for all UI components to fully initialize
        StartCoroutine(RestoreSlotDataWithDelay(0.1f));
    }

    private IEnumerator RestoreSlotDataWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Debug.Log("Loot panel opened - restoring slot data and updating UI");
        
        // IMPORTANT: First re-grab references to all LootSlots in the scene
        // This is necessary because the UI components are recreated on scene change
        lootSlots = GameObject.FindObjectsOfType<LootSlots>();
        
        Debug.Log($"Found {lootSlots.Length} loot slots in current scene");
        
        // Check for persistent data in DataManager
        if (DataManager.Instance != null)
        {
            // Get persistent inventory from DataManager
            List<DataManager.LootItemInfo> persistentItems = DataManager.Instance.GetPersistentInventoryCopy();
            
            // Clear all slots first
            foreach (var slot in lootSlots)
            {
                if (slot != null)
                {
                    slot.lootSO = null;
                    slot.quantity = 0;
                }
            }
            
            // Populate slots with persistent data
            int slotIndex = 0;
            foreach (var item in persistentItems)
            {
                if (item.lootItem != null && slotIndex < lootSlots.Length)
                {
                    lootSlots[slotIndex].lootSO = item.lootItem;
                    lootSlots[slotIndex].quantity = item.quantity;
                    Debug.Log($"Assigned {item.lootItem.name} x{item.quantity} to slot {slotIndex}");
                    slotIndex++;
                }
            }
        }
        
        // Update UI for all slots
        UpdateAllLootUI();
    }
}