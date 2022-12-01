using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSounds : MonoBehaviour
{
    [SerializeField] private SimpleAI simpleAi;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CharacterAudioManager characterAudioManager;


    private void Awake()
    {
        if (simpleAi)
            simpleAi.AimStarted += PlayAttackSound;
    }

    private void OnDestroy()
    {
        if (simpleAi)
            simpleAi.AimStarted -= PlayAttackSound;
    }

    private void PlayAttackSound()
    {
        if (!audioSource || !characterAudioManager) return;

        characterAudioManager.PlayAttackSound(audioSource);
    }
}