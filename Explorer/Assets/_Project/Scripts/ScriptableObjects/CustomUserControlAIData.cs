using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/CustomUserControlAIData", order = 1)]
public class CustomUserControlAIData : ScriptableObject
{
    public float stoppingDistance = 0.5f;
    public float stoppingThreshold = 1.5f;
    public float runSpeed = 1;
    public float walkStopDistance = .5f;
    public float walkSpeed = .5f;
    public float strafeSpeed = .7f;
    public float backUpRunSpeed = .8f;
    public float backUpWalkSpeed = .5f;
    public float accelerationSpeed = .1f;
    public float setMoveModeInterval = .5f;
    public float arriveAtAimTransformDistance = .1f;
    public float navDistanceBuffer = .1f;
}
