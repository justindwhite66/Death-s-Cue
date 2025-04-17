using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffChargeUIController : MonoBehaviour
{
    [SerializeField] private Slider chargeSlider;
    [SerializeField] private float cooldownTime = 2f;

    private Coroutine cooldownRoutine;

    public void SetChargeProgress(float progress)
    {
        if (chargeSlider != null)
            chargeSlider.value = Mathf.Clamp01(progress);
    }

    public void StartCooldown()
    {
        if (cooldownRoutine != null)
            StopCoroutine(cooldownRoutine);
        cooldownRoutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        float elapsed = 0f;
        if (chargeSlider != null)
            chargeSlider.value = 1f;

        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            if (chargeSlider != null)
                chargeSlider.value = Mathf.Lerp(1f, 0f, elapsed / cooldownTime);
            yield return null;
        }

        if (chargeSlider != null){
            chargeSlider.value = 0f;
        }

        if (ActiveWeapon.Instance.CurrentActiveWeapon is Staff staff){
            staff.CooldownComplete();
        }
    }
}
