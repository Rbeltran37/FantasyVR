using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : ArenaObject
{
    [SerializeField] private Transform pivotTransform;
    [SerializeField] private Transform openTransform;
    [SerializeField] private Transform closeTransform;
    [SerializeField] private float openSpeed;
    [SerializeField] private float closeSpeed;
    [SerializeField] private float tolerance = .01f;

    private bool _isMoving;
    
    
    public override IEnumerator Activate(Action callback)
    {
        if (_isMoving) yield break;

        _isMoving = true;

        yield return StartCoroutine(SlerpDoor(openTransform, openSpeed));

        _isMoving = false;
        
        callback?.Invoke();
    }

    public override IEnumerator Deactivate(Action callback)
    {
        if (_isMoving) yield break;

        _isMoving = true;

        yield return StartCoroutine(SlerpDoor(closeTransform, closeSpeed));

        _isMoving = false;
        
        callback?.Invoke();
    }

    public override void SetToActivatedPosition()
    {
        if (DebugLogger.IsNullError(pivotTransform, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(closeTransform, this, "Must be set in editor.")) return;
        
        pivotTransform.position = openTransform.position;
        pivotTransform.rotation = openTransform.rotation;
    }

    public override void SetToDeactivatedPosition()
    {
        if (DebugLogger.IsNullError(pivotTransform, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(closeTransform, this, "Must be set in editor.")) return;

        pivotTransform.position = closeTransform.position;
        pivotTransform.rotation = closeTransform.rotation;
    }

    public override void PopulateParameters()
    {
        if (!pivotTransform) pivotTransform = gameObject.transform;
    }

    private IEnumerator SlerpDoor(Transform targetTransform, float speed)
    {
        if (!targetTransform) yield break;

        var pivotPosition = pivotTransform.position;
        var pivotRotation = pivotTransform.rotation;
        var targetPosition = targetTransform.position;
        var targetRotation = targetTransform.rotation;
        var distance = Vector3.Distance(pivotPosition, targetPosition);
        var angle = Quaternion.Angle(pivotRotation, targetRotation);
        while (distance > tolerance ||
               angle > tolerance)
        {
            pivotPosition = pivotTransform.position;
            pivotRotation = pivotTransform.rotation;
            
            pivotTransform.position = Vector3.Slerp(pivotPosition, targetPosition, speed);
            pivotTransform.rotation = Quaternion.Slerp(pivotRotation, targetRotation, speed);
            
            distance = Vector3.Distance(pivotPosition, targetPosition);
            angle = Quaternion.Angle(pivotRotation, targetRotation);

            yield return new WaitForEndOfFrame();
        }

        pivotTransform.position = targetPosition;
        pivotTransform.rotation = targetRotation;
    }
}
