using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FollowCurve : MonoBehaviour
{
    public GameObject UI;

    public LineRenderer lineRenderer;
    public float moveSpeed = 5f;

    public Vector3[] curvePoints;
    private int currentPointIndex = 0;
    public bool isMoving = false;

    private Rigidbody rb;
    private Coroutine moveCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Disable physics
        //transform.position = curvePoints[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            moveCoroutine = StartCoroutine(MoveAlongCurve());
        }
    }

    IEnumerator MoveAlongCurve()
    {
        isMoving = true;
        rb.isKinematic = false;
        // hide the ui so the player can actually see how the golfball moves
        UI.SetActive(false);
        // find the nearest point to current position
        currentPointIndex = GetClosestPoint();

        while (currentPointIndex < curvePoints.Length - 1)
        {
            Vector3 end = curvePoints[currentPointIndex + 1];

            while (Vector3.Distance(transform.position, end) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, end, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = end;
            currentPointIndex++;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Vector3 frozenPosition = transform.position;
        rb.isKinematic = true;
        transform.position = frozenPosition;    
        isMoving = false;
        UI.SetActive(true);
    }

    int GetClosestPoint()
    {
        float minDist = float.MaxValue;
        int closest = 0;

        for (int i = 0; i < curvePoints.Length - 1; i++)
        {
            float dist = Vector3.Distance(transform.position, curvePoints[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }

        return closest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Vector3 frozenPosition = transform.position;
                rb.isKinematic = true;
                transform.position = frozenPosition;
                isMoving = false;

                UI.SetActive(true);
            }

            Debug.Log("colliding");

            // Apply a small bounce force
           // Vector3 bounceDirection = collision.contacts[0].normal;
            // rb.AddForce(bounceDirection * 0.5f, ForceMode.Impulse);

            isMoving = false;
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                UI.SetActive(true);
            }
            Debug.Log("Finished");
        }
    }
}