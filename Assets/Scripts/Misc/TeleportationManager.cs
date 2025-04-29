using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeleportationManager : Singleton<TeleportationManager>
{
   
   [SerializeField] private GameObject teleportFieldPrefab;
   [SerializeField] private float teleportCooldown = 10f;
   [SerializeField] private Slider slider;
   private float teleportRespawnTimer = 0f;
   private bool canRespawnField = true;
   public GameObject currentField;
   
   
   

   protected override void Awake() {
    base.Awake();
   }
   
private void Update() {
    if (Input.GetKeyDown(KeyCode.Q)){
        ManualFieldRespawn();
    }
    UpdateTeleportRespawnUI();
}
 

    private void Start()
    {
       if (slider == null){
            slider = GameObject.Find("Teleport Slider").GetComponent<Slider>();
        }
    }

    public void SpawnTeleportField(){
        
        Vector3 spawnPosition = PlayerController.Instance.transform.position;
        
       GameObject newField = Instantiate(teleportFieldPrefab, spawnPosition, Quaternion.identity);
        currentField = newField;
    }

    
  

    private void ManualFieldRespawn(){
        if (!canRespawnField) return;

        if (currentField != null){
            Destroy(currentField);
        }
        SpawnTeleportField();
        StartCoroutine(TeleportCooldownRoutine());
    }

    private IEnumerator TeleportCooldownRoutine(){
        canRespawnField = false;
        teleportRespawnTimer = teleportCooldown;

        while (teleportRespawnTimer > 0){
            teleportRespawnTimer -= Time.deltaTime;
            UpdateTeleportRespawnUI();
            yield return null;
        }
        canRespawnField = true;
        UpdateTeleportRespawnUI();
    }

    private void UpdateTeleportRespawnUI(){
        if (slider == null) return;
        if (canRespawnField){
            slider.value = 1f;
        }
        else{
            slider.value = 1f - (teleportRespawnTimer / teleportCooldown);
        }
    }
  
}
