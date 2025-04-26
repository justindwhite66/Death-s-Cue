using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootDetailsPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image itemIconDisplay;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private GameObject detailsContainer;

    private void OnEnable()
    {
        // Subscribe to selection changes
        LootSlots.OnSlotSelectionChanged += UpdateDetailsPanel;
        
        ClearDetailsPanel();
    }

    private void OnDisable()
    {
        // Unsubscribe when disabled
        LootSlots.OnSlotSelectionChanged -= UpdateDetailsPanel;
    }

    private void Start()
    {
        // Clear the panel on start
        ClearDetailsPanel();
    }

    public void UpdateDetailsPanel(LootSO selectedLoot)
    {
        if (selectedLoot == null)
        {
            ClearDetailsPanel();
            return;
        }

        // Show the details container
        if (detailsContainer != null)
            detailsContainer.SetActive(true);

        // Update icon
        if (itemIconDisplay != null)
        {
            itemIconDisplay.sprite = selectedLoot.lootIcon;
            itemIconDisplay.gameObject.SetActive(true);
        }

        // Update texts
        if (itemNameText != null)
            itemNameText.text = selectedLoot.lootName;

        if (itemDescriptionText != null)
            itemDescriptionText.text = selectedLoot.lootDescription;
    }

    public void ClearDetailsPanel()
    {
        // Hide the details container
        if (detailsContainer != null)
            detailsContainer.SetActive(false);

        // Clear all fields
        if (itemIconDisplay != null)
            itemIconDisplay.gameObject.SetActive(false);

        if (itemNameText != null)
            itemNameText.text = "";

        if (itemDescriptionText != null)
            itemDescriptionText.text = "";
    }
}