using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_Modifiable", menuName = "ScriptableObjects/Modifiers/Modifiable", order = 1)]
public class ModifiableSO : ScriptableObject
{
    public ModifierTypeSO ModifierTypeSo;
    public float MinValue;
    public float MaxValue;
    [Range(MIN_LEVEL, MAX_LEVEL)] public int BaseLevel = DEFAULT_LEVEL;
    public bool ReverseModifyLevel;

    public float Increment => Range / MIN_MAX_DIFFERENCE;
    
    private float Range => MaxValue - MinValue;

    private const int MIN_LEVEL = 0;
    private const int MAX_LEVEL = 12;
    private const int DEFAULT_LEVEL = 3;
    private const int MIN_MAX_DIFFERENCE = MAX_LEVEL - MIN_LEVEL;
}
