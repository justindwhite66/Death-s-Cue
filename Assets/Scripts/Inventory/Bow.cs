using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private int initialMaxAmmo = 8;
    private Animator myAnimator;
    private AmmoUIController ammoUI;

    readonly int FIRE_HASH = Animator.StringToHash("Fire");
private void Start() {
    GameObject uiObj = GameObject.Find("AmmoUI");
    if(uiObj != null){
        ammoUI = uiObj.GetComponent<AmmoUIController>();
    if (AmmoManager.Instance.MaxAmmo == 0){
        AmmoManager.Instance.InitializeBowAmmo(initialMaxAmmo);
    }
    ammoUI?.SetFromManager();
    }

}
    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
    public WeaponInfo GetWeaponInfo(){
        return weaponInfo;
    }

    private void Update() {
        if (ammoUI != null && Input.GetKeyDown(KeyCode.R)){
            ammoUI.ManualReload();
        }
    }
    public void Attack(){

        if (ammoUI == null || !ammoUI.CanFire()) return;
        myAnimator.SetTrigger(FIRE_HASH);
        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
        newArrow.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
        ammoUI.ConsumeAmmo();

        bool insideField = false;
        TeleportField[] fields = FindObjectsOfType<TeleportField>();
        foreach(TeleportField field in fields){
            if (Vector3.Distance(field.transform.position, PlayerController.Instance.transform.position) <= field.GetRadius()){
                insideField = true;
                break;
            }
        }
        if (insideField){
            Projectile projectileComponent = newArrow.GetComponent<Projectile>();
            if (projectileComponent != null){
                projectileComponent.bornInsideField = true;
            }
        }
        
    }
}
