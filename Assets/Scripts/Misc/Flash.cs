using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
 [SerializeField] private Material shieldFlashMat;
 [SerializeField] private Material healthFlashMat;

 [SerializeField] private float restoreDefaultMatTime = .2f;
 private Material defaultMat;
 private SpriteRenderer spriteRenderer;

 private void Awake(){
    spriteRenderer = GetComponent<SpriteRenderer>();
    defaultMat = spriteRenderer.material;

 }

 public float GetRestoreMatTime(){
    return restoreDefaultMatTime;
 }

 public IEnumerator ShieldFlashRoutine(){
    spriteRenderer.material = shieldFlashMat;
    yield return new WaitForSeconds(restoreDefaultMatTime);
    spriteRenderer.material = defaultMat;
 }

 public IEnumerator HealthFlashRoutine(){
    spriteRenderer.material = healthFlashMat;
    yield return new WaitForSeconds(restoreDefaultMatTime);
    spriteRenderer.material = defaultMat;
 }


}
