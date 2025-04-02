using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDoorController : MonoBehaviour
{
    private Animator doorAnimator;
    private Collider2D doorCollider;
    private bool doorOpened = false;
    
    private void Awake() {
        doorAnimator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
    }

    private void Update() {
        if (!doorOpened && NoEnemeisRemaining()){
            OpenDoor();
        }
    }
    private bool NoEnemeisRemaining(){
        return GameObject.FindObjectsOfType<EnemyHealth>().Length == 0;    
    }

    private void OpenDoor(){
        doorOpened = true;
        doorAnimator.SetTrigger("Open");
        doorCollider.enabled = false;
    }
}
