using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillModifierContainer : MonoBehaviour
{
    [SerializeField] private List<SkillModifier> skillModifiers = new List<SkillModifier>();
    

    [Button]
    private void Initialize()
    {
        skillModifiers = new List<SkillModifier>();
        var tempSkillModifiers = GetComponents<SkillModifier>();
        foreach (var skillModifier in tempSkillModifiers)
        {
            if (!skillModifiers.Contains(skillModifier))
            {
                skillModifier.Initialize();
                skillModifiers.Add(skillModifier);
            }
        }
        
        tempSkillModifiers = GetComponentsInChildren<SkillModifier>();
        foreach (var skillModifier in tempSkillModifiers)
        {
            if (!skillModifiers.Contains(skillModifier))
            {
                skillModifier.Initialize();
                skillModifiers.Add(skillModifier);
            }
        }
    }

    public void ApplyModifiers(ModifierData modifierData)
    {
        if (DebugLogger.IsNullError(modifierData, this)) return;
        
        foreach (var skillModifier in skillModifiers)
        {
            skillModifier.Apply(modifierData);
        }
    }
}
