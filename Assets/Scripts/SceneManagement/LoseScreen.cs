using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreen : MonoBehaviour
{
    [SerializeField] private float waitTime = 2f;
    
    void Start()
    {
        StartCoroutine(WaitForTitleRoutine());
        
    }

    private IEnumerator WaitForTitleRoutine(){
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadSceneAsync("Title");
    }
}
