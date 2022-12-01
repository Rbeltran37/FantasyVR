using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ForwardRayCast_TargetAcquisition", menuName = "ScriptableObjects/TargetAcquisition/ForwardRayCast", order = 1)]
public class ForwardRayCast : TargetAcquisition
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

        return Vector3.zero;;
    }
}