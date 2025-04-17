using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AmmoManager : MonoBehaviour
{
   public static AmmoManager Instance {get; private set;}

   public int CurrentAmmo {get; private set;}
   public int MaxAmmo{get; private set;}

    void Awake()
    {
        if (Instance != null && Instance != this){
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void InitializeBowAmmo(int maxAmmo){
        MaxAmmo = maxAmmo;
        CurrentAmmo = maxAmmo;
    }

    public void SetAmmo(int current, int max){
        CurrentAmmo = current;
        MaxAmmo = max;
    }

    public void ConsumeAmmo(){
        CurrentAmmo = Mathf.Max(0, CurrentAmmo - 1);
    }

    public void RefillAmmo(){
        CurrentAmmo = MaxAmmo;
    }
}
