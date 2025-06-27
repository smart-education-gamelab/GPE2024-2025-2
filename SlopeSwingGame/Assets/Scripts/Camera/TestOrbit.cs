using UnityEngine;
using UnityEngine.InputSystem;

public class TestOrbit : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float mouseSpeed = 1f;
    [SerializeField] private float orbitDampening = 10f; 
    private Vector2 localRotation;

    public void CameraOrbit(InputAction.CallbackContext context)
    {
        Vector2 mouseInput = context.ReadValue<Vector2>();

        transform.position = player.position;

        localRotation.x += mouseInput.x * mouseSpeed;
        localRotation.y -= mouseInput.y * mouseSpeed;

        localRotation.y = Mathf.Clamp(localRotation.y, 0f, 80f);

        Quaternion targetRotation = Quaternion.Euler(localRotation.y, localRotation.x, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * orbitDampening);
    }
}
