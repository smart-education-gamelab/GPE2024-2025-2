using UnityEngine;

public class WindmillTurn : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
