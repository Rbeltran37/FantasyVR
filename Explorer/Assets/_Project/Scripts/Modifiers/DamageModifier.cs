using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageModifier : SkillModifier
{
    [SerializeField] private DamageDealt damageDealt;
    
    
    public override void Apply(ModifierData modifierData)
    {
        if (DebugLogger.IsNullError(damageDealt, this, "Must be set in editor. Attempting to set.")) return;

        damageDealt.SetDamage(modifierData.GetCurrentValue(ModifierTypeSo));
    }

    public override void Initialize()
    {
        if (DebugLogger.IsNullWarning(damageDealt, this, "Must be set in editor. Attempting to set."))
        {
            damageDealt = GetComponent<DamageDealt>();
            if (DebugLogger.IsNullError(damageDealt, this, "Must be set in editor.")) return;
        }
    }
}
