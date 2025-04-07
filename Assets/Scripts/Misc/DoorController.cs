using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator doorController;
    private Collider2D doorCollider;
    private bool isOpen = false;
    private bool playerNearby = false;

private void Awake() {
    doorController = GetComponent<Animator>();
    Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders){
            if(!col.isTrigger){
                doorCollider = col;
                break;
            }
        }
}
    private void Update()
    {
        if (!isOpen && playerNearby){
            doorOpen();
        }
        else if (isOpen && !playerNearby){
            doorClose();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")){
            playerNearby = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        playerNearby = false;
    }
}

    private void doorOpen(){
        isOpen = true;
        doorController.SetTrigger("Open");
        StartCoroutine(colliderWaitRoutine());
        doorCollider.enabled = false;

    }

    private void doorClose(){
        isOpen = false;
        doorController.SetTrigger("Close");
        doorCollider.enabled = true;
    }

    private IEnumerator colliderWaitRoutine(){
        yield return new WaitForSeconds(1);
    }


}
