using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance { get; private set; }
    
    // References to hotbar slot UI elements - Z, X, C slots
    public HotbarSlot[] hotbarSlots = new HotbarSlot[3]; // Array for slots Z, X, C
    
    // Reference to input system
    private PlayerControls playerControls;
    
    private void Awake()
    {
        // Set up singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize input system
        playerControls = new PlayerControls();
        
        // Register callbacks for hotbar keys (Z, X, C for assignment)
        playerControls.Hotbar.HotbarZ.performed += ctx => AssignToHotbar(0); // Z key
        playerControls.Hotbar.HotbarX.performed += ctx => AssignToHotbar(1); // X key
        playerControls.Hotbar.HotbarC.performed += ctx => AssignToHotbar(2); // C key
        
        // Register callbacks for using hotbar items (Z, X, C for use)
        playerControls.Hotbar.UseHotbarZ.performed += ctx => UseHotbarItem(0); // Z key
        playerControls.Hotbar.UseHotbarX.performed += ctx => UseHotbarItem(1); // X key
        playerControls.Hotbar.UseHotbarC.performed += ctx => UseHotbarItem(2); // C key
    }

    private void Start()
    {
        // Initialize the key text for each hotbar slot
        if (hotbarSlots.Length >= 3)
        {
            if (hotbarSlots[0] != null) hotbarSlots[0].SetKeyText("Z");
            if (hotbarSlots[1] != null) hotbarSlots[1].SetKeyText("X");
            if (hotbarSlots[2] != null) hotbarSlots[2].SetKeyText("C");
        }
        else
        {
            Debug.LogError("HotbarManager: Not enough hotbar slots assigned in the inspector!");
        }
    }
    
    private void OnEnable()
    {
        // Enable input actions
        playerControls.Hotbar.Enable();
        
        // Subscribe to inventory changes
        LootManager.OnInventoryChanged += RefreshAllHotbarSlots;
    }
    
    private void OnDisable()
    {
        // Disable input actions
        playerControls.Hotbar.Disable();
        
        // Unsubscribe from inventory changes
        LootManager.OnInventoryChanged -= RefreshAllHotbarSlots;
    }
    
    // Assign currently selected inventory item to hotbar slot
    private void AssignToHotbar(int slotIndex)
    {
        // Check if the loot panel is active (only assign when inventory is open)
        PauseMenuManager pauseMenu = FindObjectOfType<PauseMenuManager>();
        if (pauseMenu == null || !pauseMenu.IsLootPanelActive())
        {
            return;
        }
        
        // Check if we have a selected slot with an item
        if (LootSlots.SelectedSlot != null && LootSlots.SelectedSlot.lootSO != null)
        {
            // Assign to hotbar slot
            hotbarSlots[slotIndex].AssignItem(LootSlots.SelectedSlot.lootSO);
            
            // Get key name for logging
            string keyName = slotIndex == 0 ? "Z" : (slotIndex == 1 ? "X" : "C");
            Debug.Log($"Assigned {LootSlots.SelectedSlot.lootSO.lootName} to hotbar slot {keyName}");
        }
    }
    
    // Use an item from the hotbar
    private void UseHotbarItem(int slotIndex)
    {
        // Check if the slot has an item and is not in menu
        if (hotbarSlots[slotIndex].HasItem() && !IsPauseMenuActive())
        {
            hotbarSlots[slotIndex].UseItem();
        }
    }
    
    // Check if we're in a menu
    private bool IsPauseMenuActive()
    {
        PauseMenuManager pauseMenu = FindObjectOfType<PauseMenuManager>();
        return pauseMenu != null && pauseMenu.IsAnyMenuActive();
    }
    
    // Refresh all hotbar slots from inventory
    public void RefreshAllHotbarSlots()
    {
        foreach (var slot in hotbarSlots)
        {
            if (slot != null)
            {
                slot.RefreshQuantityFromInventory();
            }
        }
    }
}