using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_ForwardSphereCast_TargetAcquisition", menuName = "ScriptableObjects/TargetAcquisition/ForwardSphereCast", order = 1)]
public class ForwardSphereCast : TargetAcquisition
{
    [SerializeField] private float sphereRadius = 4;
    

    public override Transform AcquireTarget(Transform transform)
    {
        if (!transform) return null;

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, 
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
        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, 
            Range, TargetLayerMask, QueryTriggerInteraction))
        {
            return hit.point;
        }

        return Vector3.zero;    
    }
}
