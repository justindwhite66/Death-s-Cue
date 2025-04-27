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

    private void Start()
    {
        // Check if maskImage not assigned in inspector
        if (maskImage == null)
        {
            // Try to get it from this GameObject
            maskImage = GetComponent<Image>();
            
            // If still null, show error
            if (maskImage == null)
            {
                return;
            }
        }

        // Make sure maskImage enabled
        maskImage.enabled = true;
        
        // Initialize mask based on starting health
        UpdateMaskDisplay();        
    }

    public void Update()
    {
        UpdateMaskDisplay();
    }

    // Updates mask image based on current health
    private void UpdateMaskDisplay()
    {
        if (maskImage == null || StatsManager.Instance == null) return;

        // Ensure maskImage enabled
        if (!maskImage.enabled && StatsManager.Instance.currentHealth > 0)
        {
            maskImage.enabled = true;
        }
                
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
                // Handle death or make mask invisible
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