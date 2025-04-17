using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Staff : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject magicLaser;
    [SerializeField] private Transform magicLaserSpawnPoint;
    [SerializeField] private float chargeTimeThreshold = 1.5f;
    [SerializeField] private GameObject chargeVFX;
    


    private SpriteRenderer spriteRenderer;
    private Animator myAnimator;
    private StaffChargeUIController chargeUIController;
    private float currentChargeTime = 0;
    private bool isCharging = false;
    private bool attackButtonHeld = false;
    private bool isCooldown = false;

    readonly int AttackHash = Animator.StringToHash("Attack");
     readonly int ChargeHash = Animator.StringToHash("IsCharging");

    
    private void Awake() {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start() {
    if (chargeUIController == null){
        GameObject sliderObj = GameObject.Find("Rifle Charge Slider");
        if (sliderObj != null){
            chargeUIController = sliderObj.GetComponent<StaffChargeUIController>();
        }
    }
}

    private void Update() {

       
        if (!isCooldown && attackButtonHeld && chargeUIController != null) {
        currentChargeTime += Time.deltaTime;

        float progress = Mathf.Min(currentChargeTime / chargeTimeThreshold, 1f);
        chargeUIController?.SetChargeProgress(progress);

        if (chargeVFX != null && !chargeVFX.activeSelf)
            chargeVFX.SetActive(true);
    }
    }

    public void Attack() {
        myAnimator.SetTrigger(AttackHash);
    }
    public void StartCharging(){
        if (isCooldown) return;

    attackButtonHeld = true;
    isCharging = true;
    currentChargeTime = 0f;

    chargeUIController?.SetChargeProgress(0f);

    if (chargeVFX != null)
        chargeVFX.SetActive(true);



    myAnimator.SetBool(ChargeHash, true);
    }

    
    public void StopChargingAndTryFire(){
        if (isCooldown) return;

    attackButtonHeld = false;
    isCharging = false;

    if (chargeVFX != null)
        chargeVFX.SetActive(false);

    myAnimator.SetBool(ChargeHash, false);

    if (currentChargeTime >= chargeTimeThreshold) {
        FireChargedShot();
        isCooldown = true;
        chargeUIController?.StartCooldown();
    } else {
        chargeUIController?.SetChargeProgress(0f);
    }

    currentChargeTime = 0f;
    }
    public void CooldownComplete(){
        isCooldown = false;
    }

    
    private void FireChargedShot(){
        myAnimator.SetTrigger(AttackHash);
    }
    
    public void SpawnStaffProjectileAnimEvent() {
        GameObject newLaser = Instantiate(magicLaser, magicLaserSpawnPoint.position, Quaternion.identity);
        newLaser.GetComponent<MagicLaser>().UpdateLaserRange(weaponInfo.weaponRange);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }



    
}
