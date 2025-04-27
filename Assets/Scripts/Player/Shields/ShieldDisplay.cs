using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldDisplay : MonoBehaviour
{
    [SerializeField] private GameObject shieldIconPrefab;
    [SerializeField] private Transform shieldPanel;  
    private List<GameObject> activeShields = new List<GameObject>();
    private int previousShieldCount = 0;
    
    void Start()
    {
        // Initial shield setup
        UpdateShieldDisplay();
    }
    
    void Update()
    {
        // Only update if shield count changed
        if (StatsManager.Instance != null && 
            StatsManager.Instance.currentShield != previousShieldCount)
        {
            UpdateShieldDisplay();
        }
    }
    
    private void UpdateShieldDisplay()
    {
        // Make sure references exist
        if (shieldPanel == null || 
            shieldIconPrefab == null || 
            StatsManager.Instance == null) 
        {
            return;
        }
        
        int currentShieldCount = StatsManager.Instance.currentShield;
        previousShieldCount = currentShieldCount;
        
        // Handle adding new shields
        while (activeShields.Count < currentShieldCount)
        {
            AddShieldIcon();
        }
        
        // Handle removing shields
        while (activeShields.Count > currentShieldCount)
        {
            RemoveShieldIcon();
        }
    }
    
    private void AddShieldIcon()
    {
        // Instantiate new shield icon
        GameObject newShield = Instantiate(shieldIconPrefab, shieldPanel);
        
        // Set color
        Image shieldImage = newShield.GetComponent<Image>();
        
        // Add to tracking list
        activeShields.Add(newShield);
        
        // Play add animation (add if time)
        StartCoroutine(AnimateShieldAdd(newShield));
    }
    
    private void RemoveShieldIcon()
    {
        if (activeShields.Count == 0) return;
        
        // Get last shield in list
        GameObject shieldToRemove = activeShields[activeShields.Count - 1];
        activeShields.RemoveAt(activeShields.Count - 1);
        
        // Play remove animation then destroy
        StartCoroutine(AnimateShieldRemove(shieldToRemove));
    }
    
    private IEnumerator AnimateShieldAdd(GameObject shield)
    {
        // Simple scale-in animation
        shield.transform.localScale = Vector3.zero;
        
        float duration = 0.25f;
        float timer = 0f;
        
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            shield.transform.localScale = Vector3.one * 
                Mathf.SmoothStep(0, 1, progress);
            yield return null;
        }
        
        shield.transform.localScale = Vector3.one;
    }
    
    private IEnumerator AnimateShieldRemove(GameObject shield)
    {
        // Simple flash and fade animation
        Image img = shield.GetComponent<Image>();
        float duration = 0.3f;
        float timer = 0f;
        
        // Flash white
        if (img != null) img.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        
        // Fade out
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            
            if (img != null)
            {
                Color c = img.color;
                c.a = 1 - progress;
                img.color = c;
            }
            
            shield.transform.localScale = Vector3.one * (1 - progress);
            yield return null;
        }
        
        Destroy(shield);
    }
}