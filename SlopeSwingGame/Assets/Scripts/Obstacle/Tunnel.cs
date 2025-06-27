using System.Linq;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
    [SerializeField] private GameObject[] tunnelSegments;

    [SerializeField] private BoxCollider startCollider;
    
    [SerializeField] private float moveSpeed = 5f;
    private Vector3[] waypoints;
    private int currentWaypoint = 0;
    private bool isFollowingPath = false;
    private Rigidbody rb;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rb = other.GetComponent<Rigidbody>();
            // Collect segment positions as waypoints
            waypoints = tunnelSegments.Select(seg => seg.transform.position).ToArray();
            FollowPath(waypoints);
        }
    }

    private void FollowPath(Vector3[] path)
    {
        waypoints = path;
        currentWaypoint = 0;
        isFollowingPath = true;
        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        if (!isFollowingPath || waypoints == null || currentWaypoint >= waypoints.Length)
            return;

        Vector3 target = waypoints[currentWaypoint];
        Vector3 direction = (target - rb.transform.position).normalized;
        float step = moveSpeed * Time.fixedDeltaTime;

        if (Vector3.Distance(rb.transform.position, target) <= step)
        {
            rb.transform.position = target;
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                isFollowingPath = false;
                rb.isKinematic = false;
            }
        }
        else
        {
            rb.transform.position += direction * step;
        }
    }
    
    private void OnDrawGizmos()
    {
        // Draw a cube at the tunnel's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);

        // Draw spheres at each tunnel segment's position
        if (tunnelSegments != null)
        {
            for (int i = 0; i < tunnelSegments.Length; i++)
            {
                var seg = tunnelSegments[i];
                if (seg == null) continue;

                Gizmos.color = (i == tunnelSegments.Length - 1) ? Color.red : Color.cyan;
                Gizmos.DrawSphere(seg.transform.position, 0.3f);
            }
        }
    }
}
