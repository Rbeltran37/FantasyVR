using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierType", menuName = "ScriptableObjects/Modifiers/ModifierType", order = 1)]
public class ModifierTypeSO : ScriptableObject
{
    public string Name;
    public Color Color;
    public ModifierTypeLookUp ModifierTypeLookUp;
}
