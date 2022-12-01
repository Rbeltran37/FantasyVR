using System;
using System.Reflection;
using UnityEngine;

public class AbilityModifier : SkillModifier
{
    [SerializeField] protected SkillAbility skillAbility;
    
    
    public override void Apply(ModifierData modifierData)
    {
        if (DebugLogger.IsNullError(modifierData, this)) return;
        if (DebugLogger.IsNullError(skillAbility, this)) return;

        var level = modifierData.GetCurrentLevel(ModifierTypeSo);
        var value = modifierData.GetCurrentValue(ModifierTypeSo);
        skillAbility.SetLevel(level);
        skillAbility.SetValue(value);
    }

    public override void Initialize()
    {
        if (DebugLogger.IsNullWarning(skillAbility, "Should be set in editor. Attempting to set.", this))
        {
            skillAbility = GetComponent<SkillAbility>();
            if (DebugLogger.IsNullError(skillAbility, this)) return;
        }
    }
}
