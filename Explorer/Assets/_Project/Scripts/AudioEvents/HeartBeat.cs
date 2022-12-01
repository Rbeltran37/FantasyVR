using System;
using System.Collections;
using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip heartBeatClip;
    
    [SerializeField] private RangedFloat volume;
    [SerializeField] [MinMaxFloatRange(0, 3)] private RangedFloat pitch;
    [SerializeField] [Range(0, 1)] private float healthPercentActivation = .5f;

    private float _volumeDifference;
    private float _pitchDifference;


    private void Awake()
    {
        if (health)
        {
            health.WasHit += SetHeartBeat;
            health.WasAdded += SetHeartBeat;
        }

        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.clip = heartBeatClip;
        }
        
        _volumeDifference = volume.maxValue - volume.minValue;
        _pitchDifference = pitch.maxValue - pitch.minValue;
    }

    private void OnDestroy()
    {
        if (health)
        {
            health.WasHit -= SetHeartBeat;
            health.WasAdded -= SetHeartBeat;
        }
    }

    private void SetHeartBeat(float damage)
    {
        if (!health || !audioSource) return;
        
        var currentHealthPercentage = health.GetHealthPercentage();
        if (currentHealthPercentage > healthPercentActivation) return;

        var healthPercentDifference = healthPercentActivation - currentHealthPercentage;
        var heartVolume = _volumeDifference * (healthPercentDifference / healthPercentActivation) + volume.minValue;
        audioSource.volume = Mathf.Clamp(heartVolume, volume.minValue, volume.maxValue);
        
        var heartPitch = _pitchDifference * (healthPercentDifference / healthPercentActivation) + pitch.minValue;
        audioSource.pitch = Mathf.Clamp(heartPitch, pitch.minValue, pitch.maxValue);
        
        audioSource.Play();
    }
}
