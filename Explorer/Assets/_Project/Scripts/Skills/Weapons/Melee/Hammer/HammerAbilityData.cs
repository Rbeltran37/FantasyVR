using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class HammerAbilityData : WeaponAbilityData
{
    public float CastingSpeed => _modifierData.GetCurrentValue(Skill.SkillSo.ModifierTypeLookUp.Speed);
    public float Lifetime => _modifierData.GetCurrentValue(Skill.SkillSo.ModifierTypeLookUp.Ability1);
    
    private ModifierData _modifierData;
    
    public HammerAbilityData(HammerInstance hammerInstance) : base(hammerInstance)
    {
        var skill = hammerInstance.GetSkill();
        if (!skill)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(skill)} is null.");
            return;
        }
        
        _modifierData = skill.ModifierData;
    }
}
