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
    [SerializeField] private GameObject pauseCanvas;      // Main canvas
    [SerializeField] private GameObject mainMenuPanel;    // Main panel
    [SerializeField] private GameObject lootPanel;        // Inventory
    [SerializeField] private GameObject equipmentPanel;   // Equipment
    [SerializeField] private GameObject statsPanel;       // Stats
    [SerializeField] private GameObject settingsPanel;    // Settings

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
    [SerializeField] private string titleSceneName = "Title";

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
        playerControls.UI.Pause.performed += ctx => 
        {
            // Don't allow [Tab] to toggle menu if in sub-panel
            if (IsAnySubPanelActive())
            {
                // Ignore pause input when in sub-panel
                return;
            }
            
            TogglePauseMenu();
        };
    }

    private bool IsAnySubPanelActive()
    {
        return (lootPanel != null && lootPanel.activeSelf) ||
               (equipmentPanel != null && equipmentPanel.activeSelf) ||
               (statsPanel != null && statsPanel.activeSelf) ||
               (settingsPanel != null && settingsPanel.activeSelf);
    }

    void OnEnable()
    {
        playerControls.UI.Enable();
        
        // Make sure playerControls exists and initialized
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.Pause.performed += ctx => 
            {
                if (IsAnySubPanelActive())
                {
                    return;
                }
                
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
        
        // Set canvas to ignore timescale
        Canvas canvas = pauseCanvas.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10; // High value to ensure renders on top
        }
        
        // Configure GraphicRaycaster to ignore timescale
        GraphicRaycaster raycaster = pauseCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            raycaster.ignoreReversedGraphics = false;
        }
        
        // Component to handle input when timescale 0
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

        // Initialize pause menu
        isPaused = false;

        // Ensure pauseCanvas always enabled
        pauseCanvas.SetActive(true);
        
        // Hide all panels except pauseCanvas
        HideAllPanels();
        
        // Set main menu as default (but don't show yet)
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
        // Open inventory panel
        if (inventoryButton != null)
        {
            inventoryButton.onClick.AddListener(() => 
            {
                OpenSubMenu(lootPanel, lootBackButton);
            });
        }
            
        // Open equipment panel
        if (equipmentButton != null)
        {
            equipmentButton.onClick.AddListener(() => 
            {
                OpenSubMenu(equipmentPanel, equipmentBackButton);
            });
        }
            
        // Open stats panel
        if (statsButton != null)
        {
            statsButton.onClick.AddListener(() => 
            {
                OpenSubMenu(statsPanel, statsBackButton);
            });
        }
            
        // Open settings panel
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(() => 
            {
                OpenSubMenu(settingsPanel, settingsBackButton);
            });
        }
            
        // Handle quit button
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(ReturnToTitleScreen);
        }
        
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
            
            // Make cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Unpause game
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
            return;
        }
        
        // Hide main menu panel
        mainMenuPanel.SetActive(false);
        
        // Show requested panel
        panel.SetActive(true);
        if (backButton != null) backButton.gameObject.SetActive(true);
        currentActivePanel = panel;

        // If opening loot panel, update UI
        if (panel == lootPanel)
        {
            // Try to use singleton first
            if (LootManager.Instance != null)
            {
                LootManager.Instance.UpdateAllLootUI();
            }
            else
            {
                // Fallback to FindObjectOfType
                LootManager lootManager = FindObjectOfType<LootManager>();
                if (lootManager != null)
                {
                    lootManager.UpdateAllLootUI();
                }
            }
            
            // Additional debug output for LootSlots
            LootSlots[] slots = lootPanel.GetComponentsInChildren<LootSlots>();
        }
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

        // Reset all player data
        if (DataManager.Instance != null)
        {
            DataManager.Instance.ResetAllPlayerData();
        }
        
        // Load title screen
        SceneManager.LoadScene(titleSceneName);
    }

    public bool IsLootPanelActive()
    {
        return lootPanel != null && lootPanel.activeSelf;
    }

    // Check if any menu active
    public bool IsAnyMenuActive()
    {
        return isPaused || IsAnySubPanelActive();
    }
}