using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_OverlapSphereNearestTarget_TargetAcquisition", menuName = "ScriptableObjects/TargetAcquisition/OverlapSphereNearestTarget", order = 1)]
public class OverlapSphereNearestTarget : TargetAcquisition
{
    public override Transform AcquireTarget(Transform transform)
    {
        if (!transform) return null;

        var origin = transform.position;
        var collidersHit = Physics.OverlapSphere(origin, Range, TargetLayerMask, QueryTriggerInteraction);
        var nearestDistance = Single.MaxValue;
        Transform nearestTarget = null;
        foreach (var collider in collidersHit)
        {
            var distance = collider.ClosestPoint(origin).magnitude;
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = collider.transform;
            }
        }
        
        return nearestTarget;
    }

    public override Vector3 AcquireHitPoint(Transform transform)
    {
        if (!transform) return Vector3.zero;

        var origin = transform.position;
        var collidersHit = Physics.OverlapSphere(origin, Range, TargetLayerMask, QueryTriggerInteraction);
        var nearestDistance = Single.MaxValue;
        Vector3 nearestTarget = Vector3.zero;
        foreach (var collider in collidersHit)
        {
            var distance = collider.ClosestPoint(origin).magnitude;
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = collider.transform.position;
            }
        }
        
        return nearestTarget;
    }
}
