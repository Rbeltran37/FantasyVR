using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierTypeLookUp", menuName = "ScriptableObjects/Modifiers/ModifierTypeLookUp", order = 1)]
public class ModifierTypeLookUp : ScriptableObject
{
    public ModifierTypeSO Cooldown;
    public ModifierTypeSO Cost;
    public ModifierTypeSO Count;
    public ModifierTypeSO Power;
    public ModifierTypeSO Speed;
    public ModifierTypeSO Ability1;
    public ModifierTypeSO Ability2;
}
