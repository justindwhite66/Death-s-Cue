using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class LootSlots : MonoBehaviour, IPointerClickHandler
{
    public LootSO lootSO;
    public int quantity;

    public Image lootIcon;
    public TMP_Text quantityText;
    
    // Add selection visual indicator
    public Image selectionOutline;

    // Track if this slot is selected
    private bool isSelected = false;
    
    // Static reference to currently selected slot
    public static LootSlots SelectedSlot { get; private set; }

    public static event System.Action<LootSO> OnSlotSelectionChanged;

    public void UpdateLootUI()
    {
        Debug.Log($"Updating UI for slot: {(lootSO != null ? lootSO.name : "empty")}");

        // More aggressive component finding logic
        FindUIComponents();
        
        // Update display based on slot content
        if (lootSO != null && quantity > 0)
        {
            // Make sure icon is correctly set
            if (lootIcon != null)
            {
                lootIcon.sprite = lootSO.lootIcon;
                lootIcon.gameObject.SetActive(true);
                Debug.Log($"Setting icon for {lootSO.name}");
            }
            
            if (quantityText != null)
            {
                quantityText.text = quantity.ToString();
                Debug.Log($"Setting quantity text: {quantity}");
            }
        }
        else
        {
            // Clear slot if empty
            if (lootIcon != null)
            {
                lootIcon.gameObject.SetActive(false);
            }
            
            if (quantityText != null)
            {
                quantityText.text = "";
            }
            
            // Make sure selection is cleared if item removed
            if (isSelected)
            {
                DeselectSlot();
            }
        }
    }
    
    // Handle click events
    public void OnPointerClick(PointerEventData eventData)
    {
        // Only allow selection if slot has item
        if (lootSO != null)
        {
            // If slot already selected, deselect
            if (isSelected)
            {
                DeselectSlot();
            }
            else
            {
                // Deselect any previously selected slot
                if (SelectedSlot != null && SelectedSlot != this)
                {
                    SelectedSlot.DeselectSlot();
                }
                
                // Select slot
                SelectSlot();
            }
        }
    }
    
    private void SelectSlot()
    {
        isSelected = true;
        SelectedSlot = this;
        
        // Show selection visual
        if (selectionOutline != null)
        {
            selectionOutline.gameObject.SetActive(true);
        }
        
        // Trigger event for Loot Description Panel update
        OnSlotSelectionChanged?.Invoke(lootSO);
    }
    
    private void DeselectSlot()
    {
        isSelected = false;
        
        // Clear static reference if this was selected slot
        if (SelectedSlot == this)
        {
            SelectedSlot = null;
        }
        
        // Hide selection visual
        if (selectionOutline != null)
        {
            selectionOutline.gameObject.SetActive(false);
        }

        // Trigger event for Loot Description Panel update
        OnSlotSelectionChanged?.Invoke(null);
    }

    public void FindUIComponents()
    {
        // Find lootIcon with multiple attempts
        if (lootIcon == null)
        {
            // Try direct child first
            lootIcon = GetComponentInChildren<Image>(true);
            
            // If that fails, try looking for an "Icon" named child
            if (lootIcon == null || lootIcon.gameObject == gameObject)
            {
                Transform iconTransform = transform.Find("Icon");
                if (iconTransform != null)
                    lootIcon = iconTransform.GetComponent<Image>();
            }
            
            // Final fallback - search all children for any image that's not on this object
            if (lootIcon == null || lootIcon.gameObject == gameObject)
            {
                Image[] images = GetComponentsInChildren<Image>(true);
                foreach (var img in images)
                {
                    if (img.gameObject != gameObject)
                    {
                        lootIcon = img;
                        break;
                    }
                }
            }
            
            if (lootIcon == null)
                Debug.LogError($"Could not find lootIcon for slot {name}");
        }
        
        // Find quantityText with multiple attempts
        if (quantityText == null)
        {
            // Try direct child first
            quantityText = GetComponentInChildren<TMP_Text>(true);
            
            // Try looking for a "Quantity" named child
            if (quantityText == null)
            {
                Transform quantityTransform = transform.Find("Quantity");
                if (quantityTransform != null)
                    quantityText = quantityTransform.GetComponent<TMP_Text>();
            }
            
            if (quantityText == null)
                Debug.LogError($"Could not find quantityText for slot {name}");
        }
        
        // Find selection outline with multiple attempts
        if (selectionOutline == null)
        {
            // Try direct child first with specific name
            Transform outlineTransform = transform.Find("SelectionOutline");
            if (outlineTransform != null)
                selectionOutline = outlineTransform.GetComponent<Image>();
                
            // Fallback to any child with "outline" in the name
            if (selectionOutline == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.name.ToLower().Contains("outline") || child.name.ToLower().Contains("selection"))
                    {
                        selectionOutline = child.GetComponent<Image>();
                        if (selectionOutline != null) break;
                    }
                }
            }
        }
    }
}