using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : Singleton<SceneManagement>
{
    public string SceneTransitionName { get; private set;}

    public void SetTransitionName(string sceneTransitionName){
        this.SceneTransitionName = sceneTransitionName;
    }

    public void LoadScene(string sceneName){

        SceneTransitionName = sceneName;
        SceneManager.LoadScene(sceneName);
        TeleportationManager.Instance.SpawnTeleportField();
    }
  
}
