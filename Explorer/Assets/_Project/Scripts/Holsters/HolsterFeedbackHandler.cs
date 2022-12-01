using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterFeedbackHandler : MonoBehaviour
{
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private HolsterAudioManager holsterAudioManager;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private HolsterCollider holsterCollider;
    [SerializeField] private GameObject inHolsterUi;
    
    [SerializeField] public ControllerHaptics leftControllerHaptics;
    [SerializeField] public ControllerHaptics rightControllerHaptics;
    

    private void Awake()
    {
        holsterCollider.HolsterWasEntered += InHolsterFeedback;
        holsterCollider.HolsterWasExited += ExitHolsterFeedback;
        holsterCollider.SkillWasEquipped += EquipSkillFeedback;
        holsterCollider.InvalidEquipWasAttempted += InvalidEquipFeedback;
    }

    private void OnDestroy()
    {
        holsterCollider.HolsterWasEntered -= InHolsterFeedback;
        holsterCollider.HolsterWasExited -= ExitHolsterFeedback;
        holsterCollider.SkillWasEquipped -= EquipSkillFeedback;
        holsterCollider.InvalidEquipWasAttempted -= InvalidEquipFeedback;
    }

    private void InHolsterFeedback(bool isLeftController)
    {
        if (DebugLogger.IsNullError(holsterAudioManager, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(holsterManager, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(inHolsterUi, this, "Must be set in editor.")) return;

        holsterAudioManager.PlayEnterHolsterSound(audioSource);

        var controllerHaptics = isLeftController ? leftControllerHaptics : rightControllerHaptics;
        holsterManager.PlayEnterHolsterHaptics(controllerHaptics);

        inHolsterUi.SetActive(true);
    }

    private void ExitHolsterFeedback()
    {
        inHolsterUi.SetActive(false);
    }

    private void EquipSkillFeedback(bool isLeftController)
    {
        if (DebugLogger.IsNullError(holsterAudioManager, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(holsterManager, this, "Must be set in editor.")) return;

        holsterAudioManager.PlayEquipSkillSound(audioSource);

        var controllerHaptics = isLeftController ? leftControllerHaptics : rightControllerHaptics;
        holsterManager.PlayEquipHaptics(controllerHaptics);
    }

    private void InvalidEquipFeedback()
    {
        holsterAudioManager.PlayInvalidEquipSound(audioSource);
    }
}
