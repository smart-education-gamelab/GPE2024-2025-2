using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MathBall : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 40f;
    [SerializeField] private bool debugMode = false;
    [SerializeField] private GameObject debugShotBeacon;
    [SerializeField] private GameObject debugForceBeacon;
    private List<GameObject> debugShotBeacons;
    private List<GameObject> debugForceBeacons;
    [SerializeField] private float baseShootForce = 10f;
    private float multiplierForce = 1f;
    private bool shootingQuadratic = false;
    [SerializeField] private float quadraticMultiplier = 10f;
    private Rigidbody rb;
    private bool isAllowedToMove = false;
    private float minimumSpeedBeforeStop = 0.25f;
    private float speedCheckTime = 0.3f;
    private float speedCheckTimer = 0f;

    private Vector3[] shotTargets;
    private int shotTargetsIndex = 0;
    [SerializeField] private float shotTargetSnapZone = 5f;

    private SwingManager swingManager;

    private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip wallBounce;

    // Shot targets are essentially a bunch of points
    // We calculate the vector towards each point in a separate function
    public void Shoot(Vector3[] newShotTargets, float newMultiplier, bool isQuadratic)
    {
        multiplierForce = newMultiplier;
        shootingQuadratic = isQuadratic;
        shotTargets = newShotTargets;
        shotTargetsIndex = 0;

        if (debugMode && debugShotBeacon)
        {
            for (int i = 0; i < shotTargets.Length; i++)
            {
                debugShotBeacons.Add(Instantiate(debugShotBeacon, shotTargets[i], Quaternion.identity));
            }
        }

        ShootToTarget();
    }

    public void SetBaseShootForce(float force) { baseShootForce = force; }

    private Vector3 CalculateShotVector()
    {
        shotTargets[shotTargetsIndex].y = transform.position.y; // We ignore the height in case the floor is at a very slight slope
        return shotTargets[shotTargetsIndex] - transform.position; 
    }

    private void ShootToTarget()
    {
        // Wait have we reached the end of the list?
        if (shotTargetsIndex >= shotTargets.Length)
        {
            Debug.Log("No more lines to follow");
            return;
        }

        ResetMove(); // We do this so that it more closely follows the curve

        Vector3 shotVector = CalculateShotVector();
        shotVector.y = 0;

        isAllowedToMove = true;
        speedCheckTimer = 0f;

        Vector3 totalForceVector = shotVector * baseShootForce * multiplierForce;

        if (shootingQuadratic)
        {
            totalForceVector *= quadraticMultiplier;
        }

        rb.AddForce(totalForceVector);

        // Debug
        if (debugMode && debugForceBeacon)
        {
            debugForceBeacons.Add(Instantiate(debugForceBeacon, transform.position, Quaternion.identity));
        }
    }

    public void ResetMove()
    { 
        rb.linearVelocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    private bool CurrentTargetCheck()
    {
        if (shotTargetsIndex >= shotTargets.Length - 1)
        {
            return false;
        }

        shotTargets[shotTargetsIndex].y = transform.position.y; // We ignore the height in case the floor is at a very slight slope
        return Vector3.Distance(shotTargets[shotTargetsIndex], transform.position) < shotTargetSnapZone;
    }

    private void FixedUpdate()
    {
        // Are we allowed to move in the first place?
        if (!isAllowedToMove)
        {
            // No
            ResetMove();
        }
        else if (speedCheckTimer >= speedCheckTime)
        {
            // We have stood still for too long

            // Just stop moving
            isAllowedToMove = false;

            swingManager.BallStoppedMoving();

            // Debug

            for (int i = 0; i < debugShotBeacons.Count; i++)
            {
                Destroy(debugShotBeacons[i]);
            }

            debugShotBeacons.Clear();

            for (int i = 0; i < debugForceBeacons.Count; i++)
            {
                Destroy(debugForceBeacons[i]);
            }

            debugForceBeacons.Clear();
        }
        else
        {
            // Yes
            // Are we close enough to our current target?
            if (CurrentTargetCheck())
            {
                // We are!
                // Do the next shot
                shotTargetsIndex++;
                ShootToTarget();
            }
            // Did we lose all momentum?
            else if (rb.linearVelocity.magnitude < minimumSpeedBeforeStop)
            {
                // Yes. Increment the timer
                speedCheckTimer += Time.deltaTime;
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        swingManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<SwingManager>();
        soundFXManager = GameObject.FindGameObjectWithTag("SoundFXManager").GetComponent<SoundFXManager>();
        debugShotBeacons = new List<GameObject>();
        debugForceBeacons = new List<GameObject>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        swingManager.SetBall(this);
        SwitchGravity(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            soundFXManager.PlaySoundFXClip(wallBounce, transform, 1f);
        }
    }

    private void OnDrawGizmos()
    {
        // Set the color with custom alpha.
        Gizmos.color = Color.red;

        // Draw wire sphere outline.
        Gizmos.DrawWireSphere(transform.position, shotTargetSnapZone);
    }

    public float getBaseForce()
    {
        return baseShootForce;
    }

    public void SetForce(float t_newSpeed)
    {
        baseShootForce = t_newSpeed;
    }

    public void SwitchGravity(bool gravity)
    {
        rb.useGravity = gravity;
    }
}
