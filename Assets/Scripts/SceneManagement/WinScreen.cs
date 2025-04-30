using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private float waitTime = 10f;
    
    void Start()
    {
        StartCoroutine(WaitForTitleRoutine());
        
    }

    private IEnumerator WaitForTitleRoutine(){
        yield return new WaitForSeconds(waitTime);
        if (DataManager.Instance != null)
        {
            DataManager.Instance.ResetAllPlayerData();
        }
        
        // Reset stamina
        if (Stamina.Instance != null)
        {
            Stamina.Instance.ReplenshStaminaOnDeath();
        }

        SceneManager.LoadScene("Title");
    }
}
