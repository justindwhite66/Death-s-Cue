using UnityEngine;

public class ManagerInitializer : MonoBehaviour
{
    [SerializeField] private GameObject dataManagerPrefab;
    [SerializeField] private GameObject lootManagerPrefab;
    [SerializeField] private GameObject hotbarManagerPrefab;
    [SerializeField] private GameObject statsManagerPrefab;
    
    private void Awake()
    {
        Debug.Log("ManagerInitializer: Checking all required managers exist");
        
        // Create managers in correct order if needed
        EnsureManager<DataManager>(dataManagerPrefab);
        EnsureManager<StatsManager>(statsManagerPrefab);
        EnsureManager<LootManager>(lootManagerPrefab);
        EnsureManager<HotbarManager>(hotbarManagerPrefab);
        
        // Self-destruct after initialization
        Destroy(gameObject);
    }
    
    private void EnsureManager<T>(GameObject prefab) where T : Component
    {
        // Check if manager exists
        T existingManager = FindObjectOfType<T>();
        
        if (existingManager == null)
        {
            if (prefab != null)
            {
                Instantiate(prefab);
                Debug.Log($"Created missing manager: {typeof(T).Name}");
            }
            else
            {
                Debug.LogWarning($"Missing manager: {typeof(T).Name} and no prefab provided");
            }
        }
        else
        {
            Debug.Log($"Manager already exists: {typeof(T).Name}");
        }
    }
}
