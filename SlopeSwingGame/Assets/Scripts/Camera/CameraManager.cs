using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CameraMode[] cameraModes;
    private PlayerCardManager playerCardManager;
    int cameraModeIndex = 0;

    public void SetTarget(Transform target)
    {
        for (int i = 0; i < cameraModes.Length; i++)
        {
            cameraModes[i].SetTarget(target);
        }

        ActivateCurrentCameraMode();
    }

    public void OnCameraSwitchTarget(InputAction.CallbackContext context)
    {
        if (context.performed) {
            // Button was pressed down
            ToggleTarget();
        }
    }
    
    public CameraMode GetCurrentCameraMode()
    {
        return cameraModes[cameraModeIndex];
    }

    private void ToggleTarget()
    {
        cameraModes[cameraModeIndex].Deactivate();
        cameraModes[cameraModeIndex].enabled = false; // Disable it
        cameraModeIndex++;

        if (cameraModeIndex >= cameraModes.Length)
        {
            cameraModeIndex = 0;
        }

        ActivateCurrentCameraMode();
    }

    private void ActivateCurrentCameraMode()
    {
        cameraModes[cameraModeIndex].enabled = true;
        cameraModes[cameraModeIndex].Activate();
        playerCardManager.SetCameraFullUI(cameraModes[cameraModeIndex].ShowUI);
    }

    private void Awake()
    {
        playerCardManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerCardManager>();
    }
}
