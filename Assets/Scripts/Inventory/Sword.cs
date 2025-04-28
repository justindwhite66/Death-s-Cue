using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Sword : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private float swordAttackCD = .5f;
        [SerializeField] private WeaponInfo weaponInfo;
 


    private Animator myAnimator;
    private Transform weaponCollider;
    private GameObject slashAnim;
    private SwordUIController cooldownUI;
    private bool isParrying = false;
    private void Awake(){
        

        myAnimator = GetComponent<Animator>();
    }

    private void Start(){
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = GameObject.Find("SlashAnimationSpawnPoint").transform;

        GameObject uiObj = GameObject.Find("Sword Slider");
        if (uiObj != null){
            cooldownUI = uiObj.GetComponent<SwordUIController>();
        }
    }

    private void Update(){
        MouseFollowWithOffset();
    }

    public WeaponInfo GetWeaponInfo(){
        return weaponInfo;
    }
    
    public void Attack(){
        
            
            myAnimator.SetTrigger("Attack");
            weaponCollider.gameObject.SetActive(true);
            isParrying = true;
            slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
            slashAnim.transform.parent = this.transform.parent;

            cooldownUI?.StartCooldown(weaponInfo.weaponCooldown);
            
        
    }

    

    
    public void DoneAttackingAnimEvent(){
        weaponCollider.gameObject.SetActive(false);
        isParrying = false;
    }

    public void SwingUpFlipAnim(){
        FlipLeft_X_Angle(-180);
    }

    public void SwingDownFlipAnim(){
        FlipLeft_X_Angle(0);
    }

    private void FlipLeft_X_Angle(int xAngle){
        slashAnim.transform.rotation = Quaternion.Euler(xAngle,0,0);

        if(PlayerController.Instance.FacingLeft){
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
   
   private void MouseFollowWithOffset(){
    if (ActiveWeapon.Instance == null){return;}
    
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector3 playerPos = PlayerController.Instance.transform.position;
    Vector2 direction = mousePos - playerPos;

    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    // Apply rotation directly on Z axis
    ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);

    // Flip the sword sprite vertically if pointing left
    if (Mathf.Abs(angle) > 90f)
    {
        ActiveWeapon.Instance.transform.localScale = new Vector3(1, -1, 1);
    }
    else
    {
        ActiveWeapon.Instance.transform.localScale = new Vector3(1, 1, 1);
    }

    // Optionally align weaponCollider too (if it’s rotated separately)
    weaponCollider.transform.rotation = Quaternion.Euler(0, 0, angle);

   }
   public bool IsParrying(){
    return isParrying;
   }

  
}