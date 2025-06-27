using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Rigidbody rb;
    private Vector3 moveVector;

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector3>();
    }

    private void FixedUpdate()
    {
        rb.AddForce(moveVector * speed);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        moveVector = new Vector3();
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
