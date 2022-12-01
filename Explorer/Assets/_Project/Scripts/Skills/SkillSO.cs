using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "_SkillSO", menuName = "ScriptableObjects/Skills/Skill", order = 1)]
public class SkillSO : ScriptableObject
{
    public string SkillName;
    public GameObject holsterModel;
    public GameObject holsterRing;
    public GameObject equipFx;
    public Sprite cooldownUiSprite;
    public Color uiColor;
    public bool usedByOtherHand;
    public bool isTwoHanded;
    public CastType castType = CastType.Instant;
    public SimpleAudioEvent castingAudioEvent;
    public float castingAudioInterval;
    public SimpleHapticEvent castingHapticEvent;
    [Range(0, 2)] public float castingHapticInterval;
    public SimpleAudioEvent castAudioEvent;
    public SimpleHapticEvent castHapticEvent;
    public ModifiableSO[] Modifiables;
    public ModifierTypeLookUp ModifierTypeLookUp;


    public enum CastType
    {
        Instant,
        Sorcery,
        Phased
    }

    public bool IsInstant()
    {
        return castType == CastType.Instant;
    }

    public bool IsSorcery()
    {
        return castType == CastType.Sorcery;
    }

    public bool IsPhased()
    {
        return castType == CastType.Phased;
    }

    public void PlayCastingHaptics(ControllerHaptics controllerHaptics)
    {
        if (DebugLogger.IsNullError(castingHapticEvent, this)) return;
        
        castingHapticEvent.Play(controllerHaptics);
    }
    
    public void PlayCastingAudio(AudioSource audioSource)
    {
        if (DebugLogger.IsNullError(castingAudioEvent, this)) return;

        castingAudioEvent.Play(audioSource);
    }
    
    public void PlayCastHaptics(ControllerHaptics controllerHaptics)
    {
        if (DebugLogger.IsNullError(castHapticEvent, this)) return;

        castHapticEvent.Play(controllerHaptics);
    }
    
    public void PlayCastAudio(AudioSource audioSource)
    {
        if (DebugLogger.IsNullError(castAudioEvent, this)) return;

        castAudioEvent.Play(audioSource);
    }
}
