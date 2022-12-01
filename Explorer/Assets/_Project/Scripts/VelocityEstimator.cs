using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityEstimator : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 controllerCenterOfMass;
    public Vector3 controllerVelocityCross;
    public Vector3 grabbedObjectOffset;
    public Vector3 fullThrowVelocity;
    public Vector3 angularVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        angularVelocity = rb.angularVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        controllerCenterOfMass = rb.centerOfMass;
        controllerVelocityCross = Vector3.Cross(rb.angularVelocity, grabbedObjectOffset - controllerCenterOfMass);
        fullThrowVelocity = rb.velocity + controllerVelocityCross;
    }
}
