using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/PuppetHeadHitReactionData", order = 1)]
public class PuppetHeadHitReactionData : ScriptableObject
{
    [Range(0, .1f)] public float headDistanceLimit = .05f;
    [Range(0, .1f)] public float headRegainPinDistance = .03f;
    [Range(0, .5f)] public float headHitRegain = 0;
    [Range(0, 10)] public float defaultHeadRegain = 2;
    [Range(0, .5f)] public float activationBufferTime = .05f;
}