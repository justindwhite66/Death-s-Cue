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
    [SerializeField] private float parryWindow = 0.2f;


    private Animator myAnimator;
    private Transform weaponCollider;
    private GameObject slashAnim;
    private bool isParrying = false;
    private void Awake(){
        

        myAnimator = GetComponent<Animator>();
    }

    private void Start(){
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = GameObject.Find("SlashAnimationSpawnPoint").transform;
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
    Vector3 mousePos = Input.mousePosition;
    
    Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);
   float angle = Mathf.Atan2(mousePos.y - playerScreenPoint.y, Mathf.Abs(mousePos.x - playerScreenPoint.x)) * Mathf.Rad2Deg;

   if (mousePos.x < playerScreenPoint.x) {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        } else {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);

        }

   }
   public bool IsParrying(){
    return isParrying;
   }

  
}