using UnityEngine;

public abstract class CameraMode : MonoBehaviour
{
    public Transform Target => target;
    public bool ShowUI => showUI;

    [SerializeField] protected float xOffset = 0f;
    [SerializeField] protected float yOffset = 10f;
    [SerializeField] protected float zOffset = 0f;
    [SerializeField] private bool orphographic = true;
    [SerializeField] private CursorLockMode cursorLockMode = CursorLockMode.None;
    public CursorLockMode CursorLockMode => cursorLockMode;
    
    [SerializeField] private bool showUI;
    private Transform target;
    private Camera sourceCamera;

    public void SetTarget(Transform newTarget) { target = newTarget; }

    public void SetOffset(float newXOffset, float newYOffset, float newZOffset)
    {
        xOffset = newXOffset;
        yOffset = newYOffset;
        zOffset = newZOffset;
    }

    public void SetCursorLockMode(CursorLockMode newCursorLockMode)
    {
        cursorLockMode = newCursorLockMode;
        Cursor.lockState = cursorLockMode;
    }
    
    public virtual void Activate()
    {
        sourceCamera.orthographic = orphographic;
        Cursor.lockState = cursorLockMode;
        transform.position = Target.position + new Vector3(xOffset, yOffset, zOffset);
    }

    public virtual void Deactivate()
    {

    }

    protected virtual void OnAwake()
    {
        sourceCamera = Camera.main;
    }

    protected virtual void OnStart()
    {

    }

    protected virtual void OnFixedUpdate()
    {
        
    }

    protected virtual void OnUpdate()
    {

    }

    private void Awake()
    {
        OnAwake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnStart();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }
}
