using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
   [SerializeField] private float moveSpeed = 2f;

   private Rigidbody2D rb;
   private Vector2 moveDir;
   private Knockback knockback;
   private SpriteRenderer spriteRenderer;
   private Animator animator;

   private void Awake(){
      knockback = GetComponent<Knockback>();
    rb = GetComponent<Rigidbody2D>();
     spriteRenderer = GetComponent<SpriteRenderer>();
     animator = GetComponent<Animator>();


   }

     private void FixedUpdate(){
        if (knockback.GettingKnockedBack){return;}
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        if (moveDir.x < 0){
           spriteRenderer.flipX = true;
        }else{
           spriteRenderer.flipX = false;
        }

     }

    /*private void FixedUpdate()
    {
        if (knockback.GettingKnockedBack){return ;}

        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        UpdateAnimation();
    }*/

    public void MoveTo(Vector2 TargetPosition){
    moveDir = TargetPosition;
   }

   private void UpdateAnimation(){
      if (moveDir != Vector2.zero){
         animator.SetFloat("MoveX", moveDir.x);
         animator.SetFloat("MoveY", moveDir.y);
         
      }

   }

   public void StopMoving(){
      moveDir = Vector3.zero;
   }
}
