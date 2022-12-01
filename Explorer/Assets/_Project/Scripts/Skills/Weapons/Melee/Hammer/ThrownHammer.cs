using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Events;
using Zinnia.Tracking.Velocity;

public class ThrownHammer : ThrownWeapon
{
    [SerializeField] private AverageVelocityEstimator averageVelocityEstimator;
    [SerializeField] private float slerpTime = .5f;
    
    private float _adjustedSlerp;
    private float _decelerationTimer;
    private float _lifetime;
    private Vector3 _rotationAdjustment = new Vector3(90, 0, 0);
    
    private const float DECELERATION_TIME = 1.5f;
    private const int LIFETIME = 3;


    protected override void OnCollisionEnter(Collision other)
    {
        if (IsReturning) return;
        if (IsStuck) return;
        if (IsInLayerMask(other.gameObject, StickLayers))
        {
            Stick();
            return;
        }
        
        base.OnCollisionEnter(other);
    }
    
    private void FixedUpdate()
    {
        if (!IsThrown) return;

        var distance = Vector3.Distance(ThisTransform.position, ReturnTarget.position);
        var isNearTarget = IsNearTarget(distance);
        if (!IsStuck && !isNearTarget) SlerpTowardsVelocity();
        
        if (IsReturning)
        {
            AccelerationReturn();
            
            if (HasReachedTarget(distance))
            {
                FinishReturn();
                return;
            }

            if (isNearTarget) SlerpToReturnRotation(distance);
        }
        
        if (!IsReturning)
        {
            _lifetime -= Time.deltaTime;
            if (_lifetime < 0)
            {
                StartReturn();
            }
            
            return;
        }
    }

    public override void SetThrowSpeed(float speed)
    {
        base.SetThrowSpeed(speed);
        
        _adjustedSlerp = slerpTime * ThrowSpeed;
    }

    protected override void ResetWeapon()
    {
        base.ResetWeapon();
        
        _decelerationTimer = DECELERATION_TIME;
        _lifetime = LIFETIME;
    }
    
    public override void StartReturn()
    {
        IsReturning = true;

        if (IsStuck) _decelerationTimer = 0;
        IsStuck = false;
        
        PlayThrowFx();
    }

    private void SlerpTowardsVelocity()
    {
        var velocity = averageVelocityEstimator.GetVelocity();
        var velocityRotation = Quaternion.LookRotation(velocity.normalized) * Quaternion.Euler(_rotationAdjustment);
        ThisTransform.rotation = Quaternion.Slerp(ThisTransform.rotation, velocityRotation, _adjustedSlerp * Time.deltaTime);
    }

    private void AccelerationReturn()
    {
        if (_decelerationTimer > 0)
        {
            Decelerate();
            
            _decelerationTimer -= Time.deltaTime;
            if (_decelerationTimer < 0) StopRigidbody();
            
            return;
        }
        
        MoveTowardsReturn();
    }

    private void Decelerate()
    {
        var direction = ReturnTarget.position - ThisTransform.position;
        ThisRigidbody.AddForce(direction.normalized * ThrowSpeed, ForceMode.Acceleration);
    }

    private void MoveTowardsReturn()
    {
        var currentPosition = ThisTransform.position;
        var returnTargetPosition = ReturnTarget.position;
        
        // Move our position a step closer to the target.
        float step = ThrowSpeed * Time.deltaTime; // calculate distance to move
        ThisTransform.position = Vector3.MoveTowards(currentPosition, returnTargetPosition, step);
    }

    private void SlerpToReturnRotation(float distance)
    {
        ThisTransform.rotation = Quaternion.Slerp(ThisTransform.rotation, ReturnTarget.rotation,
            (_adjustedSlerp / distance) * Time.deltaTime);
    }
}
