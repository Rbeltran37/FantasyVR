using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/HealthData", order = 1)]
public class HealthData : ScriptableObject
{
    public float maxHealth = 1000;
    [Range(0, 2)] public float hitInvulnerabilityTime = 0;
}
