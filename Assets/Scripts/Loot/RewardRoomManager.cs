using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardRoomManager : MonoBehaviour
{
    [Header("Loot Settings")]
    [SerializeField] private LootSO[] availableLoot;
    [SerializeField] private Transform[] spawnPoints; // Exactly 3 spawn points
    [SerializeField] private GameObject lootPrefab;
    
    [Header("Animation")]
    [SerializeField] private float delayBetweenSpawns = 0.5f;
    [SerializeField] private bool animateSpawning = true;
    
    private void Start()
    {
        if (availableLoot.Length == 0)
        {
            Debug.LogError("No loot available to spawn!");
            return;
        }
        
        if (spawnPoints.Length != 3)
        {
            Debug.LogWarning("Expected exactly 3 spawn points. Found: " + spawnPoints.Length);
        }
        
        // Start spawning loot
        if (animateSpawning)
        {
            StartCoroutine(SpawnLootWithDelay());
        }
        else
        {
            SpawnRandomLoot();
        }
    }
    
    private IEnumerator SpawnLootWithDelay()
    {
        // Select 3 random loot items
        LootSO[] selectedLoot = SelectRandomLoot();
        
        // Spawn each item with a delay
        for (int i = 0; i < selectedLoot.Length; i++)
        {
            if (i < spawnPoints.Length)
            {
                SpawnLootItem(selectedLoot[i], spawnPoints[i]);
                yield return new WaitForSeconds(delayBetweenSpawns);
            }
        }
    }
    
    private void SpawnRandomLoot()
    {
        // Select 3 random loot items
        LootSO[] selectedLoot = SelectRandomLoot();
        
        // Spawn each item at a fixed spawn point
        for (int i = 0; i < selectedLoot.Length; i++)
        {
            if (i < spawnPoints.Length)
            {
                SpawnLootItem(selectedLoot[i], spawnPoints[i]);
            }
        }
    }
    
    private LootSO[] SelectRandomLoot()
    {
        // Create a temporary list to shuffle
        List<LootSO> lootPool = new List<LootSO>(availableLoot);
        LootSO[] selectedLoot = new LootSO[3]; // Always select exactly 3 items
        
        // Make sure we have enough items to choose from
        if (lootPool.Count < 3)
        {
            Debug.LogWarning("Not enough loot items available. Needed: 3, Found: " + lootPool.Count);
            
            // Fill with available items and duplicate if necessary
            for (int i = 0; i < 3; i++)
            {
                selectedLoot[i] = lootPool[i % lootPool.Count];
            }
            
            return selectedLoot;
        }
        
        // Select 3 random items without repetition
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, lootPool.Count);
            selectedLoot[i] = lootPool[randomIndex];
            lootPool.RemoveAt(randomIndex);
        }
        
        return selectedLoot;
    }
    
    private void SpawnLootItem(LootSO lootSO, Transform spawnPoint)
    {
        GameObject lootObject = Instantiate(lootPrefab, spawnPoint.position, Quaternion.identity);
        Loot lootComponent = lootObject.GetComponent<Loot>();
        
        if (lootComponent != null)
        {
            lootComponent.lootSO = lootSO;
            lootComponent.quantity = GenerateRandomQuantity(lootSO);
            
            // Update sprite and name through OnValidate
            lootComponent.OnValidate();
        }
    }
    
    // Generate a random quantity based on the loot type
    private int GenerateRandomQuantity(LootSO lootSO)
    {
        if (lootSO.lootName.Contains("Coffee"))
        {
            return 1;
        }
        else if (lootSO.lootName.Contains("Burrito"))
        {
            return Random.Range(1,2);
        }
        else if (lootSO.lootName.Contains("Soda"))
        {
            return Random.Range(1,2);
        }
        else
        {
            return Random.Range(1,5); // Default for normal loot
        }
    }
}