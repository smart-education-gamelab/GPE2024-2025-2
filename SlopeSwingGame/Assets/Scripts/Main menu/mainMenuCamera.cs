using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 50f, 0f);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);


        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
