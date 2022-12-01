using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using Zinnia.Tracking.Velocity;

public class PhysicsObject : MonoBehaviour
{
    [SerializeField] private AverageVelocityEstimator averageVelocityEstimator;
    [SerializeField] private Rigidbody thisRigidbody;
    [SerializeField] private Collider thisCollider;
    [SerializeField] private float size = 1;
    [SerializeField] private bool isKinematic;
    [SerializeField] private float addedSpeed = 0;
    [SerializeField] private bool getSizeOnEnable;
    

    private Dictionary<ColliderType, Action> _sizeActions = new Dictionary<ColliderType, Action>();
    private ColliderType _colliderType = ColliderType.Sphere;
    
    private enum ColliderType
    {
        Box,
        Sphere,
        Capsule,
        Mesh
    }

    private void Awake()
    {
        Initialize();
        
        if (DebugLogger.IsNullError(thisCollider, this, "Should be set in editor.")) return;
        
        _sizeActions.Add(ColliderType.Sphere, SetSizeFromSphere);
        _sizeActions.Add(ColliderType.Capsule, SetSizeFromCapsule);
        _sizeActions.Add(ColliderType.Box, SetSizeFromBox);
        _sizeActions.Add(ColliderType.Mesh, SetSizeFromMesh);
    }

    private void OnEnable()
    {
        if (!getSizeOnEnable) return;
        
        _sizeActions[_colliderType].Invoke();
    }
    
    [Button]
    public void PopulateParameters()
    {
        if (gameObject.isStatic) isKinematic = true;
        if (!averageVelocityEstimator) averageVelocityEstimator = GetComponent<AverageVelocityEstimator>();
        if (!thisRigidbody) thisRigidbody = GetComponent<Rigidbody>();
        if (thisRigidbody && thisRigidbody.isKinematic) isKinematic = true;
        if (!thisCollider) thisCollider = GetComponent<Collider>();
    }

    private void Initialize()
    {
        var sphereCollider = thisCollider as SphereCollider;
        if (sphereCollider)
        {
            _colliderType = ColliderType.Sphere;
            size = sphereCollider.bounds.size.magnitude;
            return;
        }

        var capsuleCollider = thisCollider as CapsuleCollider;
        if (capsuleCollider)
        {
            _colliderType = ColliderType.Capsule;
            size = capsuleCollider.bounds.size.magnitude;
            return;
        }

        var boxCollider = thisCollider as BoxCollider;
        if (boxCollider)
        {
            _colliderType = ColliderType.Box;
            size = boxCollider.bounds.size.magnitude;
            return;
        }

        var meshCollider = thisCollider as MeshCollider;
        if (meshCollider)
        {
            _colliderType = ColliderType.Mesh;
            size = meshCollider.bounds.size.magnitude;
            return;
        }
    }

    private void SetSizeFromSphere()
    {
        var sphereCollider = thisCollider as SphereCollider;
        size = sphereCollider.radius * 2;
    }
    
    private void SetSizeFromCapsule()
    {
        var capsuleCollider = thisCollider as CapsuleCollider;
        size = capsuleCollider.radius * 2;
    }
    
    private void SetSizeFromBox()
    {
        var boxCollider = thisCollider as BoxCollider;
        size = boxCollider.bounds.size.magnitude;
    }

    private void SetSizeFromMesh()
    {
        var meshCollider = thisCollider as MeshCollider;
        size = meshCollider.bounds.size.magnitude;
    }

    public float GetSpeed()
    {
        var speed = addedSpeed;
        
        if (averageVelocityEstimator) speed += averageVelocityEstimator.GetVelocity().magnitude;
        else if (thisRigidbody && !isKinematic) speed += thisRigidbody.velocity.magnitude;

        return speed;
    }

    public Vector3 GetVelocity()
    {
        if (averageVelocityEstimator) return averageVelocityEstimator.GetVelocity();
        if (thisRigidbody && !isKinematic) return thisRigidbody.velocity;
        
        return Vector3.zero;
    }

    public float GetSize()
    {
        return size;
    }

    public float GetForce()
    {
        return size * GetSpeed();
    }

    public bool GetIsKinematic()
    {
        return isKinematic;
    }
}
