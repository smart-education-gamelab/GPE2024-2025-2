using UnityEngine;
using UnityEngine.Events;

    public enum RotationMode
    {
        Continuous,
        Interval,
        Bidirectional,
        OnHitOnce,
        OnHitToggle
    }

public class ObjectRotator : MonoBehaviour
{

    [Header("Rotation Settings")]
    [SerializeField] private RotationMode rotationMode = RotationMode.Continuous;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 90f; // degrees per second
    [SerializeField] private bool useLocalRotation = true;
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Interval Settings")]
    [SerializeField] private float rotateDuration = 1f;
    [SerializeField] private float pauseDuration = 1f;

    [Header("Bidirectional Settings")]
    [SerializeField] private bool startInReverse = false;

    [Header("Trigger Settings")]
    [SerializeField] private UnityEvent onRotationStart;
    [SerializeField] private UnityEvent onRotationStop;

    private float timer;
    private bool rotating = false;
    private bool isReversed;
    private bool rotationTriggered = false;
    private bool toggleState = false;

    private Quaternion initialRotation;

    public RotationMode CurrentRotationMode => rotationMode;

    private void Start()
    {
        isReversed = startInReverse;
        initialRotation = transform.rotation;

        if (rotationMode == RotationMode.Continuous || rotationMode == RotationMode.Bidirectional)
        {
            rotating = true;
        }
        else if (rotationMode == RotationMode.Interval)
        {
            StartCoroutine(IntervalRoutine());
        }
    }

    private void Update()
    {
        if (!rotating) return;

        float delta = rotationSpeed * Time.deltaTime;
        float t = Mathf.PingPong(Time.time, 1f);
        delta *= rotationCurve.Evaluate(t);
        if (isReversed) delta *= -1f;

        Rotate(delta);
    }

    private void Rotate(float delta)
    {
        if (useLocalRotation)
            transform.Rotate(rotationAxis, delta, Space.Self);
        else
            transform.Rotate(rotationAxis, delta, Space.World);
    }

    private System.Collections.IEnumerator IntervalRoutine()
    {
        while (true)
        {
            rotating = true;
            onRotationStart.Invoke();
            yield return new WaitForSeconds(rotateDuration);

            rotating = false;
            onRotationStop.Invoke();
            yield return new WaitForSeconds(pauseDuration);

            if (rotationMode == RotationMode.Bidirectional)
                isReversed = !isReversed;
        }
    }

    public void TriggerRotationOnce()
    {
        if (rotationTriggered) return;
        rotationTriggered = true;
        StartCoroutine(RotateOnceRoutine());
    }

    public void ToggleRotation()
    {
        toggleState = !toggleState;
        rotating = toggleState;
        if (rotating) onRotationStart.Invoke();
        else onRotationStop.Invoke();
    }

    private System.Collections.IEnumerator RotateOnceRoutine()
    {
        rotating = true;
        onRotationStart.Invoke();
        yield return new WaitForSeconds(rotateDuration);
        rotating = false;
        onRotationStop.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        if (rotationMode == RotationMode.OnHitOnce)
            TriggerRotationOnce();
        else if (rotationMode == RotationMode.OnHitToggle)
            ToggleRotation();
    }

    // Optional debug
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 dir = useLocalRotation ? transform.TransformDirection(rotationAxis) : rotationAxis.normalized;
        Gizmos.DrawLine(transform.position, transform.position + dir);
        Gizmos.DrawWireSphere(transform.position + dir, 0.2f);
    }
#endif
}
