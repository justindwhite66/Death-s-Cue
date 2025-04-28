using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Singleton<PlayerController>
{

   public bool FacingLeft {get {return facingLeft;}}

   [SerializeField] private float dashSpeed = 4f;
   [SerializeField] private float dashDuration = .2f;
   [SerializeField] private float dashCooldown = 0.25f;
   [SerializeField] private TrailRenderer myTrailRenderer;
   [SerializeField] private Transform weaponCollider;
   [SerializeField] private float teleportRange = 8f;
   [SerializeField] private float teleportCooldownTime = 1.5f;
   [SerializeField] private float fieldDestroyDelay = 3f;
   [SerializeField] private float invincibilityTimer = 0.2f;
   [SerializeField] private Collider2D invincibilityDamageCollider;
   
   

   private PlayerControls playerControls;
   private Vector2 movement;
   private Rigidbody2D rb;
   private Animator myAnimator;
   private SpriteRenderer mySpriteRender;
   private Knockback knockback;
   
   private Camera mainCamera;
   
   private float startingMoveSpeed;
   private bool facingLeft = false;
   private bool isDashing = false;
   private bool isTeleporting = false;
   private bool isInvincible = false;
   
   private float teleportCooldownTimer = 0f;

   protected override void Awake()
   {
      base.Awake();
      playerControls = new PlayerControls();
      rb = GetComponent<Rigidbody2D>();
      myAnimator = GetComponent<Animator>();
      mySpriteRender = GetComponent<SpriteRenderer>();
      knockback = GetComponent<Knockback>();
      mainCamera = Camera.main;
   }
   private void Start()
   {
      playerControls.Combat.Dash.performed += _ => Dash();
      playerControls.Movement.Teleport.performed += _ => AttemptTeleport();
      startingMoveSpeed = StatsManager.Instance.moveSpeed;
      ActiveInventory.Instance.EquipStartingWeapon();
   }


   private void OnEnable()
   {
     playerControls.Enable();
     SceneManager.sceneLoaded += OnSceneLoaded;
   }

   void OnDisable()
   {
     playerControls.Disable();
     SceneManager.sceneLoaded -= OnSceneLoaded;
   }

   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
     playerControls.Disable();
     playerControls.Enable(); // Reset input actions, prevent missing references
   }

   private void Update()
   {
     PlayerInput();
   }

   private void FixedUpdate()
   {
     AdjustPlayerFacingDirection();
     Move();
   }

   public Transform GetWeaponCollider()
   {
     return weaponCollider;
   }

   private void PlayerInput()
   {
      movement = playerControls.Movement.Move.ReadValue<Vector2>();
      myAnimator.SetFloat("moveX", movement.x);
      myAnimator.SetFloat("moveY", movement.y);
   }

   private void Move()
   {
     if(knockback.GettingKnockedBack || PlayerHealth.Instance.isDead) {return;}
     rb.MovePosition(rb.position + movement * (StatsManager.Instance.moveSpeed * Time.fixedDeltaTime));
   }

   private void AdjustPlayerFacingDirection()
   {
      Vector3 mousePos = Input.mousePosition;
      Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

      if (mousePos.x < playerScreenPoint.x)
      {
         mySpriteRender.flipX = true;
         facingLeft = true;
      }
      else
      {
        mySpriteRender.flipX = false; 
        facingLeft = false;
      }
   }

   private void Dash() 
   {
      if (!isDashing && Stamina.Instance.CurrentStamina > 0) 
      {
        Stamina.Instance.UseStamina();
        isDashing = true;
        StatsManager.Instance.moveSpeed *= dashSpeed;
        myTrailRenderer.emitting = true;
        StartCoroutine(EndDashRoutine());
        StartCoroutine(DashInvincibilityRoutine());
      }
   }

   private IEnumerator DashInvincibilityRoutine()
   {
      isInvincible = true;
      if (invincibilityDamageCollider != null)
      {
        invincibilityDamageCollider.enabled = true;
      }

      yield return new WaitForSeconds(invincibilityTimer);

      if (invincibilityDamageCollider != null)
      {
         invincibilityDamageCollider.enabled = false;
      }

      isInvincible = false;
   }


   private IEnumerator EndDashRoutine()
   {
      yield return new WaitForSeconds(dashDuration);
      
      // IMPORTANT: Reapply any active modifiers AFTER resetting the speed
      HotbarSlot.ReapplyAllActiveModifiers();
      
      myTrailRenderer.emitting = false;
      yield return new WaitForSeconds(dashCooldown);
      isDashing = false;
   }

   private void AttemptTeleport()
   {
      if (isTeleporting)
      {
         return;
      }

      if (Camera.main == null)
      {
         return; 
      }

      Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      targetPosition.z = 0f;

      TeleportField[] teleportFields = FindObjectsOfType<TeleportField>();

      // Check if target position inside any teleport field
      foreach (TeleportField field in teleportFields)
      {
         if (Vector3.Distance(field.transform.position, targetPosition) <= field.GetRadius())
         {
            if (!field.IsValidTeleportLocation(targetPosition))
            {
               return;
            }
            
            transform.position = targetPosition;
            myTrailRenderer.emitting = true;
            
            StartCoroutine(InvincibilityRoutine());
            StartTeleportCooldown();
            
            return;
         }
      }
   }

   private IEnumerator InvincibilityRoutine()
   {
      isInvincible = true;
      if(invincibilityDamageCollider != null)
      {
         invincibilityDamageCollider.enabled = true;
      }

      yield return new WaitForSeconds(invincibilityTimer);

      if (invincibilityDamageCollider != null)
      {
         invincibilityDamageCollider.enabled = false;
      }
      
      isInvincible = false;
   }


   private void StartTeleportCooldown()
   {
      if (!isTeleporting)
      {
        
         StartCoroutine(TeleportCooldownRoutine());
      }
   }

   private IEnumerator TeleportCooldownRoutine()
   {
      isTeleporting = true; 
      
      teleportCooldownTimer = teleportCooldownTime;

      while (teleportCooldownTimer> 0)
      {
         teleportCooldownTimer -= Time.deltaTime;
         yield return null;
      }

      isTeleporting = false;
      myTrailRenderer.emitting = false;
   }
   public bool IsInvincible()
   {
      return isInvincible;
   }
}