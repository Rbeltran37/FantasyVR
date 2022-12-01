using System;
using UnityEngine;
using System.Collections;
using RootMotion.Demos;
using Sirenix.OdinInspector;

/// <summary>
/// User input for an AI controlled character controller.
/// </summary>
public class CustomUserControlAI : CustomUserControlThirdPerson {

    public CustomUserControlAIData customUserControlAiData;
    public CustomCharacterPuppet customCharacterPuppet;
    public Transform navTarget;
    public Transform aimTarget;
    public Navigator navigator;

    private bool _settingMovement;
    
    
    protected override void Start()
    {
        base.Start();

        if (DebugLogger.IsNullError(navigator, this, "Must be set in editor.")) return;

        navigator.Initiate(transform);
    }

    public void StopMoving()
    {
        MoveAwayFromTarget(0);
    }
    
    public void RunTowardsTarget()
    {
        if (DebugLogger.IsNullError(customUserControlAiData, this, "Must be set in editor.")) return;
        
        MoveTowardsTarget(customUserControlAiData.runSpeed);
    }
    
    public void WalkTowardsTarget()
    {
        if (DebugLogger.IsNullError(customUserControlAiData, this, "Must be set in editor.")) return;
        
        MoveTowardsTarget(customUserControlAiData.walkSpeed);
    }

    public void WalkAwayFromTarget()
    {
        if (DebugLogger.IsNullError(customUserControlAiData, this, "Must be set in editor.")) return;

        MoveAwayFromTarget(customUserControlAiData.backUpWalkSpeed);
    }

    public void RunAwayFromTarget()
    {
        if (DebugLogger.IsNullError(customUserControlAiData, this, "Must be set in editor.")) return;

        MoveAwayFromTarget(customUserControlAiData.backUpRunSpeed);
    }

    public void Strafe(float moveSpeed, bool isStrafingLeft)
    {
        if (!navTarget) return;
        
        if (DebugLogger.IsNullError(customCharacterPuppet, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(customUserControlAiData, this, "Must be set in editor.")) return;

        customCharacterPuppet.moveMode = CustomCharacterThirdPerson.MoveMode.Strafe;
        customCharacterPuppet.IsMovingBackward = false;
        
        Vector3 direction = navTarget.position - transform.position;
        if (isStrafingLeft)
        {
	        direction = Quaternion.Euler(0, -90, 0) * direction;
        }
        else
        {
	        direction = Quaternion.Euler(0, 90, 0) * direction;
        }
        
        float distance = direction.magnitude;
        Vector3 normal = transform.up;
        Vector3.OrthoNormalize(ref normal, ref direction);

        float sD = state.move != Vector3.zero ? customUserControlAiData.stoppingDistance : 
            customUserControlAiData.stoppingDistance * customUserControlAiData.stoppingThreshold;

        state.move = distance > sD ? direction * moveSpeed : Vector3.zero;
        state.move = direction * moveSpeed;
        state.lookPos = navTarget.position;
    }

    public void Avoid(GameObject avoidTarget)
    {
        if (!avoidTarget) return;
        if (!navTarget) return;

        customCharacterPuppet.IsMovingBackward = false;
        
        Vector3 direction = navTarget.position - transform.position;
        if (Math.Abs(VectorMath.IsTargetToTheLeftOrRight(direction, avoidTarget.transform) - 1) < Mathf.Epsilon)
        {
            direction = Quaternion.Euler(0, -45, 0) * direction;
        }
        else
        {
            direction = Quaternion.Euler(0, 45, 0) * direction;
        }
        float distance = direction.magnitude;

        Vector3 normal = transform.up;
        Vector3.OrthoNormalize(ref normal, ref direction);

        float sD = state.move != Vector3.zero ? customUserControlAiData.stoppingDistance : 
            customUserControlAiData.stoppingDistance * customUserControlAiData.stoppingThreshold;

        state.move = distance > sD ? direction * customUserControlAiData.runSpeed : Vector3.zero;
        state.lookPos = navTarget.position;
    }
    
    public float GetDistance()
    {
        if (DebugLogger.IsNullError(navTarget, this, "Must be set in editor.")) return 0f;

        return Vector3.Distance(navTarget.position, transform.position);
    }

    public void DirectionalMode()
    {
        if (DebugLogger.IsNullError(customCharacterPuppet, this, "Must be set in editor.")) return;

        customCharacterPuppet.moveMode = CustomCharacterThirdPerson.MoveMode.Directional;
    }

    public void StrafeMode()
    {
        if (DebugLogger.IsNullError(customCharacterPuppet, this, "Must be set in editor.")) return;

        customCharacterPuppet.moveMode = CustomCharacterThirdPerson.MoveMode.Strafe;
    }
    
    private void MoveTowardsTarget(float moveSpeed)
    {
        if (!navTarget) return;

        if (DebugLogger.IsNullError(customCharacterPuppet, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(navTarget, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(customUserControlAiData, this, "Must be set in editor.")) return;

        customCharacterPuppet.IsMovingBackward = false;
        
        Vector3 directionToNavTarget = navTarget.position - transform.position;
        float navDistance = directionToNavTarget.magnitude;

        Vector3 normal = transform.up;
        Vector3.OrthoNormalize(ref normal, ref directionToNavTarget);

        float sD = state.move != Vector3.zero ? customUserControlAiData.walkStopDistance : 
            customUserControlAiData.walkStopDistance * customUserControlAiData.stoppingThreshold;

        if (Vector3.Distance(navTarget.position, aimTarget.position) < customUserControlAiData.arriveAtAimTransformDistance)
        {
            state.move = navDistance > sD ? directionToNavTarget * moveSpeed : Vector3.zero;
        }
        else
        {
            state.move = directionToNavTarget * moveSpeed;
        }
        
        state.lookPos = navTarget.position;
    }

    private void MoveAwayFromTarget(float moveSpeed)
    {
        if (!navTarget) return;

        if (DebugLogger.IsNullError(customCharacterPuppet, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(navTarget, this, "Must be set in editor.")) return;

        customCharacterPuppet.IsMovingBackward = true;
        
        Vector3 direction = navTarget.position - transform.position;
        direction *= -1;
        float distance = direction.magnitude;

        Vector3 normal = transform.up;
        Vector3.OrthoNormalize(ref normal, ref direction);

        float sD = state.move != Vector3.zero ? customUserControlAiData.stoppingDistance : 
            customUserControlAiData.stoppingDistance * customUserControlAiData.stoppingThreshold;

        state.move = direction * moveSpeed;
        state.lookPos = navTarget.position;
    }

    // Visualize the navigator
    void OnDrawGizmos()
    {
        if (navigator.activeTargetSeeking) navigator.Visualize();
    }
}

