using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/CustomCharacterAnimationThirdPersonData", order = 1)]
public class CustomCharacterAnimationThirdPersonSO : ScriptableObject
{
    public float turnSensitivity = 0.2f; // Animator turning sensitivity
    public float turnSpeed = 5f; // Animator turning interpolation speed
    public float runCycleLegOffset = 0.2f; // The offset of leg positions in the running cycle
    public float animSpeedMultiplier = 1;
}
