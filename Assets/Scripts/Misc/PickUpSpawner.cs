using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoinPrefab;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject staminaPrefab;
    [SerializeField] private float pickUpLifetime = 3f;

    public void DropItems(){
        int pickUpNumber = Random.Range(1,6);

        switch (pickUpNumber){

            
            case 1:
                int randomGoldAmount = Random.Range(1,4);

                for (int i = 0; i < randomGoldAmount; i++)
                {
                Instantiate(goldCoinPrefab, transform.position, Quaternion.identity);
                
                }
            break;
            case 2:
                Instantiate(heartPrefab, transform.position, Quaternion.identity);
                
            break;
            case 3:
                Instantiate(staminaPrefab, transform.position, Quaternion.identity);
                
            break;
            default:
            break;

        }
    }

   
    
}
