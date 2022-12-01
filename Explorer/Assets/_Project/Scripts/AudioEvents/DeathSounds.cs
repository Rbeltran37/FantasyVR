using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSounds : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CharacterAudioManager characterAudioManager;


    private void Awake()
    {
        if (health)
            health.WasKilled += PlayDeathSound;
    }

    private void OnDestroy()
    {
        if (health)
            health.WasKilled -= PlayDeathSound;
    }

    private void PlayDeathSound()
    {
        if (!audioSource || !characterAudioManager) return;

        characterAudioManager.PlayDeathClip(audioSource);
    }
    
}