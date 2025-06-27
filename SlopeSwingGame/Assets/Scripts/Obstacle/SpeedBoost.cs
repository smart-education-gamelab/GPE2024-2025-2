using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boostSpeed = 20f;
    public ForceMode forceMode = ForceMode.VelocityChange;

    private Rigidbody rb;
    private bool boosted = false;

    void OnTriggerEnter(Collider other)
    {
        rb = other.GetComponent<Rigidbody>();

        if (!boosted)
        {
            boosted = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = false;
            rb.AddForce(transform.forward * boostSpeed, forceMode);
            boosted = false;
        }
    }

    
    /*
    public float boostSpeed = 20f;
    public ForceMode forceMode = ForceMode.VelocityChange;

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        float currentSpeed = rb.linearVelocity.magnitude;
        Vector3 boostDirection = rb.linearVelocity.sqrMagnitude > 0.01f ? rb.linearVelocity.normalized : transform.forward;
        float newBoostSpeed = boostSpeed;

        Debug.Log($"Current Speed: {currentSpeed}, Boost Speed: {newBoostSpeed}");

        if (newBoostSpeed > currentSpeed)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.AddForce(boostDirection * newBoostSpeed, forceMode);
        }
    }
     */
}
