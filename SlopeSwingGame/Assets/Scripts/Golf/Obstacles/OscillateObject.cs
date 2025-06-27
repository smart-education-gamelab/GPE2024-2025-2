using UnityEngine;
using UnityEngine.Events;

    public enum OscillationType
    {
        PingPong,
        Sinusoidal
    }
public class OscillateObject : MonoBehaviour
{

    [Header("Oscillation Settings")]
    [SerializeField] private OscillationType oscillationType = OscillationType.PingPong;
    [SerializeField] private Vector3 moveDirection = Vector3.up;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool useLocalSpace = true;
    [SerializeField] private AnimationCurve motionCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Trigger Settings")]
    [SerializeField] private bool startOnPlay = true;
    [SerializeField] private UnityEvent onOscillationStart;
    [SerializeField] private UnityEvent onOscillationStop;

    private Vector3 startPosition;
    private bool oscillating = false;
    private float elapsedTime = 0f;

    private void Start()
    {
        startPosition = useLocalSpace ? transform.localPosition : transform.position;
        if (startOnPlay)
        {
            oscillating = true;
            onOscillationStart.Invoke();
        }
    }

    private void Update()
    {
        if (!oscillating) return;

        elapsedTime += Time.deltaTime;
        float value = 0f;

        switch (oscillationType)
        {
            case OscillationType.PingPong:
                value = Mathf.PingPong(elapsedTime * speed, 1f);
                break;
            case OscillationType.Sinusoidal:
                value = (Mathf.Sin(elapsedTime * speed * Mathf.PI * 2f) + 1f) / 2f;
                break;
        }

        value = motionCurve.Evaluate(value);
        Vector3 offset = moveDirection.normalized * (value * amplitude);
        if (useLocalSpace)
            transform.localPosition = startPosition + offset;
        else
            transform.position = startPosition + offset;
    }

    public void ToggleOscillation()
    {
        oscillating = !oscillating;
        if (oscillating) onOscillationStart.Invoke();
        else onOscillationStop.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        ToggleOscillation();
    }

#if UNITY_EDITOR
private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;

    Vector3 origin = Application.isPlaying ? startPosition : (useLocalSpace ? transform.localPosition : transform.position);
    Vector3 dir = moveDirection.normalized;

    Vector3 startPoint = origin - dir * amplitude;
    Vector3 endPoint = origin + dir * amplitude;

    if (useLocalSpace)
    {
        startPoint = transform.parent ? transform.parent.TransformPoint(startPoint) : transform.TransformPoint(startPoint);
        endPoint = transform.parent ? transform.parent.TransformPoint(endPoint) : transform.TransformPoint(endPoint);
    }

    Gizmos.DrawLine(startPoint, endPoint);
    Gizmos.DrawWireSphere(startPoint, 0.1f);
    Gizmos.DrawWireSphere(endPoint, 0.1f);
}
#endif
}
