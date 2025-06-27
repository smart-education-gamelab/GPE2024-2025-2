using UnityEngine;
using System.Collections;

public class SandTrap : MonoBehaviour
{
    private float normalSpeed;
    private float slowedSpeed;
    [SerializeField] float slowdown = 2;
    private MathBall ball;
    private Transform ballTransform;
    [SerializeField] GameObject dustParticle;

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        ball = other.GetComponent<MathBall>();
        ballTransform = ball.transform;

        GameObject dust = Instantiate(dustParticle, ballTransform.position, ballTransform.rotation);
        dust.transform.SetParent(ballTransform, true);
        StartCoroutine(DestroyDust(dust));


        //if the ball lands inside the sand trap reduce the force for the next shot
        normalSpeed = ball.getBaseForce();
        slowedSpeed = normalSpeed / slowdown;
        ball.SetForce(slowedSpeed);

        //if the ball passes through slow it down
        rb.linearVelocity /= slowdown;
        rb.angularVelocity /= slowdown;

    }

    private void OnTriggerExit(Collider other)
    {
        //restore the balls base force
        ball = other.GetComponent<MathBall>();
        ball.SetForce(normalSpeed);
    }

    IEnumerator DestroyDust(GameObject t_dust)
    {
        yield return new WaitForSeconds(1);
           Destroy(t_dust);
    }
}

