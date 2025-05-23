
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private enum PickUpType{
        StaminaGlobe,
        HealthGlobe,
        ShieldGlobe
    }

    [SerializeField] private PickUpType pickUpType;
    [SerializeField] private float pickUpDistance =5f;
    [SerializeField] private float moveSpeedAcceleration =.1f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 1.5f;
    [SerializeField] private float popDuration = 1f;
    [SerializeField] private float pickUpLifetime = 3f;
    
    private Vector3 moveDir;
    private Rigidbody2D rb;


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(AnimCurveSpawnRoutine());

        StartCoroutine(LifeTimeRoutine());
    }
    private void Update() {
        Vector3 playerPos = PlayerController.Instance.transform.position;

        if (Vector3.Distance(transform.position, playerPos)< pickUpDistance){
            moveDir = (playerPos - transform.position).normalized;
            moveSpeed += moveSpeedAcceleration;
        }else{
            moveDir = Vector3.zero;
            moveSpeed = 0;
        }
    }

    private void FixedUpdate() {
        rb.velocity = moveDir * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>()){
            DetectPickupType();
            Destroy(gameObject);
        }
    }
    private IEnumerator AnimCurveSpawnRoutine(){
        Vector2 startPoint = transform.position;
        Vector2 endPoint = new(transform.position.x + Random.Range(-2f, 2f), transform.position.y + Random.Range(-1f,1f));
        float timePassed = 0f;

        while (timePassed < popDuration){
            timePassed += Time.deltaTime;
            float linearT = timePassed / popDuration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(startPoint, endPoint, linearT) + new Vector2(0f, height);

            yield return null;
        }
    }

    private void DetectPickupType(){
        switch (pickUpType){

            case PickUpType.HealthGlobe:
                PlayerHealth.Instance.HealDamage();
                
                break;
            case PickUpType.StaminaGlobe:
                Stamina.Instance.RefreshStamina();
                break;
            case PickUpType.ShieldGlobe:
                StatsManager.Instance.AddShield(1);
                break;
            default:
                break;
        }
    }
    
    private IEnumerator LifeTimeRoutine(){

        yield return new WaitForSeconds(pickUpLifetime);

        SpriteFade spriteFade = GetComponent<SpriteFade>();

        if(spriteFade != null){

            yield return spriteFade.SlowFadeRoutine();
        } else{
            Destroy(gameObject);
        }
    }
}
