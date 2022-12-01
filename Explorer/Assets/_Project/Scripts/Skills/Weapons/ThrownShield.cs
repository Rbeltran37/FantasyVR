using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RootMotion.Dynamics;
using UnityEngine;

public class ThrownShield : ThrownWeapon
{
    [SerializeField] private LaunchRigidbody launchRigidbody;
    [SerializeField] private RotateRigidbody rotateRigidbody;
    [SerializeField] private float torqueMultiplier = .2f;
    [SerializeField] private float bounceRange = 7;
    [SerializeField] private LayerMask bounceLayerMask;
    [SerializeField] private QueryTriggerInteraction bounceQueryTriggerInteraction = QueryTriggerInteraction.Collide;
    
    private float _time;
    private float _lifetime;
    private int _numHitsRemaining;
    private Vector3 _returnOrigin;
    private Transform _curvePoint;
    private Transform _bounceTarget;
    private Health _lastOwnerHit;
    
    private const float COLLISION_BUFFER_NUMERATOR = 1;
    private const int DOWNWARD_ROTATION = -90;
    private const int HIPS = 0;
    private const int LIFETIME = 2;
    

    protected override void ResetWeapon()
    {
        base.ResetWeapon();
        
        IsStuck = false;
        
        _lifetime = LIFETIME;
        _numHitsRemaining = 0;
        ThrowSpeed = 0;
        _time = 0;

        _returnOrigin = Vector3.zero;
        
        _curvePoint = null;
        _bounceTarget = null;
        _lastOwnerHit = null;
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (IsStuck) return;
        
        var damageReceived = other.collider.GetComponent<DamageReceived>();
        if (damageReceived)
        {
            RegisterHit(damageReceived);

            if (_numHitsRemaining <= 0)
            {
                StartReturn();
                return;
            }
            
            AttemptBounce();
            return;
        }

        if (IsInLayerMask(other.gameObject, StickLayers))
        {
            Stick();
        }
        
        base.OnCollisionEnter(other);
    }

    private void FixedUpdate()
    {
        if (IsStuck) return;
        
        if (!IsReturning)
        {
            _lifetime -= Time.deltaTime;
            if (_lifetime < 0)
            {
                StartReturn();
            }
            
            return;
        }
        
        var distance = Vector3.Distance(ThisTransform.position, ReturnTarget.position);
        if (HasReachedTarget(distance))
        {
            FinishReturn();
            return;
        }
        
        CurvedReturn();
    }

    private void RegisterHit(DamageReceived damageReceived)
    {
        var owner = damageReceived.GetOwner();
        if (!_lastOwnerHit || !_lastOwnerHit.Equals(owner))
        {
            _numHitsRemaining--;
            _lastOwnerHit = owner;
        }
    }
    
    public override void StartReturn()
    {
        base.StartReturn();

        _returnOrigin = ThisTransform.position;
        
        SetTriggers(true);
        RotateRigidbody(true);
    }

    private void RotateRigidbody(bool isReversed)
    {
        if (isReversed) rotateRigidbody.SetTorqueSpeed(-ThrowSpeed * torqueMultiplier);
        rotateRigidbody.RotateRigid();
    }

    private void RecoverFromBounce()
    {
        SetTriggers(false);
        RotateRigidbody(false);
    }

    private void AttemptBounce()
    {
        _bounceTarget = FindTarget();
        if (!_bounceTarget)
        {
            StartReturn();
            return;
        }
        
        BounceThrow(_bounceTarget);
    }

    private Transform FindTarget()
    {
        var origin = transform.position;
        var collidersHit = Physics.OverlapSphere(origin, bounceRange, bounceLayerMask, bounceQueryTriggerInteraction);
        var nearestDistance = Single.MaxValue;
        Transform nearestTarget = null;
        foreach (var colliderHit in collidersHit)
        {
            var potentialTarget = colliderHit.transform;
            var damageReceived = potentialTarget.GetComponent<DamageReceived>();
            var targetOwner = damageReceived.GetOwner();
            if (targetOwner.Equals(_lastOwnerHit)) continue;
            if (!targetOwner.isAlive) continue;

            var distance = colliderHit.ClosestPoint(origin).magnitude;
            var muscleCollisionBroadcaster = colliderHit.GetComponent<MuscleCollisionBroadcaster>();
            if (muscleCollisionBroadcaster)
            {
                nearestTarget = muscleCollisionBroadcaster.puppetMaster.muscles[HIPS].target;
                continue;
            }

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = potentialTarget;
            }
        }
        
        return nearestTarget;
    }
    
    private void BounceThrow(Transform target)
    {
        StopRigidbody();
        SetTriggers(true);

        var direction = target.position - ThisTransform.position;
        direction = direction.normalized;
        launchRigidbody.Launch(direction);
        
        ThisTransform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(DOWNWARD_ROTATION, 0, 0);

        ThrowAudioEvent.Play(AudioSource);
        
        CoroutineCaller.Instance.StartCoroutine(EnableTriggerCoroutine());
    }

    private IEnumerator EnableTriggerCoroutine()
    {
        yield return new WaitForSeconds(COLLISION_BUFFER_NUMERATOR/ThrowSpeed);
        RecoverFromBounce();
    }

    private void CurvedReturn()
    {
        _time += Time.deltaTime;
        ThisTransform.position =
            VectorMath.GetBezierQuadraticCurvePoint(_returnOrigin, _curvePoint.position, ReturnTarget.position, _time);
    }

    public void SetSpeed(float value)
    {
        ThrowSpeed = value;
        
        launchRigidbody.SetForce(value);
        rotateRigidbody.SetTorqueSpeed(value * torqueMultiplier);
    }

    public void SetNumHitsRemaining(int value)
    {
        _numHitsRemaining = value;
    }

    public void SetCurvePoint(Transform curvePoint)
    {
        _curvePoint = curvePoint;
    }
}
