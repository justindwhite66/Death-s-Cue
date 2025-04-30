using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    
    private bool isResettingStats = false;
    
    // Structure to hold inventory data
    [System.Serializable]
    private class SavedLootItem
    {
        public LootSO lootItem;
        public int quantity;
        
        public SavedLootItem(LootSO item, int qty)
        {
            lootItem = item;
            quantity = qty;
        }
    }
    
    // List to store the persistent inventory
    private List<SavedLootItem> persistentInventory = new List<SavedLootItem>();
    
    private void Awake()
    {
        Debug.Log("DataManager Awake called");
        
        // Singleton pattern with error checking
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("DataManager instance created and set to DontDestroyOnLoad");
        }
        else if (Instance != this)
        {
            Debug.Log("Destroying duplicate DataManager");
            Destroy(gameObject);
        }
    }
    
    private void OnEnable()
    {
        Debug.Log("DataManager OnEnable - Subscribing to events");
        
        // Listen for scene load events to restore inventory
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Listen for loot pickup events to store items
        Loot.OnLootPickup += StoreLootItem;
    }
    
    private void OnDisable()
    {
        Debug.Log("DataManager OnDisable - Unsubscribing from events");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Loot.OnLootPickup -= StoreLootItem;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name} - Delaying inventory restoration");
        // Give a slight longer delay to ensure LootManager is initialized
        Invoke("RestoreInventory", 0.5f);
    }
    
    // Store loot when picked up
    private void StoreLootItem(LootSO lootItem, int quantity)
    {
        if (lootItem == null)
        {
            Debug.LogError("Attempted to store null loot item");
            return;
        }
        
        bool itemFound = false;
        
        // Check if we already have this item type
        foreach (var item in persistentInventory)
        {
            if (item.lootItem == lootItem)
            {
                // Add to existing quantity
                item.quantity += quantity;
                itemFound = true;
                Debug.Log($"Updated existing item: {lootItem.name}, new quantity: {item.quantity}");
                break;
            }
        }
        
        // If not found, add as new item
        if (!itemFound)
        {
            persistentInventory.Add(new SavedLootItem(lootItem, quantity));
            Debug.Log($"Added new item to persistent inventory: {lootItem.name}, quantity: {quantity}");
        }
    }
    
    // Restore inventory after scene change
    public void RestoreInventory()
    {
        if (LootManager.Instance == null)
        {
            Debug.LogWarning("LootManager not found when trying to restore inventory - retrying in 0.5 seconds");
            Invoke("RestoreInventory", 0.5f);
            return;
        }
        
        Debug.Log($"Restoring {persistentInventory.Count} items from persistent inventory");
        
        // Clear all slots first
        foreach (var slot in LootManager.Instance.lootSlots)
        {
            if (slot != null)
            {
                slot.lootSO = null;
                slot.quantity = 0;
            }
        }
        
        int itemsRestored = 0;
        
        // Then add all items back
        foreach (var item in persistentInventory)
        {
            if (item.lootItem != null)
            {
                Debug.Log($"Restoring item: {item.lootItem.name}, quantity: {item.quantity}");
                LootManager.Instance.AddLoot(item.lootItem, item.quantity);
                itemsRestored++;
            }
            else
            {
                Debug.LogError("Null loot item found in persistent inventory");
            }
        }
        
        Debug.Log($"Inventory restoration complete - {itemsRestored}/{persistentInventory.Count} items restored");
    }
    
    // Remove consumed items from persistent storage
    public void RemoveUsedItem(LootSO lootItem, int quantity = 1)
    {
        if (lootItem == null)
        {
            Debug.LogError("Attempted to remove null loot item");
            return;
        }
        
        Debug.Log($"Removing {quantity}x {lootItem.name} from persistent inventory");
        
        for (int i = 0; i < persistentInventory.Count; i++)
        {
            if (persistentInventory[i].lootItem == lootItem)
            {
                persistentInventory[i].quantity -= quantity;
                Debug.Log($"Updated quantity: {persistentInventory[i].quantity}");
                
                // Remove item completely if quantity is 0
                if (persistentInventory[i].quantity <= 0)
                {
                    Debug.Log($"Removing item completely: {lootItem.name}");
                    persistentInventory.RemoveAt(i);
                }
                
                break;
            }
        }
    }
    
    // Call this when player dies or quits
    public void ResetAllPlayerData()
    {
        Debug.Log("Resetting all player data");
        isResettingStats = true;
        
        // Reset StatsManager
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.currentHealth = StatsManager.Instance.maxHealth;
            StatsManager.Instance.moveSpeed = 1f; // Reset to default speed
            StatsManager.Instance.staminaRefreshRate = 3; // Reset to default refresh rate
        }
        
        // Reset Stamina
        if (Stamina.Instance != null)
        {
            Stamina.Instance.ReplenshStaminaOnDeath();
        }
        
        
        isResettingStats = false;
        Debug.Log("Player data reset complete");
    }
    
    // Helper method to check if currently resetting
    public bool IsResettingStats()
    {
        return isResettingStats;
    }
    
    // For debugging purposes
    public void DebugPrintInventory()
    {
        Debug.Log($"==== Persistent Inventory ({persistentInventory.Count} items) ====");
        foreach (var item in persistentInventory)
        {
            if (item.lootItem != null)
                Debug.Log($"Item: {item.lootItem.name}, Quantity: {item.quantity}");
            else
                Debug.Log("Null item entry in persistent inventory");
        }
        Debug.Log("=======================================");
    }
    
    private void Start()
    {
        // Perform an initial inventory check in case any items were picked up before
        // the first scene change
        Debug.Log("DataManager Start: Checking for initial inventory items");
        DebugPrintInventory();
    }
    
    // Public struct to share inventory data
    public struct LootItemInfo
    {
        public LootSO lootItem;
        public int quantity;
        
        public LootItemInfo(LootSO item, int qty)
        {
            lootItem = item;
            quantity = qty;
        }
    }
    
    public List<LootItemInfo> GetPersistentInventoryCopy()
    {
        List<LootItemInfo> result = new List<LootItemInfo>();
        
        foreach (var item in persistentInventory)
        {
            if (item.lootItem != null)
            {
                result.Add(new LootItemInfo(item.lootItem, item.quantity));
            }
        }
        
        Debug.Log($"Provided copy of persistent inventory with {result.Count} items");
        return result;
    }
}