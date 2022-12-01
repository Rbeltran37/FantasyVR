using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Events;

public class ImpactBlast : PooledSkillAbility
{
    [SerializeField] private ForceFunctions forceFunctions;


    protected override void OnEnable()
    {
        base.OnEnable();

        SetScale();
        SetRange();
        Explode();
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        
        ResetAbility();
    }

    private void SetScale()
    {
        var adjustedScale = new Vector3(Value, Value, Value);
        ThisTransform.localScale = adjustedScale;
    }
    
    private void SetRange()
    {
        forceFunctions.SetRange(Value);
    }

    private void Explode()
    {
        forceFunctions.ForceRepulse();
    }

    private void ResetAbility()
    {
        forceFunctions.ResetValues();
    }
}
