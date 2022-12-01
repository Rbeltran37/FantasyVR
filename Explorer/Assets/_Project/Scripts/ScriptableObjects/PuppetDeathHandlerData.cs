using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/PuppetDeathHandlerData", order = 1)]
public class PuppetDeathHandlerData : ScriptableObject
{
    public float deathVelocity = 0.3f;
    [MinMaxFloatRange(0, 10)] public RangedFloat turnOffTime;
    public float checkForDeathInterval = .51f;
}
