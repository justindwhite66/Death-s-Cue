using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationManager : MonoBehaviour
{
   public static TeleportationManager Instance {get; private set;}
   [SerializeField] private GameObject teleportFieldPrefab;
   public GameObject currentField;

   /*protected override void Awake() {
    base.Awake();
    if (Instance == null)
        Instance = this;
   }*/

   private void Awake() {
    
    if (Instance == null)
        Instance = this;
   }

    private void Start()
    {
        SpawnTeleportField();
    }

    public void SpawnTeleportField(){
        if (currentField == null){return;}

        Vector3 spawnPos = PlayerController.Instance.transform.position;
        currentField = Instantiate(teleportFieldPrefab, spawnPos, Quaternion.identity);
    }
}
