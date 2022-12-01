using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimatorHitReactionHandler : MonoBehaviour
{
    [SerializeField] private AnimatorHitReactionHandlerData animatorHitReactionHandlerData;
    [SerializeField] private Animator animator;

    private const int FORWARD_ANGLE = 45;
    private const int SIDE_ANGLE = 135;
    
    public Action HeadWasHit;
    
    
    private void Awake()
    {
        if (DebugLogger.IsNullError(animatorHitReactionHandlerData, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullWarning(animator, this, "Should be set in editor. Attempting to find..."))
        {
            var colliderParent = transform.parent;
            if (colliderParent)
            {
                var puppetParent = colliderParent.parent;
                if (puppetParent)
                {
                    animator = GetComponentInChildren<Animator>();
                }
            }
            
            if (DebugLogger.IsNullError(animator, this, "Should be set in editor. Was not found.")) return;
        }
    }

    public void SetHitReaction(int muscleIndex, float damage, Vector3 referenceForward, Vector3 referencePoint, Vector3 hitPoint)
    {
        animatorHitReactionHandlerData.SetAnimation(animator, muscleIndex, damage, referenceForward, referencePoint, hitPoint);
    }

    [Button]
    private void PrintAngle(Transform transformHit, Vector3 hitPoint)
    {
        var forwardVector = transform.forward;
        var vector1 = new Vector2(forwardVector.x, forwardVector.z);
        var headingOfHit = VectorMath.GetTargetHeading(transformHit, hitPoint);
        var vector2 = new Vector2(headingOfHit.x, headingOfHit.z);
        var angle = VectorMath.AngleBetweenVector2(vector1, vector2);
        
        if (angle < FORWARD_ANGLE && angle > -FORWARD_ANGLE)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"angle={angle} is a FRONT hit.", this);
        }
        else if (angle > FORWARD_ANGLE && angle < SIDE_ANGLE)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"angle={angle} is a LEFT hit.", this);
        }
        else if (angle < -FORWARD_ANGLE && angle > -SIDE_ANGLE)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"angle={angle} is a RIGHT hit.", this);
        }
        else
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"angle={angle} is a BACK hit.", this);
        }
    }
    
    [Button]
    private void PrintAngle(Transform transformHit, Transform hitTransform)
    {
        var forwardVector = transform.forward;
        var vector1 = new Vector2(forwardVector.x, forwardVector.z);
        var headingOfHit = VectorMath.GetTargetHeading(transformHit, hitTransform.position);
        var vector2 = new Vector2(headingOfHit.x, headingOfHit.z);
        var angle = VectorMath.AngleBetweenVector2(vector1, vector2);
        
        if (angle < FORWARD_ANGLE && angle > -FORWARD_ANGLE)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"angle={angle} is a FRONT hit.", this);
        }
        else if (angle > FORWARD_ANGLE && angle < SIDE_ANGLE)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"angle={angle} is a LEFT hit.", this);
        }
        else if (angle < -FORWARD_ANGLE && angle > -SIDE_ANGLE)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"angle={angle} is a RIGHT hit.", this);
        }
        else
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"angle={angle} is a BACK hit.", this);
        }
    }
}
