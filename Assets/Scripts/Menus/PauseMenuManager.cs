using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    // References to all menu panels
    [Header("Menu Panels")]
    [SerializeField] private GameObject pauseCanvas;        // Main canvas
    [SerializeField] private GameObject mainMenuPanel;      // Main panel
    [SerializeField] private GameObject lootPanel;          // Inventory
    [SerializeField] private GameObject equipmentPanel;     // Equipment
    [SerializeField] private GameObject statsPanel;         // Stats
    [SerializeField] private GameObject settingsPanel;      // Settings

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
    private bool isPaused = false;
    private GameObject currentActivePanel;
    private float previousTimeScale;

    // Input System
    private PlayerControls playerControls;

    void Awake()
    {
        // Initialize input system
        playerControls = new PlayerControls();
        
        // Register callback for pause action
        playerControls.UI.Pause.performed += ctx => {
            TogglePauseMenu();
        };
    }

    void OnEnable()
    {
        // Enable the UI action map
        playerControls.UI.Enable();
        
        // Make sure playerControls exists and is properly initialized
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.Pause.performed += ctx => {
                TogglePauseMenu();
            };
            playerControls.UI.Enable();
        }
    }

    void OnDisable()
    {
        // Disable the UI action map
        if (playerControls != null)
        {
            playerControls.UI.Disable();
        }
    }

    void Start()
    {
        // Check all key references
        if (pauseCanvas == null)
        {
            return;
        }
        
        // Set the canvas to ignore timescale
        Canvas canvas = pauseCanvas.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10; // High value to ensure it renders on top
        }
        
        // Configure the GraphicRaycaster to ignore timescale
        GraphicRaycaster raycaster = pauseCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            raycaster.ignoreReversedGraphics = false;
        }
        
        // Add this component to handle input when timescale is 0
        if (!pauseCanvas.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup = pauseCanvas.AddComponent<CanvasGroup>();
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        if (mainMenuPanel == null)
        {
            return;
        }

        // Initialize the pause menu
        isPaused = false;

        // Ensure pauseCanvas is always enabled
        pauseCanvas.SetActive(true);
        
        // Hide all panels except pauseCanvas
        HideAllPanels();
        
        // Set main menu as default (but don't show it yet)
        currentActivePanel = mainMenuPanel;
        
        // Add listeners to all buttons
        SetupButtonListeners();
        
        // Make sure input is enabled
        if (playerControls != null && !playerControls.UI.enabled)
        {
            playerControls.UI.Enable();
        }
    }

    // Hide all panels but keep pauseCanvas active
    private void HideAllPanels()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        
        if (lootPanel) lootPanel.SetActive(false);
        if (lootBackButton) lootBackButton.gameObject.SetActive(false);

        if (equipmentPanel) equipmentPanel.SetActive(false);
        if (equipmentBackButton) equipmentBackButton.gameObject.SetActive(false);

        if (statsPanel) statsPanel.SetActive(false);
        if (statsBackButton) statsBackButton.gameObject.SetActive(false);

        if (settingsPanel) settingsPanel.SetActive(false);
        if (settingsBackButton) settingsBackButton.gameObject.SetActive(false);
    }

    void SetupButtonListeners()
    {
        if (inventoryButton != null)
            inventoryButton.onClick.AddListener(
                () => OpenSubMenu(lootPanel, lootBackButton)
            );
        else
            Debug.LogError("Inventory button is not assigned in inspector!");
            
        if (equipmentButton != null)
            equipmentButton.onClick.AddListener(
                () => OpenSubMenu(equipmentPanel, equipmentBackButton)
            );
        else
            Debug.LogError("Equipment button is not assigned in inspector!");
            
        if (statsButton != null)
            statsButton.onClick.AddListener(
                () => OpenSubMenu(statsPanel, statsBackButton)
            );
        else
            Debug.LogError("Stats button is not assigned in inspector!");
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(
                () => OpenSubMenu(settingsPanel, settingsBackButton)
            );
        else
            Debug.LogError("Settings button is not assigned in inspector!");
            
        if (quitButton != null)
            quitButton.onClick.AddListener(ReturnToTitleScreen);
        else
            Debug.LogError("Quit button is not assigned in inspector!");
        
        // Back buttons
        if (lootBackButton) 
            lootBackButton.onClick.AddListener(BackToMainMenu);
        if (equipmentBackButton) 
            equipmentBackButton.onClick.AddListener(BackToMainMenu);
        if (statsBackButton) 
            statsBackButton.onClick.AddListener(BackToMainMenu);
        if (settingsBackButton) 
            settingsBackButton.onClick.AddListener(BackToMainMenu);
    }

    public void TogglePauseMenu()
    {   
        // Toggle pause state based on main menu panel visibility
        bool mainMenuActive = mainMenuPanel != null && mainMenuPanel.activeSelf;
        isPaused = !mainMenuActive;
        
        if (isPaused)
        {
            // Store current time scale
            previousTimeScale = Time.timeScale;
            
            // Pause the game
            Time.timeScale = 0f;
            
            // Show main menu panel
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                currentActivePanel = mainMenuPanel;
            }
            else
            {
                Debug.LogError("mainMenuPanel is null!");
            }
            
            // Make cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Unpause the game
            Time.timeScale = previousTimeScale;
            
            // Hide all panels but keep pauseCanvas active
            HideAllPanels();
        }
    }

    void OpenSubMenu(GameObject panel, Button backButton)
    {
        // Additional check for null references
        if (panel == null)
        {
            Debug.LogError("Trying to open a null panel!");
            return;
        }
        
        // Hide main menu panel
        mainMenuPanel.SetActive(false);
        
        // Show the requested panel
        panel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
        currentActivePanel = panel;
    }

    void BackToMainMenu()
    {
        // Hide all submenus
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
    }

    void ReturnToTitleScreen()
    {
        // Resume normal time before loading
        Time.timeScale = 1f;
        
        // Load the title screen
        SceneManager.LoadScene(titleSceneName);
    }
}