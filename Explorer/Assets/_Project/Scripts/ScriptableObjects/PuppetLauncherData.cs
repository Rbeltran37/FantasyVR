using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/PuppetLauncherData", order = 1)]
public class PuppetLauncherData : ScriptableObject
{
    public CharacterAudioManager characterAudioManager;
    public float hitForceMultiplier = 5;
    [MinMaxFloatRange(100, 5000)] public RangedFloat launchForce;
    public float liftForce = 10;
    
    public float launchDelay = .1f;
}
