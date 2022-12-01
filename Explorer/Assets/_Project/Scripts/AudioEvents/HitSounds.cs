using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSounds : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CharacterAudioManager characterAudioManager;


    private void Awake()
    {
        if (health)
            health.WasHit += PlayHitSound;
    }

    private void OnDestroy()
    {
        if (health)
            health.WasHit -= PlayHitSound;
    }

    private void PlayHitSound(float damage)
    {
        if (!audioSource || !characterAudioManager) return;

        characterAudioManager.PlayReceiveHitSound(audioSource);
    }
}