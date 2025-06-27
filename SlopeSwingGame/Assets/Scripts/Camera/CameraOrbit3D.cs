using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit3D : CameraMode
{
    [SerializeField] private float lowerYLimit = 5f;
    [SerializeField] private float upperYLimit = 80f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float orbitDampening = 1f;
    [SerializeField] private Transform hook;
    private Vector2 localRotation;
    private bool holdingCamera = false;
    [SerializeField] private int zoomChange = 0;
    [SerializeField] private int zoomChangeMax = 20;
    [SerializeField] private float zoomStep = 1f;

    public override void Activate()
    {
        hook.position = Target.position;
        base.Activate();

        transform.LookAt(Target);

        Quaternion targetRotation = Quaternion.Euler(localRotation.y, localRotation.x, 0f);
        hook.rotation = Quaternion.Lerp(hook.rotation, targetRotation, orbitDampening * Time.deltaTime);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        hook.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void CameraLockOn(InputAction.CallbackContext context)
    {
        if (!enabled)
        {
            return;
        }

        // We are pressing the button down
        if (context.performed)
        {
            holdingCamera = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (context.canceled)
        {
            holdingCamera = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void CameraOrbit(InputAction.CallbackContext context)
    {
        if (!enabled || !holdingCamera)
        {
            return;
        }

        transform.LookAt(Target);

        Vector2 mouseInput = context.ReadValue<Vector2>();

        localRotation.x += mouseInput.x * rotationSpeed;
        localRotation.y -= mouseInput.y * rotationSpeed;

        localRotation.y = Mathf.Clamp(localRotation.y, lowerYLimit, upperYLimit);

        Quaternion targetRotation = Quaternion.Euler(localRotation.y, localRotation.x, 0f);
        hook.rotation = Quaternion.Lerp(hook.rotation, targetRotation, orbitDampening * Time.deltaTime);
    }

    public void CameraZoom(InputAction.CallbackContext context)
    {
        if (!enabled)
        {
            return;
        }

        // Only on the Y (thanks Unity)
        Vector2 scrollInput = context.ReadValue<Vector2>();

        if (scrollInput.y != 0)
        {
            // WE SCROLLED

            if (scrollInput.y > 0 && zoomChange < zoomChangeMax)
            {
                zoomChange++;
                StepFromHook(-1);
            }
            else if (scrollInput.y < 0 && zoomChange > -zoomChangeMax)
            {
                zoomChange--;
                StepFromHook(1);
            }
        }
    }

    protected override void OnFixedUpdate()
    {
        if (!enabled)
        {
            return;
        }

        base.OnFixedUpdate();
        hook.position = Target.position;
    }

    private void StepFromHook(int directionToHook)
    {
        Vector3 directionFromHook = (transform.position - hook.position).normalized;

        transform.position += directionFromHook * zoomStep * directionToHook;
    }
}
