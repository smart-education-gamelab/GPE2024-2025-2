using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallPathFollower : MonoBehaviour
{
    public Transform[] pathPoints; // 0 = start, last = hole
    public float speed = 2f;
    public float resetDelay = 1f;
    public float waitAtEachPoint = 1f;

    private int currentPointIndex = 0;
    private bool isResetting = false;
    private bool isWaiting = false;
    private float stopDistance;
    private LineRenderer lineRenderer;
    private Transform currentTarget;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;


        currentTarget = pathPoints[currentPointIndex];
    }

    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, currentTarget.position);

        if (isResetting || isWaiting || pathPoints.Length == 0) return;

        // Set stop distance depending on whether we're at the hole
        stopDistance = (currentPointIndex == pathPoints.Length - 1) ? 1f : 0.5f;

        // Move ball toward the current target point
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentTarget.position) < stopDistance)
        {
            StartCoroutine(WaitAndAdvance());
        }
    }

    private System.Collections.IEnumerator WaitAndAdvance()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitAtEachPoint);

        currentPointIndex++;

        if (currentPointIndex >= pathPoints.Length)
        {
            StartCoroutine(ResetBall());
        }
        else
        {
            currentTarget = pathPoints[currentPointIndex];
        }

        isWaiting = false;
    }

    private System.Collections.IEnumerator ResetBall()
    {
        isResetting = true;
        lineRenderer.enabled = false;

        yield return new WaitForSeconds(resetDelay);

        transform.position = pathPoints[0].position;
        currentPointIndex = 1;
        currentTarget = pathPoints[currentPointIndex];

        lineRenderer.enabled = true;
        isResetting = false;
    }
}
