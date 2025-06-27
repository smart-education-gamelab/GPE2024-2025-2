using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCameraController : CameraMode
{
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float fastMovementSpeed = 10f;
    [SerializeField] private float freeLookSensitivity = 3f;

    private Vector3 moveInputVector;
    private Vector3 moveVector;
    private Vector2 lookVector;
    private bool sprinting = false;
    private float speedMultiplier = 1;


    private float xRotation = 0f;
    private float yRotation = 0f;

    public void CameraMove(InputAction.CallbackContext context)
    {
        if (!enabled || CursorLockMode == CursorLockMode.None)
        {
            return;
        }

        moveInputVector = context.ReadValue<Vector3>();
    }

    public void CameraLook(InputAction.CallbackContext context)
    {
        if (!enabled || CursorLockMode == CursorLockMode.None)
        {
            return;
        }

        lookVector = context.ReadValue<Vector2>();

        lookVector *= freeLookSensitivity;
        yRotation += lookVector.x;
        xRotation = Mathf.Clamp(xRotation - lookVector.y, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    public void CameraSprint(InputAction.CallbackContext context)
    {
        if (!enabled)
        {
            return;
        }

        if (context.performed)
        {
            sprinting = true;
        }
        else if (context.canceled)
        {
            sprinting = false;
        }

        speedMultiplier = sprinting ? fastMovementSpeed : 1;
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        moveInputVector = new Vector3();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        moveVector = transform.forward * moveInputVector.z;
        moveVector += transform.right * moveInputVector.x;
        moveVector += transform.up * moveInputVector.y;

        transform.position += moveVector * movementSpeed * speedMultiplier;
    }
}
