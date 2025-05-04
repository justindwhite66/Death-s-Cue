using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Search;
using UnityEngine;

public class ActiveWeapon : Singleton<ActiveWeapon>
{
    public MonoBehaviour CurrentActiveWeapon{get; private set;}
  private PlayerControls playerControls;
    private float timeBetweenAttacks;
    private bool attackButtonDown, isAttacking = false;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
        
    }

    private void OnEnable(){
        playerControls.Enable();
    }
    private void Start()
    {
        playerControls.Combat.Attack.started += _ => StartAttacking();
        playerControls.Combat.Attack.canceled += _ => StopAttacking();

        AttackCooldown();
    }

    private void Update()
    {
        Attack();
        
    }

    public void NewWeapon(MonoBehaviour newWeapon){
        CurrentActiveWeapon = newWeapon;
        AttackCooldown();
        timeBetweenAttacks = (CurrentActiveWeapon as IWeapon).GetWeaponInfo().weaponCooldown;
    }

    public void WeaponNull(){
        CurrentActiveWeapon = null;
    }

    private void AttackCooldown(){
        isAttacking = true;
        StopAllCoroutines();
        StartCoroutine(TimeBetweenAttacksRoutine());
    }

    private IEnumerator TimeBetweenAttacksRoutine(){
        yield return new WaitForSeconds(timeBetweenAttacks);
        isAttacking = false;
    }

    private void StartAttacking(){
        attackButtonDown = true;
        if (CurrentActiveWeapon is Staff staffWeapon){
            staffWeapon.StartCharging();
        }
    }
    private void StopAttacking(){

        attackButtonDown = false;

        if (CurrentActiveWeapon is Staff staffWeapon){
            staffWeapon.StopChargingAndTryFire();
        }
    }

    private void Attack(){

        if (attackButtonDown && !isAttacking && CurrentActiveWeapon){
            if (CurrentActiveWeapon is Staff){
                return;
            }
            AttackCooldown();
            (CurrentActiveWeapon as IWeapon).Attack();
        }
    }
}