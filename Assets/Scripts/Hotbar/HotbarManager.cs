using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : Singleton<HotbarManager>
{
    public static HotbarManager Instance { get; private set; }

    // Array for slots Z, X, C
    public HotbarSlot[] hotbarSlots = new HotbarSlot[3];
    
    private PlayerControls playerControls;
    
    protected override void Awake()
    {
        base.Awake();
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
        
        playerControls = new PlayerControls();
        
        // Register callbacks for hotbar keys (Z, X, C for assignment)
        playerControls.Hotbar.HotbarZ.performed += ctx => AssignToHotbar(0);
        playerControls.Hotbar.HotbarX.performed += ctx => AssignToHotbar(1);
        playerControls.Hotbar.HotbarC.performed += ctx => AssignToHotbar(2);
        
        // Register callbacks for using hotbar items (Z, X, C for use)
        playerControls.Hotbar.UseHotbarZ.performed += ctx => UseHotbarItem(0);
        playerControls.Hotbar.UseHotbarX.performed += ctx => UseHotbarItem(1);
        playerControls.Hotbar.UseHotbarC.performed += ctx => UseHotbarItem(2);
    }

    private void Start()
    {
        // Initialize key text for each hotbar slot
        if (hotbarSlots.Length >= 3)
        {
            if (hotbarSlots[0] != null) hotbarSlots[0].SetKeyText("Z");
            if (hotbarSlots[1] != null) hotbarSlots[1].SetKeyText("X");
            if (hotbarSlots[2] != null) hotbarSlots[2].SetKeyText("C");
        }
    }
    
    private void OnEnable()
    {
        playerControls.Hotbar.Enable();
        
        // Subscribe to inventory changes
        LootManager.OnInventoryChanged += RefreshAllHotbarSlots;
    }
    
    private void OnDisable()
    {
        playerControls.Hotbar.Disable();
        
        // Unsubscribe from inventory changes
        LootManager.OnInventoryChanged -= RefreshAllHotbarSlots;
    }
    
    // Assign currently selected inventory item to hotbar slot
    private void AssignToHotbar(int slotIndex)
    {
        // Check if the loot panel is active (only assign when inventory open)
        PauseMenuManager pauseMenu = FindObjectOfType<PauseMenuManager>();
        if (pauseMenu == null || !pauseMenu.IsLootPanelActive())
        {
            return;
        }
        
        // Check if we have a selected slot with an item
        var selectedSlot = LootSlots.SelectedSlot;
        if (selectedSlot != null && selectedSlot.lootSO != null)
        {
            // Assign to hotbar slot
            hotbarSlots[slotIndex].AssignItem(selectedSlot.lootSO);
            
            // Get key name for logging
            string keyName;
            if (slotIndex == 0) keyName = "Z";
            else if (slotIndex == 1) keyName = "X";
            else keyName = "C";
        }
    }
    
    // Use item from hotbar
    private void UseHotbarItem(int slotIndex)
    {
        // Check if slot has item and is not in menu
        if (hotbarSlots[slotIndex].HasItem() && !IsPauseMenuActive())
        {
            hotbarSlots[slotIndex].UseItem();
        }
    }
    
    // Check if in menu
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