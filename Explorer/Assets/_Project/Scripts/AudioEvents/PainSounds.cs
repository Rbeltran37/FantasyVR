using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainSounds : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CharacterAudioManager characterAudioManager;


    private void Awake()
    {
        if (health)
            health.WasHitButNotKilled += PlayPainSound;
    }

    private void OnDestroy()
    {
        if (health)
            health.WasHitButNotKilled -= PlayPainSound;
    }

    private void PlayPainSound()
    {
        if (!audioSource || !characterAudioManager) return;

        characterAudioManager.PlayPainClip(audioSource);
    }
}
