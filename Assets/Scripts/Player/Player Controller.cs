using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Singleton<PlayerController>
{

   public bool FacingLeft {get {return facingLeft;}}

   [SerializeField] private float moveSpeed = 1f;
   [SerializeField] private float dashSpeed = 4f;
   [SerializeField] private TrailRenderer myTrailRenderer;
   [SerializeField] private Transform weaponCollider;
   [SerializeField] private float teleportRange = 8f;
   [SerializeField] private float teleportCooldownTime = 1.5f;
   [SerializeField] private GameObject teleportFieldPrefab;
   [SerializeField] private float fieldDestroyDelay = 3f;
   
   

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
   
   private float teleportCooldownTimer = 0f;

   protected override void Awake(){
    base.Awake();
    playerControls = new PlayerControls();
    rb = GetComponent<Rigidbody2D>();
    myAnimator = GetComponent<Animator>();
    mySpriteRender = GetComponent<SpriteRenderer>();
    knockback = GetComponent<Knockback>();
    mainCamera = Camera.main;
   }
   private void Start(){
      playerControls.Combat.Dash.performed += _ => Dash();
      playerControls.Movement.Teleport.performed += _ => AttemptTeleport();
      startingMoveSpeed = moveSpeed;
      ActiveInventory.Instance.EquipStartingWeapon();
   }


   private void OnEnable(){
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
    playerControls.Enable(); // Reset input actions to prevent missing references
}

    private void Update(){
    PlayerInput();
   }

   private void FixedUpdate(){
      AdjustPlayerFacingDirection();
      Move();
   }

   public Transform GetWeaponCollider(){
      return weaponCollider;
   }
   private void PlayerInput(){
    movement = playerControls.Movement.Move.ReadValue<Vector2>();
    myAnimator.SetFloat("moveX", movement.x);
    myAnimator.SetFloat("moveY", movement.y);

    
   }
   private void Move(){
      if(knockback.GettingKnockedBack || PlayerHealth.Instance.isDead) {return;}
      rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));

   }
  private void AdjustPlayerFacingDirection(){

   if (Camera.main == null)
    {
        Debug.LogWarning("Camera.main is null, trying to reassign...");
        return; // Prevents errors when the camera is destroyed
    }

   Vector3 mousePos = Input.mousePosition;
   Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

   if (mousePos.x < playerScreenPoint.x){
      mySpriteRender.flipX = true;
      facingLeft = true;
   }
   else {
      mySpriteRender.flipX = false; 
      facingLeft = false;
   }
  }


  private void Dash() {
    if (!isDashing && Stamina.Instance.CurrentStamina > 0) {
      Stamina.Instance.UseStamina();
      isDashing = true;
      moveSpeed *= dashSpeed;
      myTrailRenderer.emitting = true;
      StartCoroutine(EndDashRoutine());
    }
   }


  private IEnumerator EndDashRoutine() {

        float dashTime = .2f;
        float dashCD = .25f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
 }

 private void AttemptTeleport(){
   if (isTeleporting) {
      return;
   }

   if (Camera.main == null)
    {
        Debug.LogWarning("Camera.main is null! Waiting to reassign...");
        return; // Prevents teleportation until camera is assigned
    }

   Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    targetPosition.z = 0f;

    GameObject teleportField = TeleportationManager.Instance.currentField;
    if (teleportField == null) return;

    TeleportField fieldComponent = teleportField.GetComponent<TeleportField>();

   if (Vector3.Distance(teleportField.transform.position, targetPosition) <= fieldComponent.GetRadius())
    {
        // Check if the teleportation location is restricted
        if (fieldComponent.IsValidTeleportLocation(targetPosition))
        {
            transform.position = targetPosition;
            StartTeleportCooldown();
        }
        else
        {
            Debug.Log("Invalid teleport location! Cannot teleport onto water or foreground.");
        }
    }
}
 private void StartTeleportCooldown(){
   if (!isTeleporting){
      StartCoroutine(TeleportCooldownRoutine());
   }
 }
 private IEnumerator TeleportCooldownRoutine(){
   isTeleporting = true;
   teleportCooldownTimer = teleportCooldownTime;

   while (teleportCooldownTimer> 0){
      teleportCooldownTimer -= Time.deltaTime;
      yield return null;
   }
   isTeleporting = false;
 }


}