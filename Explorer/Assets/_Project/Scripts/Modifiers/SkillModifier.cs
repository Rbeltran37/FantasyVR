using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillModifier : MonoBehaviour
{
    public ModifierTypeSO ModifierTypeSo;
    
    public abstract void Apply(ModifierData modifierData);
    public abstract void Initialize();
}
