using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordUIController : MonoBehaviour
{
    [SerializeField] private Slider cooldownSlider;

    public void StartCooldown(float duration){
        if (cooldownSlider != null){
            StartCoroutine(CooldownRoutine(duration));
        }
    }
    private IEnumerator CooldownRoutine(float duration){
        float elapsed = 0f;
        cooldownSlider.value = 0f;

        while (elapsed < duration){
            elapsed += Time.deltaTime;
            cooldownSlider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        cooldownSlider.value = 0f;
    }
   
}
