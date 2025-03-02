using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : Singleton<CameraController>
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject); // Prevents the camera from being destroyed
    }

    private void Start()
    {
        StartCoroutine(AssignCameraAfterSceneLoad());
    }

    private IEnumerator AssignCameraAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure scene is loaded
        SetPlayerCameraFollow();
    }

    public void SetPlayerCameraFollow()
    {
        cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (cinemachineVirtualCamera != null)
        {
            cinemachineVirtualCamera.Follow = PlayerController.Instance.transform;
        }
        else
        {
            Debug.LogWarning("No CinemachineVirtualCamera found in the scene!");
        }
    }
}
