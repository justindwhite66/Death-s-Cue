using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // For new Input System

public class PauseMenuManager : MonoBehaviour
{
    // References to all menu panels
    [Header("Menu Panels")]
    [SerializeField] private GameObject pauseCanvas;        // Your main canvas
    [SerializeField] private GameObject mainMenuPanel;      // Panel with all buttons
    [SerializeField] private GameObject lootPanel;          // Inventory panel
    [SerializeField] private GameObject equipmentPanel;     // Equipment panel
    [SerializeField] private GameObject statsPanel;         // Stats panel
    [SerializeField] private GameObject settingsPanel;      // Settings panel

    // References to all buttons
    [Header("Buttons")]
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    // References to back buttons
    [Header("Back Buttons")]
    [SerializeField] private Button lootBackButton;
    [SerializeField] private Button equipmentBackButton;
    [SerializeField] private Button statsBackButton;
    [SerializeField] private Button settingsBackButton;

    // Scene to load when quitting
    [SerializeField] private string titleSceneName = "Shop";

    // Tracking variables
    private bool isPaused;
    private GameObject currentActivePanel;
    private float previousTimeScale;

    // Input System
    private PlayerControls playerControls;

    // Add traditional input support as a fallback
    [SerializeField] private KeyCode alternativePauseKey = KeyCode.P;

    void Awake()
    {
        // Initialize input system
        playerControls = new PlayerControls();
        
        // Register callback for pause action with more detailed logging
        playerControls.UI.Pause.performed += ctx => {
            Debug.Log("Pause key pressed via Input System - context: " + ctx);
            TogglePauseMenu();
        };
    }

    void OnEnable()
    {
        // Enable both the UI action map and Player action map if applicable
        playerControls.UI.Enable();
        Debug.Log("PauseMenuManager UI controls enabled");
    }

    void OnDisable()
    {
        playerControls.UI.Disable();
    }

    void Start()
    {
        // Hide all panels at start
        pauseCanvas.SetActive(false);
        
        // Hide all panels and back buttons
        if (lootPanel) lootPanel.SetActive(false);
        if (lootBackButton) lootBackButton.gameObject.SetActive(false);

        if (equipmentPanel) equipmentPanel.SetActive(false);
        if (equipmentBackButton) equipmentBackButton.gameObject.SetActive(false);

        if (statsPanel) statsPanel.SetActive(false);
        if (statsBackButton) statsBackButton.gameObject.SetActive(false);

        if (settingsPanel) settingsPanel.SetActive(false);
        if (settingsBackButton) settingsBackButton.gameObject.SetActive(false);
        
        // Set main menu as default
        currentActivePanel = mainMenuPanel;
        
        // Add listeners to all buttons
        SetupButtonListeners();
        
        // Additional debug
        Debug.Log("PauseMenuManager started. Pause key should be working now.");
    }

    void SetupButtonListeners()
    {
        // FIXED: Ensure buttons match their correct panels
        if (inventoryButton != null)
            inventoryButton.onClick.AddListener(() => OpenSubMenu(lootPanel, lootBackButton));
        else
            Debug.LogError("Inventory button is not assigned in the inspector!");
            
        if (equipmentButton != null)
            equipmentButton.onClick.AddListener(() => OpenSubMenu(equipmentPanel, equipmentBackButton));
        else
            Debug.LogError("Equipment button is not assigned in the inspector!");
            
        if (statsButton != null)
            statsButton.onClick.AddListener(() => OpenSubMenu(statsPanel, statsBackButton));
        else
            Debug.LogError("Stats button is not assigned in the inspector!");
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(() => OpenSubMenu(settingsPanel, settingsBackButton));
        else
            Debug.LogError("Settings button is not assigned in the inspector!");
            
        if (quitButton != null)
            quitButton.onClick.AddListener(ReturnToTitleScreen);
        else
            Debug.LogError("Quit button is not assigned in the inspector!");
        
        // Back buttons
        if (lootBackButton) lootBackButton.onClick.AddListener(BackToMainMenu);
        if (equipmentBackButton) equipmentBackButton.onClick.AddListener(BackToMainMenu);
        if (statsBackButton) statsBackButton.onClick.AddListener(BackToMainMenu);
        if (settingsBackButton) settingsBackButton.onClick.AddListener(BackToMainMenu);
        
        Debug.Log("All button listeners set up correctly");
    }
    
    // Add traditional input checking as fallback
    void Update()
    {
        // Check for traditional input as a fallback
        if (Input.GetKeyDown(alternativePauseKey))
        {
            Debug.Log("Pause key pressed via traditional input system");
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        Debug.Log("TogglePauseMenu called. Current state: " + isPaused);
        
        // Toggle the pause state first
        isPaused = !isPaused;
        Debug.Log("Pause state toggled to: " + isPaused);
        
        if (isPaused)
        {
            // Store current time scale
            previousTimeScale = Time.timeScale;
            
            // Pause the game
            Time.timeScale = 0f;
            
            // Show pause menu
            pauseCanvas.SetActive(true);
            Debug.Log("pauseCanvas activated: " + pauseCanvas.activeSelf);
            
            // Ensure start with main menu
            ShowOnlyPanel(mainMenuPanel);
            
            // Make cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Unpause the game
            Time.timeScale = previousTimeScale;
            
            // Hide all menus
            pauseCanvas.SetActive(false);
            if (lootPanel) lootPanel.SetActive(false);
            if (lootBackButton) lootBackButton.gameObject.SetActive(false);
            
            if (equipmentPanel) equipmentPanel.SetActive(false);
            if (equipmentBackButton) equipmentBackButton.gameObject.SetActive(false);
            
            if (statsPanel) statsPanel.SetActive(false);
            if (statsBackButton) statsBackButton.gameObject.SetActive(false);
            
            if (settingsPanel) settingsPanel.SetActive(false);
            if (settingsBackButton) settingsBackButton.gameObject.SetActive(false);

            Debug.Log("Pause menu closed. TimeScale restored to " + previousTimeScale);
        }
    }

    void OpenSubMenu(GameObject panel, Button backButton)
    {
        // Additional check for null references
        if (panel == null) {
            Debug.LogError("Trying to open a null panel!");
            return;
        }
        
        if (backButton == null) {
            Debug.LogWarning("Back button is null when opening submenu!");
            // Continue anyway as we might not need the back button
        }
        
        // Hide main menu panel
        mainMenuPanel.SetActive(false);
        
        // Show the requested panel
        panel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
        currentActivePanel = panel;
        
        Debug.Log("Opened submenu: " + panel.name);
    }

    void BackToMainMenu()
    {
        // Hide all submenus and their back buttons
        if (lootPanel) lootPanel.SetActive(false);
        if (lootBackButton) lootBackButton.gameObject.SetActive(false);

        if (equipmentPanel) equipmentPanel.SetActive(false);
        if (equipmentBackButton) equipmentBackButton.gameObject.SetActive(false);

        if (statsPanel) statsPanel.SetActive(false);
        if (statsBackButton) statsBackButton.gameObject.SetActive(false);

        if (settingsPanel) settingsPanel.SetActive(false);
        if (settingsBackButton) settingsBackButton.gameObject.SetActive(false);
        
        // Show main menu
        mainMenuPanel.SetActive(true);
        currentActivePanel = mainMenuPanel;
        
        Debug.Log("Returned to main menu");
    }

    void ShowOnlyPanel(GameObject panel)
    {
        // Additional check for null references
        if (panel == null) {
            Debug.LogError("Trying to show a null panel!");
            return;
        }
        
        // Hide all panels and their back buttons
        mainMenuPanel.SetActive(false);
        
        if (lootPanel && lootPanel != panel) {
            lootPanel.SetActive(false);
            if (lootBackButton) lootBackButton.gameObject.SetActive(false);
        }
        
        if (equipmentPanel && equipmentPanel != panel) {
            equipmentPanel.SetActive(false);
            if (equipmentBackButton) equipmentBackButton.gameObject.SetActive(false);
        }
        
        if (statsPanel && statsPanel != panel) {
            statsPanel.SetActive(false);
            if (statsBackButton) statsBackButton.gameObject.SetActive(false);
        }
        
        if (settingsPanel && settingsPanel != panel) {
            settingsPanel.SetActive(false);
            if (settingsBackButton) settingsBackButton.gameObject.SetActive(false);
        }
        
        // Show only the requested panel
        panel.SetActive(true);
        
        // Show the corresponding back button if needed
        if (panel == lootPanel && lootBackButton) lootBackButton.gameObject.SetActive(true);
        else if (panel == equipmentPanel && equipmentBackButton) equipmentBackButton.gameObject.SetActive(true);
        else if (panel == statsPanel && statsBackButton) statsBackButton.gameObject.SetActive(true);
        else if (panel == settingsPanel && settingsBackButton) settingsBackButton.gameObject.SetActive(true);
        
        currentActivePanel = panel;
        
        Debug.Log("Showing only panel: " + panel.name);
    }

    void ReturnToTitleScreen()
    {
        // Resume normal time before loading
        Time.timeScale = 1f;
        
        // Load the title screen
        SceneManager.LoadScene(titleSceneName);
    }
}