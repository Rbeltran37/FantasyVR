using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillInstance : PooledObject
{
    [SerializeField] protected AudioSource SkillAudioSource;
    [SerializeField] protected SkillModifierContainer skillModifierContainer;

    protected Skill Skill;
    
    
    public virtual void SetSkill(Skill skill)
    {
        Skill = skill;
    }

    public Skill GetSkill()
    {
        return Skill;
    }

    public virtual void ApplyModifiers(ModifierData modifierData)
    {
        if (DebugLogger.IsNullError(skillModifierContainer, this, "Must be set in editor.")) return;
        
        skillModifierContainer.ApplyModifiers(modifierData);
    }
}
