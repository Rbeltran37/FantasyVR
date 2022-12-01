using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LaunchRigidbodyModifier : SkillModifier
{
    [SerializeField] private LaunchRigidbody launchRigidbody;
    
    
    public override void Apply(ModifierData modifierData)
    {
        if (DebugLogger.IsNullError(launchRigidbody, this, "Must be set in editor. Attempting to set.")) return;

        launchRigidbody.SetForce(modifierData.GetCurrentValue(ModifierTypeSo.ModifierTypeLookUp.Speed));
    }

    public override void Initialize()
    {
        if (DebugLogger.IsNullWarning(launchRigidbody, this, "Must be set in editor. Attempting to set."))
        {
            launchRigidbody = GetComponent<LaunchRigidbody>();
            if (DebugLogger.IsNullError(launchRigidbody, this, "Must be set in editor.")) return;
        }
    }
}
