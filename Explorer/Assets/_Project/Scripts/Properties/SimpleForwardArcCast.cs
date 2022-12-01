using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Not Working Properly
[CreateAssetMenu(fileName = "SimpleForwardArcCast_TargetAcquisition", menuName = "ScriptableObjects/TargetAcquisition/SimpleForwardArcCast", order = 1)]
public class SimpleForwardArcCast : TargetAcquisition
{
    public override Transform AcquireTarget(Transform transform)
    {
        if (!transform) return null;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 
            Range, TargetLayerMask, QueryTriggerInteraction))
        {
            return hit.transform;
        }

        var forwardVector = transform.forward * Range;
        if (Physics.Raycast(forwardVector, Vector3.down, out hit, 
            Range, TargetLayerMask, QueryTriggerInteraction))
        {
            return hit.transform;
        }

        return null;
    }

    public override Vector3 AcquireHitPoint(Transform transform)
    {
        if (!transform) return Vector3.zero;
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 
            Range, TargetLayerMask, QueryTriggerInteraction))
        {
            return hit.point;
        }
        
        var forwardVector = transform.forward * Range;
        if (Physics.Raycast(forwardVector, Vector3.down, out hit, 
            Range, TargetLayerMask, QueryTriggerInteraction))
        {
            return hit.point;
        }

        return Vector3.zero;;
    }
}
