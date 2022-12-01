using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class DestructibleStructure : Destructible
{
    [SerializeField] private float rayMaxDistance = .5f;
    [SerializeField] private float collisionSpeedThresholdThreshold = 5f;
    [SerializeField] private PhysicsObject physicsObject;
    [SerializeField] private DissolveEffect dissolveEffect;
    [SerializeField] private List<DestructibleStructure> structuresSupportedBy = new List<DestructibleStructure>();
    [SerializeField] private List<DestructibleStructure> structuresSupporting = new List<DestructibleStructure>();

    private bool _canBreak;
    private float _currentRayMaxDistance;
    private List<DestructibleStructure> _structuresSupportedBy;
    private List<DestructibleStructure> _structuresSupporting;
    
    private const float RAY_ANGLE = .5f;
    private const float FALLBACK_RAYCAST_DISTANCE = 1f;
    private const int ENABLE_WAIT_TIME = 5;

    
    protected override void Awake()
    {
        base.Awake();

        CopySupportLists();
        
        _currentRayMaxDistance = rayMaxDistance;
        
        WasDestroyed.AddListener(dissolveEffect.Dissolve);
    }
    
    private void OnEnable()
    {
        CoroutineCaller.Instance.StartCoroutine(IgnoreCollisionsCoroutine());
    }

    private IEnumerator IgnoreCollisionsCoroutine()
    {
        _canBreak = false;

        yield return new WaitForSeconds(ENABLE_WAIT_TIME);

        _canBreak = true;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (!_canBreak) return;
        
        var otherPhysicsObject = other.transform.GetComponent<PhysicsObject>();
        if (!otherPhysicsObject) return;
        
        DetectCollision(other.GetContact(0).point, otherPhysicsObject);
    }

    public override void PopulateParameters()
    {
        base.PopulateParameters();

        if (!dissolveEffect) dissolveEffect = GetComponent<DissolveEffect>();
        if (!physicsObject)
        {
            physicsObject = GetComponent<PhysicsObject>();
            if (!physicsObject) physicsObject = gameObject.AddComponent<PhysicsObject>();
        }
        
        physicsObject.PopulateParameters();
        
        InitializeSupport();
    }

    private void CopySupportLists()
    {
        _structuresSupporting = new List<DestructibleStructure>();
        foreach (var structure in structuresSupporting)
        {
            _structuresSupporting.Add(structure);
        }

        _structuresSupportedBy = new List<DestructibleStructure>();
        foreach (var structure in structuresSupportedBy)
        {
            _structuresSupportedBy.Add(structure);
        }
    }

    private void InitializeSupport()
    {
        CheckForSupport(Vector3.down);

        if (structuresSupportedBy.Count > 0) return;

        var vector = new Vector3(RAY_ANGLE, -1, 0);
        CheckForSupport(vector);

        if (structuresSupportedBy.Count > 0) return;

        vector = new Vector3(-RAY_ANGLE, -1, 0);
        CheckForSupport(vector);

        if (structuresSupportedBy.Count > 0) return;

        vector = new Vector3(RAY_ANGLE, -1, RAY_ANGLE);
        CheckForSupport(vector);

        if (structuresSupportedBy.Count > 0) return;

        vector = new Vector3(-RAY_ANGLE, -1, RAY_ANGLE);
        CheckForSupport(vector);

        if (structuresSupportedBy.Count > 0) return;

        vector = new Vector3(RAY_ANGLE, -1, -RAY_ANGLE);
        CheckForSupport(vector);

        if (structuresSupportedBy.Count > 0) return;

        vector = new Vector3(-RAY_ANGLE, -1, -RAY_ANGLE);
        CheckForSupport(vector);

        if (structuresSupportedBy.Count == 0 && _currentRayMaxDistance != FALLBACK_RAYCAST_DISTANCE)
        {
            _currentRayMaxDistance = FALLBACK_RAYCAST_DISTANCE;
            InitializeSupport();
        }
    }

    private void CheckForSupport(Vector3 vector)
    {
        var ray = new Ray(OriginalLocalPosition, vector);
        if (Physics.Raycast(ray, out var hit, _currentRayMaxDistance))
        {
            var hitRigid = hit.rigidbody;
            if (!hitRigid) return;

            var supportDestructible = hit.rigidbody.GetComponent<DestructibleStructure>();
            if (supportDestructible)
            {
                AddToStructuresSupportedBy(supportDestructible);
                if (structuresSupportedBy.Count == 0) return;
                
                supportDestructible.AddSupportingStructure(this);
            }
        }
    }

    private void AddToStructuresSupportedBy(DestructibleStructure supportDestructible)
    {
        if (supportDestructible.Equals(this)) return;
        
        if (structuresSupportedBy.Contains(supportDestructible)) return;
        if (structuresSupporting.Contains(supportDestructible)) return;
        
        structuresSupportedBy.Add(supportDestructible);
    }

    protected void AddSupportingStructure(DestructibleStructure destructibleStructure)
    {
        if (DebugLogger.IsNullError(destructibleStructure, this)) return;
        if (DebugLogger.IsNullError(structuresSupporting, this, "Must be set in editor.")) return;

        if (destructibleStructure.Equals(this)) return;

        if (structuresSupporting.Contains(destructibleStructure))
        {
            DebugLogger.Info(MethodBase.GetCurrentMethod().Name, $"{nameof(structuresSupporting)} already contains {nameof(destructibleStructure)}={destructibleStructure}.", this);
            return;
        }
        
        if (structuresSupportedBy.Contains(destructibleStructure))
        {
            DebugLogger.Info(MethodBase.GetCurrentMethod().Name, $"{nameof(structuresSupportedBy)} already contains {nameof(destructibleStructure)}={destructibleStructure}.", this);
            return;
        }
        
        structuresSupporting.Add(destructibleStructure);
    }

    [Button]
    public void Clear()
    {
        structuresSupporting.Clear();
        structuresSupportedBy.Clear();
    }

    public override void ResetObject() 
    {
        base.ResetObject();

        CopySupportLists();
    }

    [Button]
    public override void Demolish(Vector3 impactOrigin, float force)
    {
        CoroutineCaller.CachePerCall(CachedDemolish(impactOrigin, force));
    }

    private IEnumerator CachedDemolish(Vector3 impactOrigin, float force)
    {
        base.Demolish(impactOrigin, force);

        RemoveSelfFromSupportingStructures(impactOrigin, force);
        
        yield break;
    }

    private void RemoveSelfFromSupportingStructures(Vector3 impactOrigin, float force)
    {
        if (_structuresSupporting.Count == 0) return;
        
        foreach (var structure in _structuresSupporting)
        {
            if (DebugLogger.IsNullError(structure, this)) continue;

            structure.RemoveSupport(this, impactOrigin, force);
        }

        _structuresSupporting.Clear();
    }

    private void RemoveSupport(DestructibleStructure destructibleStructure, Vector3 impactOrigin,
        float force)
    {
        if (DebugLogger.IsNullError(destructibleStructure, this)) return;
        if (DebugLogger.IsNullOrEmptyError(_structuresSupportedBy, this)) return;
        
        if (!_structuresSupportedBy.Contains(destructibleStructure))
        {
            DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(_structuresSupportedBy)} does not contain {nameof(destructibleStructure)}={destructibleStructure}.", this);
            return;
        }

        _structuresSupportedBy.Remove(destructibleStructure);
        
        if (!IsSupported()) Demolish(impactOrigin, force);
    }

    private bool IsSupported()
    {
        return _structuresSupportedBy.Count > 0;
    }
    
    private void DetectCollision(Vector3 impactOrigin, PhysicsObject otherPhysicsObject)
    {
        var collisionSpeed = physicsObject.GetSpeed() + otherPhysicsObject.GetSpeed();
        if (collisionSpeed > collisionSpeedThresholdThreshold)
        {
            var combinedForce = otherPhysicsObject.GetForce() * physicsObject.GetForce();
            Demolish(impactOrigin, combinedForce);
        }
    }
}