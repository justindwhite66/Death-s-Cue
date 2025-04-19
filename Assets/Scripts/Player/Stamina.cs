using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : Singleton<Stamina>
{
    [SerializeField] private Sprite fullStaminaImage, emptyStaminaImage;
    private Transform staminaContainer;
    public int CurrentStamina { get; private set;}

    const string STAMINA_TEXT = "Stamina Container";

    protected override void Awake()
    {
        base.Awake();
        StatsManager.Instance.maxStamina = StatsManager.Instance.startingStamina;
        CurrentStamina = StatsManager.Instance.startingStamina;
    }

    private void Start() {
        staminaContainer = GameObject.Find(STAMINA_TEXT).transform;
    }

    public void UseStamina(){
        CurrentStamina--;
        UpdateStaminaImages();
         StopAllCoroutines();
        StartCoroutine(RefreshStaminaRoutine());
    }
    public void ReplenshStaminaOnDeath(){
        CurrentStamina = StatsManager.Instance.startingStamina;
        UpdateStaminaImages();

    }

    public void RefreshStamina(){
        if(CurrentStamina< StatsManager.Instance.maxStamina && !PlayerHealth.Instance.isDead){
            CurrentStamina++;
        }
        UpdateStaminaImages();
    }
    private IEnumerator RefreshStaminaRoutine(){
        while (true){

            yield return new WaitForSeconds(StatsManager.Instance.staminaRefreshRate);
            RefreshStamina();
        }
    }

    private void UpdateStaminaImages(){
        for (int i = 0; i < StatsManager.Instance.maxStamina; i++)
        {
            Transform child = staminaContainer.GetChild(i);
            Image image = child?.GetComponent<Image>();

            if ( i <= CurrentStamina -1){
                image.sprite = fullStaminaImage;
            }else {
                image.sprite = emptyStaminaImage;

            }
        }
        
    }
}
