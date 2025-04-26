using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;  // Add this for UI interaction

// Add these interfaces to handle clicks
public class LootSlots : MonoBehaviour, IPointerClickHandler
{
    public LootSO lootSO;
    public int quantity;

    public Image lootIcon;
    public TMP_Text quantityText;
    
    // Add selection visual indicator
    public Image selectionOutline; // Assign this in inspector - add an Image child with outline sprite
    
    // Track if this slot is selected
    private bool isSelected = false;
    
    // Static reference to currently selected slot
    public static LootSlots SelectedSlot { get; private set; }

    public void UpdateLootUI()
    {
        if (lootSO != null)
        {
            lootIcon.sprite = lootSO.lootIcon;
            lootIcon.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            lootIcon.gameObject.SetActive(false);
            quantityText.text = "";
            
            // Make sure selection is cleared if item is removed
            if (isSelected)
            {
                DeselectSlot();
            }
        }
    }
    
    // Handle click events
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the click was actually on this slot specifically
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            GetComponent<RectTransform>(), 
            eventData.position, 
            eventData.enterEventCamera))
        {
            return; // Click wasn't properly on this slot
        }

        // Only allow selection if the slot has an item
        if (lootSO != null)
        {
            // If this slot is already selected, deselect it
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
                
                // Select this slot
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
        
        Debug.Log($"Selected item: {lootSO.lootName}");
    }
    
    private void DeselectSlot()
    {
        isSelected = false;
        
        // Clear static reference if this was the selected slot
        if (SelectedSlot == this)
        {
            SelectedSlot = null;
        }
        
        // Hide selection visual
        if (selectionOutline != null)
        {
            selectionOutline.gameObject.SetActive(false);
        }
    }
}