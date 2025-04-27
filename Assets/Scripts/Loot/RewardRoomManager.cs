using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardRoomManager : MonoBehaviour
{
    [Header("Loot Settings")]
    [SerializeField] private LootSO[] availableLoot;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject lootPrefab;
    
    [Header("Animation")]
    [SerializeField] private float delayBetweenSpawns = 0.5f;
    [SerializeField] private bool animateSpawning = true;
    
    private void Start()
    {
        if (availableLoot.Length == 0)
        {
            return;
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
        
        // Spawn each item with delay
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
        
        // Spawn each item at fixed spawn point
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
        // Create temporary list to shuffle
        List<LootSO> lootPool = new List<LootSO>(availableLoot);
        LootSO[] selectedLoot = new LootSO[3];
        
        // Make sure enough items to choose from
        if (lootPool.Count < 3)
        {            
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
        GameObject lootObject = Instantiate(
            lootPrefab, 
            spawnPoint.position, 
            Quaternion.identity
        );
        
        Loot lootComponent = lootObject.GetComponent<Loot>();
        
        if (lootComponent != null)
        {
            lootComponent.lootSO = lootSO;
            lootComponent.quantity = GenerateRandomQuantity(lootSO);
            
            // Update sprite and name through OnValidate
            lootComponent.OnValidate();
        }
    }
    
    // Generate a quantity based on loot type
    private int GenerateRandomQuantity(LootSO lootSO)
    {
        if (lootSO.lootName.Contains("Coffee"))
        {
            return 1;
        }
        else if (lootSO.lootName.Contains("Burrito"))
        {
            return Random.Range(1, 2);
        }
        else if (lootSO.lootName.Contains("Soda"))
        {
            return Random.Range(1, 2);
        }
        else
        {
            // Default for normal loot
            return Random.Range(1, 5);
        }
    }
}