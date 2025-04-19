using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject staminaPrefab;
    [SerializeField] private float pickUpLifetime = 3f;

    public void DropItems(){
        int pickUpNumber = Random.Range(1,3);

        switch (pickUpNumber){
   
            case 1:
                Instantiate(heartPrefab, transform.position, Quaternion.identity);
                
            break;
            case 2:
                Instantiate(staminaPrefab, transform.position, Quaternion.identity);
                
            break;
            default:
            break;

        }
    }

   
    
}
