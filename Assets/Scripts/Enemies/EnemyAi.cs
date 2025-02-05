using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    [SerializeField] private float roamChangeDirFloat = 2f;
<<<<<<< HEAD
=======
   
>>>>>>> f0b96baac953bb05828a27e9b67a15bd46d13800
   private enum State {
    Roaming
   }

   private State state;
    private EnemyPath enemyPath;

   private void Awake(){
    enemyPath = GetComponent<EnemyPath>();
    state = State.Roaming;
   }

    private void Start(){
        StartCoroutine(RoamingRoutine());
    }
   private IEnumerator RoamingRoutine(){
    while (state == State.Roaming){
        Vector2 roamPosition = GetRoamingPosition();
        enemyPath.MoveTo(roamPosition);
        yield return new WaitForSeconds(roamChangeDirFloat);
    }
   }

   private Vector2 GetRoamingPosition(){
    return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
   }
}
