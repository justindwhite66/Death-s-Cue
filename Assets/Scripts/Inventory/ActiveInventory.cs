using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ActiveInventory : MonoBehaviour
{
   
   private int activeSlotIndexNum = 0;

   private PlayerControls playerControls;

   private void Awake() {
    playerControls = new PlayerControls();
   }

   private void Start() {
    playerControls.Inventory.Keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());
    playerControls.Inventory.ScrollUp.performed += ctx => ChangeActiveSlot(1);
    playerControls.Inventory.ScrollDown.performed += ctx => ChangeActiveSlot(-1);
   }

   private void OnEnable() {
    playerControls.Enable();
   }

   private void ToggleActiveSlot(int numValue){
      ToggleActiveHighlight(numValue - 1);
   }

   private void ToggleActiveHighlight(int indexNum){
      activeSlotIndexNum = indexNum;

      foreach (Transform inventorySlot in this.transform){
         inventorySlot.GetChild(0).gameObject.SetActive(false);
      }

      this.transform.GetChild(indexNum).GetChild(0).gameObject.SetActive(true);

      ChangeActiveWeapon();
   }

   private void ChangeActiveSlot(int direction){
      activeSlotIndexNum = (activeSlotIndexNum + direction + this.transform.childCount) % this.transform.childCount;

      ToggleActiveHighlight(activeSlotIndexNum);
   }

   private void ChangeActiveWeapon(){
      //Debug.Log(transform.GetChild(activeSlotIndexNum).GetComponent<InventorySlot>().GetWeaponInfo().weaponPrefab.name);
   }
}
