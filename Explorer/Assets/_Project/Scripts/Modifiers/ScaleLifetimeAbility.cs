using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class ScaleLifetimeAbility : SkillAbility
{
    [SerializeField] private SkillInstance skillInstance;

    
    private void OnEnable()
    {
        StartCoroutine(DelayedActivation());
    }
    
    private IEnumerator DelayedActivation()
    {
        yield return null;
        
        Activate();
    }

    [Button]
    protected override void Activate()
    {
        if (DebugLogger.IsNullError(skillInstance, this, "Must be set in editor.")) return;

        if (Math.Abs(Value - NOT_APPLIED) < Mathf.Epsilon) return;

        skillInstance.Despawn(Value);
    }
}
