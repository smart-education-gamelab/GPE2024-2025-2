using System;
using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour
{
    [SerializeField] private Transform rocket;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject fire;
    private bool ready = false;
    [SerializeField] private float speed;
    [SerializeField] private float rocketLaunchTime = 6;
    [SerializeField] private float countDownTimer = 0;
    [SerializeField] private float rocketSmokeTime = 2;
    [SerializeField] private float rocketFireTime = 4;
    private bool playerInRocket = false;
    private float loadParticleTime = 0.5f;
    private float timer = 0;

    void Update()
    {
        if(ready)
        {
            rocket.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            other.transform.SetParent(rocket, true);
            playerInRocket = true;
        }
    }

    private void Start()
    {
        smoke.SetActive(true);
        fire.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (loadParticleTime > timer)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= loadParticleTime)
            {
                smoke.SetActive(false);
                fire.SetActive(false);
            }
        }
        
        if (!playerInRocket)
        {
            return;
        }

        if (countDownTimer >= rocketSmokeTime)
        {
            smoke.SetActive(true);
        }
        
        if (countDownTimer >= rocketFireTime)
        {
            fire.SetActive(true);
        }
        
        if (countDownTimer >= rocketLaunchTime)
        {
            ready = true;
        }

        countDownTimer += Time.deltaTime;
    }

}
