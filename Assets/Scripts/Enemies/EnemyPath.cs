using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class EnemyPath : MonoBehaviour
{
   [SerializeField] private float moveSpeed = 2f;

   private Rigidbody2D rb;
   private Vector2 moveDir;
   private Knockback knockback;
   private SpriteRenderer spriteRenderer;
   private Animator animator;
   public event Action OnDestinationReached;

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
        }else if (moveDir.x > 0){
           spriteRenderer.flipX = false;
        }

     }
     

    public void MoveTo(Vector2 TargetPosition){
      
    moveDir = TargetPosition;
   
   }
 

   public void StopMoving(){
      moveDir = Vector3.zero;
   }

}
