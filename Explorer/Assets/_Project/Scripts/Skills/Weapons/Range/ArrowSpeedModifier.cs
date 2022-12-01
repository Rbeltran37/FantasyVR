using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpeedModifier : SkillModifier
{
    [SerializeField] private BowInstance bowInstance;
    
    
    public override void Apply(ModifierData modifierData)
    {
        if (DebugLogger.IsNullError(bowInstance, this, "Must be set in editor.")) return;

        bowInstance.SetSpeed(modifierData.GetCurrentValue(ModifierTypeSo.ModifierTypeLookUp.Speed));
    }

    public override void Initialize()
    {
        if (DebugLogger.IsNullWarning(bowInstance, this, "Must be set in editor. Attempting to set."))
        {
            var parent = transform.parent;
            if (parent) bowInstance = parent.GetComponentInChildren<BowInstance>();
            if (!bowInstance) GetComponentInChildren<BowInstance>();
            if (DebugLogger.IsNullError(bowInstance, this, "Must be set in editor.")) return;
        }
    }
}
