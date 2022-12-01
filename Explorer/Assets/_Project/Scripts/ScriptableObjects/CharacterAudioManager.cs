using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Audio/CharacterAudioManager", order = 1)]
public class CharacterAudioManager : ScriptableObject
{
    [SerializeField] private AudioClip[] painClips;
    [SerializeField] private RangedFloat painVolume;
    [SerializeField] [MinMaxFloatRange(0, 3)] private RangedFloat painPitch;

    [SerializeField] private AudioClip[] deathClips;
    [SerializeField] private RangedFloat deathVolume;
    [SerializeField] [MinMaxFloatRange(0, 3)] private RangedFloat deathPitch;
    
    [SerializeField] private AudioClip[] jumpClips;
    [SerializeField] private RangedFloat jumpVolume;
    [SerializeField] [MinMaxFloatRange(0, 3)] private RangedFloat jumpPitch;
    
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private RangedFloat attackVolume;
    [SerializeField] [MinMaxFloatRange(0, 3)] private RangedFloat attackPitch;
    
    [SerializeField] private AudioClip[] hitImpactClips;
    [SerializeField] private RangedFloat hitImpactVolume;
    [SerializeField] [MinMaxFloatRange(0, 3)] private RangedFloat hitImpactPitch;
    
    [SerializeField] private AudioClip[] launchClips;
    [SerializeField] private RangedFloat launchVolume;
    [SerializeField] [MinMaxFloatRange(0, 3)] private RangedFloat launchPitch;
    
    [SerializeField] private AudioClip[] idleClips;
    [SerializeField] private RangedFloat idleVolume;
    [SerializeField] [MinMaxFloatRange(0, 3)] private RangedFloat idlePitch;
    

    private int _numPainClips;
    private int _numDeathClips;
    private int _numJumpClips;
    private int _numAttackClips;
    private int _numReceiveHitClips;
    private int _numLaunchClips;
    private int _numIdleClips;

    private void OnEnable()
    {
        InitializeNumClips();
    }

    private void InitializeNumClips()
    {
        if (painClips != null)
            _numPainClips = painClips.Length;

        if (deathClips != null)
            _numDeathClips = deathClips.Length;

        if (jumpClips != null)
            _numJumpClips = jumpClips.Length;

        if (attackClips != null)
            _numAttackClips = attackClips.Length;

        if (hitImpactClips != null)
            _numReceiveHitClips = hitImpactClips.Length;

        if (launchClips != null)
            _numLaunchClips = launchClips.Length;
        
        if (idleClips != null)
            _numIdleClips = idleClips.Length;
    }

    public void PlayPainClip(AudioSource audioSource)
    {
        if (_numPainClips == 0) return;

        audioSource.clip = painClips[Random.Range(0, _numPainClips)];
        audioSource.volume = Random.Range(painVolume.minValue, painVolume.maxValue);
        audioSource.pitch = Random.Range(painPitch.minValue, painPitch.maxValue);
        audioSource.Play();
    }

    public void PlayDeathClip(AudioSource audioSource)
    {
        if (_numDeathClips == 0) return;
        
        audioSource.clip = deathClips[Random.Range(0, _numDeathClips)];
        audioSource.volume = Random.Range(deathVolume.minValue, deathVolume.maxValue);
        audioSource.pitch = Random.Range(deathPitch.minValue, deathPitch.maxValue);
        audioSource.Play();
    }

    public void PlayJumpSound(AudioSource audioSource)
    {
        if (_numJumpClips == 0) return;
        
        audioSource.clip = jumpClips[Random.Range(0, _numJumpClips)];
        audioSource.volume = Random.Range(jumpVolume.minValue, jumpVolume.maxValue);
        audioSource.pitch = Random.Range(jumpPitch.minValue, jumpPitch.maxValue);
        audioSource.Play();
    }
    
    public void PlayAttackSound(AudioSource audioSource)
    {
        if (_numAttackClips == 0) return;
        
        audioSource.clip = attackClips[Random.Range(0, _numAttackClips)];
        audioSource.volume = Random.Range(attackVolume.minValue, attackVolume.maxValue);
        audioSource.pitch = Random.Range(attackPitch.minValue, attackPitch.maxValue);
        audioSource.Play();
    }
    
    public void PlayReceiveHitSound(AudioSource audioSource)
    {
        if (_numReceiveHitClips == 0) return;
        
        audioSource.clip = hitImpactClips[Random.Range(0, _numReceiveHitClips)];
        audioSource.volume = Random.Range(hitImpactVolume.minValue, hitImpactVolume.maxValue);
        audioSource.pitch = Random.Range(hitImpactPitch.minValue, hitImpactPitch.maxValue);
        audioSource.Play();
    }
    
    public void PlayLaunchSound(AudioSource audioSource)
    {
        if (_numLaunchClips == 0) return;
        
        audioSource.clip = launchClips[Random.Range(0, _numLaunchClips)];
        audioSource.volume = Random.Range(launchVolume.minValue, launchVolume.maxValue);
        audioSource.pitch = Random.Range(launchPitch.minValue, launchPitch.maxValue);
        audioSource.Play();
    }
    
    public void PlayIdleSound(AudioSource audioSource)
    {
        if (_numIdleClips == 0) return;
        
        audioSource.clip = idleClips[Random.Range(0, _numIdleClips)];
        audioSource.volume = Random.Range(idleVolume.minValue, idleVolume.maxValue);
        audioSource.pitch = Random.Range(idlePitch.minValue, idlePitch.maxValue);
        audioSource.Play();
    }
}
