using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportationManager : Singleton<TeleportationManager>
{
   
   [SerializeField] private GameObject teleportFieldPrefab;
   public GameObject currentField;
   
   

   protected override void Awake() {
    base.Awake();
    SceneManager.sceneLoaded += OnSceneLoaded;
   }
   

 

    private void Start()
    {
        StartCoroutine(SpawnTeleportFieldWithDelay());
    }

    public void SpawnTeleportField(){
        
        if (PlayerController.Instance == null)
        {
            StartCoroutine(SpawnTeleportFieldWithDelay());
            return;
        }
        
        Vector3 spawnPosition = PlayerController.Instance.transform.position;
        
        currentField = Instantiate(teleportFieldPrefab, spawnPosition, Quaternion.identity);
        
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        
        StartCoroutine(SpawnTeleportFieldWithDelay());
    }


    private IEnumerator SpawnTeleportFieldWithDelay()
    {
        yield return new WaitForSeconds(0.3f); // Small delay to let the player be positioned
        SpawnTeleportField();
    }
    
  
    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

  
}
