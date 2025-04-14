using UnityEngine;
using UnityEngine.UI;

public class MaskHealthManager : MonoBehaviour
{
    [Header("Mask Images")]
    [Tooltip("Mask for 3 health points (full mask)")]
    [SerializeField] private Sprite fullMask;
    
    [Tooltip("Mask for 2 health points (one chip off)")]
    [SerializeField] private Sprite damagedMask;
    
    [Tooltip("Mask for 1 health point (heavily damaged)")]
    [SerializeField] private Sprite criticalMask;
    
    [Header("Settings")]
    [Tooltip("The Image component that will display the mask")]
    [SerializeField] private Image maskImage;
    
    private void Awake()
    {
        // Check if StatsManager exists
        if (StatsManager.Instance == null)
        {
            Debug.LogError("StatsManager not found! Make sure it's initialized before this script.");
        }
    }

    private void Start()
    {
        // Check if the maskImage is not assigned in inspector
        if (maskImage == null)
        {
            // Try to get it from this GameObject
            maskImage = GetComponent<Image>();
            
            // If still null, show error
            if (maskImage == null)
            {
                Debug.LogError("No Image component found for the mask. Please assign it in the inspector.");
                return;
            }
        }

        // Make sure the maskImage is enabled
        maskImage.enabled = true;
        
        // Initialize mask based on starting health
        UpdateMaskDisplay();
        
        Debug.Log("MaskHealthManager initialized with health: " + StatsManager.Instance.currentHealth);
        Debug.Log("Sprites assigned: " + (fullMask != null) + ", " + (damagedMask != null) + ", " + (criticalMask != null));
    }

    public void Update()
    {
        // Update the mask display based on current health
        UpdateMaskDisplay();
    }

    // Updates the mask image based on current health
    private void UpdateMaskDisplay()
    {
        if (maskImage == null || StatsManager.Instance == null) return;

        // Ensure maskImage is enabled
        if (!maskImage.enabled && StatsManager.Instance.currentHealth > 0)
        {
            maskImage.enabled = true;
        }
        
        // Debug the current health
        // Debug.Log("Current health: " + StatsManager.Instance.currentHealth);
        
        switch (StatsManager.Instance.currentHealth)
        {
            case 3:
                if (fullMask != null)
                {
                    maskImage.sprite = fullMask;
                    maskImage.color = Color.white; // Ensure full opacity
                }
                break;
            case 2:
                if (damagedMask != null)
                {
                    maskImage.sprite = damagedMask;
                    maskImage.color = Color.white;
                }
                break;
            case 1:
                if (criticalMask != null)
                {
                    maskImage.sprite = criticalMask;
                    maskImage.color = Color.white;
                }
                break;
            case 0:
                // Optional: Handle death or make mask invisible
                maskImage.enabled = false;
                break;
            default:
                if (fullMask != null)
                {
                    maskImage.sprite = fullMask;
                    maskImage.color = Color.white;
                }
                break;
        }
    }
}