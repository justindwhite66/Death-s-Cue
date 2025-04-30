using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HotbarManager : Singleton<HotbarManager>
{
    public HotbarSlot[] hotbarSlots = new HotbarSlot[3];
    
    private PlayerControls playerControls;
    private bool initialized = false;
    
    protected override void Awake()
    {
        // Call base.Awake() which will set up the Instance property correctly
        base.Awake();
        
        // No need to set Instance again, just check if this is the surviving instance
        if (this == Instance)
        {
            Debug.Log("HotbarManager instance created and set to DontDestroyOnLoad");
            InitializeControls();
        }
    }
    
    private void InitializeControls()
    {
        try {
            // Create new input system controls
            playerControls = new PlayerControls();
            
            // Register callbacks for hotbar keys (Z, X, C for assignment)
            playerControls.Hotbar.HotbarZ.performed += ctx => AssignToHotbar(0);
            playerControls.Hotbar.HotbarX.performed += ctx => AssignToHotbar(1);
            playerControls.Hotbar.HotbarC.performed += ctx => AssignToHotbar(2);
            
            // Register callbacks for using hotbar items (Z, X, C for use)
            playerControls.Hotbar.UseHotbarZ.performed += ctx => UseHotbarItem(0);
            playerControls.Hotbar.UseHotbarX.performed += ctx => UseHotbarItem(1);
            playerControls.Hotbar.UseHotbarC.performed += ctx => UseHotbarItem(2);
            
            initialized = true;
            Debug.Log("HotbarManager controls initialized");
        } 
        catch (System.Exception e) {
            Debug.LogError($"Error initializing HotbarManager controls: {e.Message}");
        }
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
        
        // Subscribe to scene loaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnEnable()
    {
        if (initialized && playerControls != null)
        {
            playerControls.Hotbar.Enable();
        }
        
        try {
            LootManager.OnInventoryChanged += RefreshAllHotbarSlots;
        } catch (System.Exception e) {
            Debug.LogError($"Error subscribing to OnInventoryChanged: {e.Message}");
        }
    }
    
    private void OnDisable()
    {
        if (initialized && playerControls != null)
        {
            playerControls.Hotbar.Disable();
        }
        
        try {
            LootManager.OnInventoryChanged -= RefreshAllHotbarSlots;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        } catch (System.Exception e) {
            Debug.LogError($"Error unsubscribing from events: {e.Message}");
        }
    }
    
    private void OnDestroy()
    {
        if (initialized && playerControls != null)
        {
            try {
                playerControls.Dispose();
            } catch (System.Exception e) {
                Debug.LogError($"Error disposing playerControls: {e.Message}");
            }
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"HotbarManager: Scene loaded: {scene.name}");
        StartCoroutine(RefreshAfterDelay(0.5f));
    }
    
    private IEnumerator RefreshAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RefreshAllHotbarSlots();
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
            // Check for valid index
            if (slotIndex >= 0 && slotIndex < hotbarSlots.Length && hotbarSlots[slotIndex] != null)
            {
                // Assign to hotbar slot
                hotbarSlots[slotIndex].AssignItem(selectedSlot.lootSO);
                
                // Get key name for logging
                string keyName = (slotIndex == 0) ? "Z" : (slotIndex == 1) ? "X" : "C";
                Debug.Log($"Assigned {selectedSlot.lootSO.name} to hotbar slot {keyName}");
            }
        }
    }
    
    // Use item from hotbar
    private void UseHotbarItem(int slotIndex)
    {
        // Check for valid index and slot
        if (slotIndex >= 0 && slotIndex < hotbarSlots.Length && hotbarSlots[slotIndex] != null)
        {
            // Check if slot has item and is not in menu
            if (hotbarSlots[slotIndex].HasItem() && !IsPauseMenuActive())
            {
                string keyName = (slotIndex == 0) ? "Z" : (slotIndex == 1) ? "X" : "C";
                Debug.Log($"Using item from hotbar slot {keyName}");
                hotbarSlots[slotIndex].UseItem();
            }
        }
    }
    
    // Check if in menu
    private bool IsPauseMenuActive()
    {
        PauseMenuManager pauseMenu = FindObjectOfType<PauseMenuManager>();
        return pauseMenu != null && pauseMenu.IsAnyMenuActive();
    }
    
    // Refresh all hotbar slots from inventory - public so DataManager can call it
    public void RefreshAllHotbarSlots()
    {
        Debug.Log("Refreshing all hotbar slots");
        foreach (var slot in hotbarSlots)
        {
            if (slot != null)
            {
                slot.RefreshQuantityFromInventory();
            }
        }
    }
}