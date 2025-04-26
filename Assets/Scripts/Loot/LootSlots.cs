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
        
        // Trigger event for Loot Description Panel update
        OnSlotSelectionChanged?.Invoke(lootSO);
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

        // Trigger event for Loot Description Panel update
        OnSlotSelectionChanged?.Invoke(null);
    }
}