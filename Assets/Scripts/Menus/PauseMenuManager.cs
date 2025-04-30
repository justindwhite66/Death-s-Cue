using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class PauseMenuManager : MonoBehaviour
{
    // References to all menu panels
    [Header("Menu Panels")]
    [SerializeField] private GameObject pauseCanvas;      // Main canvas
    [SerializeField] private GameObject mainMenuPanel;    // Main panel
    [SerializeField] private GameObject lootPanel;        // Inventory
    [SerializeField] private GameObject statsPanel;       // Stats
    [SerializeField] private GameObject settingsPanel;    // Settings

    // References to all buttons
    [Header("Buttons")]
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    // References to back buttons
    [Header("Back Buttons")]
    [SerializeField] private Button lootBackButton;
    [SerializeField] private Button statsBackButton;
    [SerializeField] private Button settingsBackButton;

    // References to Settings
    [Header("Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    //[SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

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

    private void InitializeSettingsValues()
    {
        // Initialize settings values
        if (musicVolumeSlider != null)
        {
            // Set initial value from saved data
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        /*if (sfxVolumeSlider != null)
        {
            // Set initial value from saved data
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }*/

        if (fullscreenToggle != null)
        {
            // Set initial value from saved data
            fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    // Handles music volume changes
    private void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);

        // Apply volume change immediately
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }
    }

    // Handles SFX volume changes
    /*private void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);

        // Apply volume change immediately
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.SetSFXVolume(volume);
        }
    }*/

    // Handles fullscreen toggle
    private void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);

        // Apply fullscreen change immediately
        Screen.fullScreen = isFullscreen;
    }

    private bool IsAnySubPanelActive()
    {
        return (lootPanel != null && lootPanel.activeSelf) ||
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

        if (statsPanel) statsPanel.SetActive(false);
        if (statsBackButton) statsBackButton.gameObject.SetActive(false);

        if (settingsPanel) settingsPanel.SetActive(false);
        if (settingsBackButton) settingsBackButton.gameObject.SetActive(false);
    }

    void SetupButtonListeners()
    {
        // Open inventory panel - change to use OpenLootPanel instead of directly using OpenSubMenu
        if (inventoryButton != null)
        {
            inventoryButton.onClick.AddListener(OpenLootPanel);
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

    // Method that opens your loot panel
    public void OpenLootPanel()
    {
        Debug.Log("Opening loot panel");
        
        // First make the panel active so components are accessible
        lootPanel.SetActive(true);
        
        // Show the panel via regular method
        OpenSubMenu(lootPanel, lootBackButton);
        
        // Then call the new method to update inventory data specifically after panel is open
        if (LootManager.Instance != null)
        {
            LootManager.Instance.OnLootPanelOpened();
        }
    }

    // Method that opens your loot panel
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
            // Ensure LootManager updates all UI elements
            if (LootManager.Instance != null)
            {
                Debug.Log("Opening loot panel - updating all loot UI");
                LootManager.Instance.UpdateAllLootUI();
            }
            else
            {
                Debug.LogWarning("LootManager.Instance is null when opening loot panel");
            }
        }

        if (panel == settingsPanel)
        {
            // Initialize settings values
            InitializeSettingsValues();
        }
    }

    void BackToMainMenu()
    {
        // Hide all submenus
        if (lootPanel) lootPanel.SetActive(false);
        if (lootBackButton) lootBackButton.gameObject.SetActive(false);

        if (statsPanel) statsPanel.SetActive(false);
        if (statsBackButton) statsBackButton.gameObject.SetActive(false);

        if (settingsPanel) settingsPanel.SetActive(false);
        if (settingsBackButton) settingsBackButton.gameObject.SetActive(false);
        
        // Show main menu
        mainMenuPanel.SetActive(true);
        currentActivePanel = mainMenuPanel;

        if (currentActivePanel == settingsPanel)
        {
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        // Save settings to PlayerPrefs
        PlayerPrefs.Save();
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
        
        // Destroy all DontDestroyOnLoad objects before loading title
        DestroyAllPersistentObjects();
        
        // Load title screen
        SceneManager.LoadScene(titleSceneName);
    }

    private void DestroyAllPersistentObjects()
    {
        // Find and destroy player if it exists
        if (PlayerController.Instance != null)
        {
            Destroy(PlayerController.Instance.gameObject);
        }
        
        // Find and destroy other key singleton instances
        if (StatsManager.Instance != null)
        {
            Destroy(StatsManager.Instance.gameObject);
        }
        
        if (Stamina.Instance != null)
        {
            Destroy(Stamina.Instance.gameObject);
        }
        
        if (ActiveWeapon.Instance != null)
        {
            Destroy(ActiveWeapon.Instance.gameObject);
        }
        
        if (PlayerHealth.Instance != null)
        {
            Destroy(PlayerHealth.Instance.gameObject);
        }
        
        if (LootManager.Instance != null)
        {
            Destroy(LootManager.Instance.gameObject);
        }
        
        // Additionally destroy any music-related objects
        if (AudioManager.Instance != null)
        {
            Destroy(AudioManager.Instance.gameObject);
        }
        
        // Find all active AudioSources and stop them
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }
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