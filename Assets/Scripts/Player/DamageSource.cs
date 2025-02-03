using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<EnemyAi>()) {
            Debug.Log("Enemy Hit");
        }
    }
}
