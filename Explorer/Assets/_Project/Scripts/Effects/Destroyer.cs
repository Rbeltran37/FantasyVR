using System;
using System.Collections;
using System.Collections.Generic;
using RayFire;
using Sirenix.OdinInspector;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;
    [SerializeField] private PhysicsObject physicsObject;
    [SerializeField] private int numHits = 1;

    private int _hitsRemaining;
    
    private const int INFINITE = -1;
    

    private void OnEnable()
    {
        _hitsRemaining = numHits;
    }

    private void OnCollisionEnter(Collision other)
    {
        Hit(other.GetContact(0).point, other.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        Hit(thisTransform.position, other.transform);
    }

    private void Hit(Vector3 impactOrigin, Transform otherTransform)
    {
        if (_hitsRemaining == 0) return;

        var destructible = otherTransform.GetComponent<Destructible>();
        if (!destructible) return;

        destructible.Demolish(impactOrigin, physicsObject.GetForce());
        
        if (_hitsRemaining == INFINITE) return;

        _hitsRemaining--;
    }

    [Button]
    public void PopulateParameters()
    {
        if (!thisTransform) thisTransform = transform;
        if (!physicsObject) physicsObject = GetComponent<PhysicsObject>();
    }
}
