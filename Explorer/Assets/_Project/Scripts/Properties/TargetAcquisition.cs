using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetAcquisition : ScriptableObject
{
    [SerializeField] protected float Range = 10;
    [SerializeField] protected LayerMask TargetLayerMask;
    [SerializeField] protected QueryTriggerInteraction QueryTriggerInteraction = QueryTriggerInteraction.Collide;
    
    
    public abstract Transform AcquireTarget(Transform transform);
    public abstract Vector3 AcquireHitPoint(Transform transform);
    

    public  float GetRange()
    {
        return Range;
    }
}
