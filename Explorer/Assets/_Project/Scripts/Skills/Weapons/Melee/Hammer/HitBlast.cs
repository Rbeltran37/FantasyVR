using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBlast : PooledSkillAbility
{
    [SerializeField] private ForceFunctions forceFunctions;
    [SerializeField] private ParticleSystem hitForceFx;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SimpleAudioEvent simpleAudioEvent;

    private const float SIZE_MULTIPLIER = .01f;


    protected override void OnEnable()
    {
        base.OnEnable();
        
        SetFx();
        Activate();
    }

    private void SetFx()
    {
        var size = Value * SIZE_MULTIPLIER;
        var main = hitForceFx.main;
        var startSize = main.startSize;
        startSize.constant = size;
    }
    
    private void Activate()
    {
        hitForceFx.Play();
        simpleAudioEvent.Play(audioSource);
        
        forceFunctions.ForceRepulse();
    }
}
