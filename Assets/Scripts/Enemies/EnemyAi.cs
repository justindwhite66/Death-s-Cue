using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private float roamChangeDirFloat = 2f;
    [SerializeField] private float attackRange = 0f;
    [SerializeField] private MonoBehaviour enemyType;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool stopMovingWhileAttacking = false;
    [SerializeField] private float roamCooldown = 1.5f;

    private bool canAttack = true;

    private enum State {
        Roaming, 
        Attacking
    }

    private Vector2 roamPosition;
    private float timeRoaming = 0f;
    
    private State state;
    private EnemyPath enemyPath;

    private void Awake() {
        enemyPath = GetComponent<EnemyPath>();
        state = State.Roaming;
    }

    private void Start() {
        roamPosition = GetRoamingPosition();
    }

    private void Update() {
        MovementStateControl();
    }

    private void MovementStateControl() {
        switch (state)
        {
            default:
            case State.Roaming:
                Roaming();
            break;

            case State.Attacking:
                Attacking();
            break;
        }
    }

    private void Roaming() {
        timeRoaming += Time.deltaTime;

        enemyPath.MoveTo(roamPosition);

        if (PlayerController.Instance == null) return;

        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < attackRange) {
            state = State.Attacking;
        }

        if (timeRoaming > roamChangeDirFloat) {
            roamPosition = GetRoamingPosition();
        }
    }

    private void Attacking() {
        if (PlayerController.Instance == null) return;

        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > attackRange)
        {
            canAttack = false;
            StartCoroutine(StartRoamRoutine());
        }

        if (attackRange != 0 && canAttack) {

            canAttack = false;
            (enemyType as IEnemy).Attack();

            if (stopMovingWhileAttacking) {
                enemyPath.StopMoving();
            } else {
                enemyPath.MoveTo(roamPosition);
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator StartRoamRoutine(){
        yield return new WaitForSeconds(roamCooldown);
        state = State.Roaming;
        canAttack = true;
    }

    private IEnumerator AttackCooldownRoutine() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    

    private Vector2 GetRoamingPosition() {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }


}
