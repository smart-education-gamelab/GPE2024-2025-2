using UnityEngine;

public class StayInfrontOfCamera : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float distanceFromCamera = 10f;

    private void FixedUpdate()
    {
        transform.position = targetCamera.transform.position + (targetCamera.transform.forward * distanceFromCamera);
        transform.rotation = Quaternion.LookRotation(targetCamera.transform.up, targetCamera.transform.up);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
