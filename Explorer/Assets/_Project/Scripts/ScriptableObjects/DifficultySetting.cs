using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Settings/DifficultySetting", order = 1)]
public class DifficultySetting : ScriptableObject
{
    [MinMaxFloatRange(0, 1)] public RangedFloat reactionTime;
    [MinMaxFloatRange(0, 3)] public RangedFloat timeBetweenActions;
    [Range(0, 3)] public float animatorSpeed = 1;
    [Range(0, 3)] public float healthMultiplier = 1;
}
