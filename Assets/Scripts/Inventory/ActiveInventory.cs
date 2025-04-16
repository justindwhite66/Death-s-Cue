using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class ActiveInventory : Singleton<ActiveInventory>
{
   
   private int activeSlotIndexNum = 0;

   private PlayerControls playerControls;

   protected override void Awake() {
      base.Awake();
    playerControls = new PlayerControls();
   }

   private void Start() {
   ToggleActiveHighlight(0);
    playerControls.Inventory.Keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());
    playerControls.Inventory.ScrollUp.performed += ctx => ChangeActiveSlot(1);
    playerControls.Inventory.ScrollDown.performed += ctx => ChangeActiveSlot(-1);
   }

   private void OnEnable() {
    playerControls.Enable();
   }

   public void EquipStartingWeapon(){
      ToggleActiveHighlight(0);
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
      if (PlayerHealth.Instance.isDead) {return;}

      if (ActiveWeapon.Instance.CurrentActiveWeapon != null){
         Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
      }

      Transform childTransform = transform.GetChild(activeSlotIndexNum);
      InventorySlot inventorySlot = childTransform.GetComponentInChildren<InventorySlot>();
      WeaponInfo weaponInfo = inventorySlot?.GetWeaponInfo();
      GameObject weaponToSpawn = weaponInfo?.weaponPrefab;

      if (weaponInfo == null){
         ActiveWeapon.Instance.WeaponNull();
         return;
      }

      Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      mousePos.z = 0f;
      Vector3 direction = mousePos - ActiveWeapon.Instance.transform.position;
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


    
      GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.Euler(0f, 0f, angle));
      Debug.Log($"Spawning {weaponInfo.weaponPrefab} at angle {angle} with flip: {(weaponInfo.flipOnLeftSide && Mathf.Abs(angle) > 90f)}");

       newWeapon.transform.localScale = Vector3.one;
      if (weaponInfo.flipOnLeftSide && Mathf.Abs(angle) > 90f)
      {
        newWeapon.transform.localScale = new Vector3(1f, -1f, 1f);
      }

      if (weaponInfo.flipXWhenLeft && Mathf.Abs(angle) > 90f)
      {
      Vector3 currentScale = newWeapon.transform.localScale;
      newWeapon.transform.localScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
      }
ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
newWeapon.transform.parent = ActiveWeapon.Instance.transform;
ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());

   }
}