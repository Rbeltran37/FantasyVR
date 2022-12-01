using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_FootstepsSO", menuName = "ScriptableObjects/CharacterData/FootstepsSO", order = 1)]
public class FootstepsSO : ScriptableObject
{
    public LayerMask GroundLayers;
    public float DistanceToFloor = .05f;
    public float CooldownTime = .15f;
}
