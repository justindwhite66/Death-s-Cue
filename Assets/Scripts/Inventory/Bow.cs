using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    private Animator myAnimator;

    readonly int FIRE_HASH = Animator.StringToHash("Fire");

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
    public WeaponInfo GetWeaponInfo(){
        return weaponInfo;
    }
    public void Attack(){
        myAnimator.SetTrigger(FIRE_HASH);
        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
        newArrow.GetComponent<Projectile>().UpdateWeaponInfo(weaponInfo);
    }
}
