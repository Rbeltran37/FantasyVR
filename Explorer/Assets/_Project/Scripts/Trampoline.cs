using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LaunchBall(Rigidbody rb)
    {
        Vector3 launchVector = transform.up;
        rb.velocity = Vector3.zero;
        rb.AddForce(launchVector * 100f, ForceMode.Impulse);
    }
    private void OnTriggerEnter(Collider other)
    {
        var rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            LaunchBall(rb);
        }
    }
}
