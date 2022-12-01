using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName="ScriptableObjects/Audio/ElementAudioSO")]
public class ElementAudio : ScriptableObject
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private RangedFloat volume;
    [MinMaxFloatRange(0, 3)] [SerializeField] private RangedFloat pitch;
    [SerializeField] private float volumeMultiplier = .1f;
    [SerializeField] private float pitchMultiplier = .1f;

    private float _volumeDifference;
    private float _pitchDifference;


    private void OnEnable()
    {
        _volumeDifference = volume.maxValue - volume.minValue;
        _pitchDifference = pitch.maxValue - pitch.minValue;
    }

    public void Play(AudioSource audioSource, float massMultiplier, float forceMultiplier)
    {
        if (clips.Length == 0 || !audioSource) return;

        var randomVolumeFactor = Random.Range(-volumeMultiplier, volumeMultiplier);
        var randomPitchFactor = Random.Range(-pitchMultiplier, pitchMultiplier);
        
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.volume = volume.minValue + (_volumeDifference * forceMultiplier) + randomVolumeFactor;
        audioSource.pitch = pitch.maxValue - (_pitchDifference * massMultiplier) + randomPitchFactor;    
        audioSource.Play();
    }
}