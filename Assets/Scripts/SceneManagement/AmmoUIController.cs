using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.UIElements;

public class AmmoUIController : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private float reloadTime = 4f;
    [SerializeField] private UnityEngine.UI.Slider ammoSlider;

    private int maxAmmo;
    private int currentAmmo;
    private Coroutine reloadRoutine;
    private bool hasBeenInitialized = false;

    private bool isReloading;

    public bool IsReloading => isReloading;

    public void Initialize(int newMaxAmmo){
        if (!hasBeenInitialized){
        maxAmmo = newMaxAmmo;
        currentAmmo = newMaxAmmo;
        hasBeenInitialized = true;
        UpdateUI();
        }
    }
    public void SetFromManager(){
        maxAmmo = AmmoManager.Instance.MaxAmmo;
        currentAmmo = AmmoManager.Instance.CurrentAmmo;
        UpdateUI();
    }

    public bool CanFire(){
        return currentAmmo > 0 && !isReloading;
    }

    public void ConsumeAmmo(){
        currentAmmo = Mathf.Max(0, currentAmmo -1);
        AmmoManager.Instance.SetAmmo(currentAmmo, maxAmmo);
        UpdateUI();

        if (currentAmmo <= 0){
            StartReload();
        }
    }

    public void ManualReload(){
        if (currentAmmo < maxAmmo && !isReloading){
            StartReload();
        }
    }
    public void StartReload(){
        if(reloadRoutine != null){
            StopCoroutine(reloadRoutine);
        }
        reloadRoutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine(){
        isReloading = true;

        float elapsedTime = 0f;
        ammoSlider.value = 0f;

        while (elapsedTime < reloadTime){
            elapsedTime += Time.deltaTime;
            ammoSlider.value = Mathf.Lerp(0f, 1f, elapsedTime/reloadTime);
            yield return null;
        }
        currentAmmo = maxAmmo;
        isReloading = false;
        ammoSlider.value = 0f;
        if(AmmoManager.Instance != null){
            AmmoManager.Instance.SetAmmo(currentAmmo, maxAmmo);
        }
        UpdateUI();
    }
    private void UpdateUI(){

        if (ammoText != null){
            ammoText.text = $"{currentAmmo}/{maxAmmo}";
        }
    }
    public void SetMaxAmmo(int newMax){
        maxAmmo = newMax;
        currentAmmo = Mathf.Min(currentAmmo, maxAmmo);
        UpdateUI();
    }


}
