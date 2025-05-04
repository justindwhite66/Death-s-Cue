using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Soldier : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject projectilePrefab;
    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;

    // Update is called once per frame
   readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Attack(){
        myAnimator.SetTrigger(ATTACK_HASH);

        if (transform.position.x - PlayerController.Instance.transform.position.x <0)
        spriteRenderer.flipX = false;
        else{
            spriteRenderer.flipX = true;
        }
    }

    public void SpawnProjectileAnimEvent(){
        Instantiate(projectilePrefab, transform.position, Quaternion.identity);
    }
}
