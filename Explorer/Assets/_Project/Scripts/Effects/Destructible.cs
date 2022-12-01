using System;
using System.Collections;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public abstract class Destructible : MonoBehaviour
{
    [SerializeField] protected Transform ThisTransform;
    [SerializeField] protected Rigidbody ThisRigidbody;
    [SerializeField] protected Collider ThisCollider;
    [SerializeField] protected Vector3 OriginalLocalPosition;
    [SerializeField] protected Quaternion OriginalLocalRotation;
    
    public bool IsDestroyed { get; private set; }

    public UnityEvent WasDestroyed;

    private const int MIN_EXPLOSION_FORCE = 250;
    private const float FORCE_MULTIPLIER = 3;
    private const int EXPLOSION_RADIUS = 5;
    

    protected virtual void Awake()
    {
        Initialize();
    }
    
    [Button]
    public virtual void PopulateParameters()
    {
        if (!ThisTransform) ThisTransform = transform;
        if (!ThisRigidbody)
        {
            ThisRigidbody = ThisTransform.GetComponent<Rigidbody>();
            if (!ThisRigidbody) ThisRigidbody = gameObject.AddComponent<Rigidbody>();
        }
        
        if (!ThisCollider)
        {
            ThisCollider = GetComponent<Collider>();
        }
        
        OriginalLocalPosition = ThisTransform.localPosition;
        OriginalLocalRotation = ThisTransform.localRotation;
    }
    
    public virtual void Initialize()
    {
        if (!ThisTransform)
        {
            ThisTransform = transform;
        }

        if (!ThisRigidbody)
        {
            ThisRigidbody = GetComponent<Rigidbody>();
        }

        if (!ThisCollider)
        {
            ThisCollider = GetComponent<Collider>();
        }
    }

    [Button]
    public virtual void ResetObject() 
    {
        if (ThisRigidbody) ThisRigidbody.isKinematic = true;
        
        ThisTransform.localPosition = OriginalLocalPosition;
        ThisTransform.localRotation = OriginalLocalRotation;

        Enable();
    }

    [Button]
    public virtual void Demolish(Vector3 impactOrigin, float force)
    {
        if (IsDestroyed) return;
        
        IsDestroyed = true;
        
        if (DebugLogger.IsNullError(ThisRigidbody, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(force, this)) return;

        ThisRigidbody.isKinematic = false;
        
        var adjustedForce = force * FORCE_MULTIPLIER + MIN_EXPLOSION_FORCE;
        ThisRigidbody.AddExplosionForce(adjustedForce, impactOrigin, EXPLOSION_RADIUS);

        WasDestroyed?.Invoke();
    }

    protected void Disable()
    {
        gameObject.SetActive(false);
    }

    protected void Enable()
    {
        IsDestroyed = false;
    }
}