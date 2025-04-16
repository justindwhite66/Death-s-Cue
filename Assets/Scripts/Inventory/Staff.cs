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
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private float cooldownTime = 3f;


    private SpriteRenderer spriteRenderer;
    private Animator myAnimator;
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
    // Optional safety check
    if (chargeSlider == null) {
        GameObject sliderObj = GameObject.Find("Rifle Charge Slider"); // <- Make sure this matches the name in the Hierarchy
        if (sliderObj != null) {
            chargeSlider = sliderObj.GetComponent<Slider>();
        }

        if (chargeSlider == null) {
            Debug.LogWarning("ChargeSlider not found in scene.");
        }
    }
}

    private void Update() {
        if (!isCooldown && attackButtonHeld) {
        currentChargeTime += Time.deltaTime;

        float progress = Mathf.Min(currentChargeTime / chargeTimeThreshold, 1f);
        chargeSlider.value = progress;

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

    if (chargeVFX != null)
        chargeVFX.SetActive(true);

    if (chargeSlider != null)
        chargeSlider.value = 0f;

    myAnimator.SetBool("IsCharging", true);
    }

    
    public void StopChargingAndTryFire(){
        if (isCooldown) return;

    attackButtonHeld = false;
    isCharging = false;

    if (chargeVFX != null)
        chargeVFX.SetActive(false);

    myAnimator.SetBool("IsCharging", false);

    if (currentChargeTime >= chargeTimeThreshold) {
        FireChargedShot();
        StartCoroutine(CooldownRoutine());
    } else {
        if (chargeSlider != null)
            chargeSlider.value = 0f;
    }

    currentChargeTime = 0f;
    }

    private IEnumerator CooldownRoutine(){
        isCooldown = true;

    if (chargeSlider != null)
        chargeSlider.value = 1f;

    float cooldownElapsed = 0f;
    while (cooldownElapsed < cooldownTime) {
        cooldownElapsed += Time.deltaTime;

        float value = Mathf.Lerp(1f, 0f, cooldownElapsed / cooldownTime);
        if (chargeSlider != null)
            chargeSlider.value = value;

        yield return null;
    }

    if (chargeSlider != null)
        chargeSlider.value = 0f;

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
