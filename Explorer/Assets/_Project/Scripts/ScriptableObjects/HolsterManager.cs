using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HolsterManager", menuName = "ScriptableObjects/CharacterData/HolsterManager", order = 1)]
public class HolsterManager : ScriptableObject
{
    [Header("HolsterSetupHelper")]
    public SkillContainer leftWaistSkill;
    public SkillContainer rightWaistSkill;
    public SkillContainer leftBackSkill;
    public SkillContainer rightBackSkill;
    
    [Header("HolsterPositioner")]
    [Range(0, 2)] public float waistDistanceFromHeadset = 1.1f;
    [Range(0, 1)] public float waistDistanceFromCenter = .6f;
    [Range(0, 1)] public float backDistanceFromHeadset = .68f;
    [Range(0, 1)] public float backDistanceFromCenter = .62f;

    [Header("HolsterFeedbackHandler")] 
    public SimpleHapticEvent EnterHapticEvent;
    public SimpleHapticEvent EquipHapticEvent;
    
    [Header("HolsterCooldown")]
    public AudioClip cooldownClip;
    
    [Header("Cooldown UI Object")]
    public float filledBufferTime = 1;
    
    [Header("SkillCount")]
    public AudioClip emptyClip;
    public AudioClip invalidClip;
    [Range(1, 10)] public float ringLaunchForce = 4;
    [Range(.1f, 1)] public float ringLife = .3f;
    [Range(0, 1)] public float startSize = .1f;
    [Range(0, .1f)] public float sizeIncrement = .02f;
    [Range(0, 1)] public float timeIncrement = .2f;

    public void PlayEnterHolsterHaptics(ControllerHaptics controllerHaptics)
    {
        if (DebugLogger.IsNullError(EnterHapticEvent, this, "Must be set in editor.")) return;

        EnterHapticEvent.Play(controllerHaptics);
    }
    
    public void PlayEquipHaptics(ControllerHaptics controllerHaptics)
    {
        if (DebugLogger.IsNullError(EquipHapticEvent, this, "Must be set in editor.")) return;

        EquipHapticEvent.Play(controllerHaptics);
    }
}
