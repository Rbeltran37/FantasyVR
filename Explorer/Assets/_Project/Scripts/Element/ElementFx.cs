using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class ElementFx : MonoBehaviour
{
    [SerializeField] private ElementFxSO elementFxSo;
    [SerializeField] private ElementAudio ineffectiveElementAudio;
    [SerializeField] private ElementAudio normalElementAudio;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem debrisParticleSystem;
    [SerializeField] private ParticleSystem dustParticleSystem;

    private FxScalingParameters _debrisParameters;
    private FxScalingParameters _dustParameters;
    private float _velocityDifference;
    private float _sizeDifference;
    private float _forceDifference;
    private Dictionary<Element.Effectiveness, Action<float, float>> _fxDictionary;

    private class FxScalingParameters
    {
        public Transform ParticleTransform;
        public ParticleSystem ParticleSystem;
        public RangedFloat BaseSpeed;
        public RangedFloat BaseSize;
        public RangedInt BaseCount;
        public Vector3 LocalScale;
        public bool UseScaleAsSize;

        public FxScalingParameters(ElementFx elementFx, ElementFxSO elementFxSo, bool isDebris)
        {
            if (!elementFx || !elementFxSo) return;

            ParticleSystem = isDebris ? elementFx.debrisParticleSystem : elementFx.dustParticleSystem;
            if (!ParticleSystem) return;

            ParticleTransform = ParticleSystem.transform;
            
            var main = ParticleSystem.main;
            BaseSpeed.minValue = main.startSpeed.constantMin;
            BaseSpeed.maxValue = main.startSpeed.constantMax;
            BaseSize.minValue = main.startSize.constantMin;
            BaseSize.maxValue = main.startSize.constantMax;
            BaseCount.minValue = ParticleSystem.emission.GetBurst(0).minCount;
            BaseCount.maxValue = ParticleSystem.emission.GetBurst(0).maxCount;
            LocalScale = ParticleTransform.localScale;
            UseScaleAsSize = isDebris ? elementFxSo.debrisUseScaleAsSize : elementFxSo.dustUseScaleAsSize;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _velocityDifference = elementFxSo.velocityRange.maxValue - elementFxSo.velocityRange.minValue;
        _sizeDifference = elementFxSo.sizeRange.maxValue - elementFxSo.sizeRange.minValue;
        _forceDifference = elementFxSo.forceRange.maxValue - elementFxSo.forceRange.minValue;

        if (debrisParticleSystem)
        {
            _debrisParameters = new FxScalingParameters(this, elementFxSo, true);
        }

        if (dustParticleSystem)
        {
            _dustParameters = new FxScalingParameters(this, elementFxSo, false);
        }

        _fxDictionary = new Dictionary<Element.Effectiveness, Action<float, float>>
        {
            {Element.Effectiveness.Ineffective, PlayIneffectiveFx},
            {Element.Effectiveness.Normal, PlayNormalFx},
            {Element.Effectiveness.Effective, PlayEffectiveFx},
        };
    }

    public ElementFxSO GetElementFxSo()
    {
        return elementFxSo;
    }

    public void PlayFx(Element.Effectiveness effectiveness, float velocity, float size)
    {
        _fxDictionary[effectiveness].Invoke(velocity, size);
    }

    private void PlayIneffectiveFx(float velocity, float size)
    {
        var velocityPercentage = CalculateVelocityPercentage(velocity);
        var forcePercentage = CalculateForcePercentage(velocity * size);
        var sizePercentage = CalculateSizePercentage(size);
        PlayIneffectiveAudio(sizePercentage, forcePercentage);
        PlayParticleFx(velocityPercentage, sizePercentage, forcePercentage);
    }
    
    private void PlayNormalFx(float velocity, float size)
    {
        var velocityPercentage = CalculateVelocityPercentage(velocity);
        var forcePercentage = CalculateForcePercentage(velocity * size);
        var sizePercentage = CalculateSizePercentage(size);
        PlayNormalAudio(sizePercentage, forcePercentage);
        PlayParticleFx(velocityPercentage, sizePercentage, forcePercentage);
    }
    
    private void PlayEffectiveFx(float velocity, float size)
    {
        var velocityPercentage = CalculateVelocityPercentage(velocity);
        var forcePercentage = CalculateForcePercentage(velocity * size);
        var sizePercentage = CalculateSizePercentage(size);
        PlayNormalAudio(sizePercentage, forcePercentage);
        PlayParticleFx(velocityPercentage, sizePercentage, forcePercentage);
    }

    private float CalculateVelocityPercentage(float velocity)
    {
        var velocityPercentage = Mathf.Clamp(velocity, elementFxSo.velocityRange.minValue, elementFxSo.velocityRange.maxValue);
        return (velocityPercentage - elementFxSo.velocityRange.minValue) / _velocityDifference;
    }

    private float CalculateForcePercentage(float force)
    {
        var forcePercentage = Mathf.Clamp(force, elementFxSo.forceRange.minValue, elementFxSo.forceRange.maxValue);
        return (forcePercentage - elementFxSo.forceRange.minValue) / _forceDifference;
    }

    private float CalculateSizePercentage(float size)
    {
        var massPercentage = Mathf.Clamp(size, elementFxSo.sizeRange.minValue, elementFxSo.sizeRange.maxValue);
        return (massPercentage - elementFxSo.sizeRange.minValue) / _sizeDifference;
    }

    private void PlayIneffectiveAudio(float mass, float force)
    {
        PlayAudio(ineffectiveElementAudio, mass, force);
    }
    
    private void PlayNormalAudio(float mass, float force)
    {
        PlayAudio(normalElementAudio, mass, force);
    }

    private void PlayAudio(ElementAudio elementAudio, float mass, float force)
    {
        if (!elementAudio) return;
        
        elementAudio.Play(audioSource, mass, force);
    }
    
    private void PlayParticleFx(float velocityPercentage, float sizePercentage, float forcePercentage)
    {
        var debrisSpeedMultiplier = CalculateMultiplier(velocityPercentage, elementFxSo.debrisSpeedMaxMultiplier);
        var debrisSizeMultiplier = CalculateMultiplier(sizePercentage, elementFxSo.debrisSizeMaxMultiplier);
        var debrisCountMultiplier = CalculateMultiplier(forcePercentage, elementFxSo.debrisCountMaxMultiplier);
        ApplyChangesToParticleSystem(_debrisParameters, debrisSpeedMultiplier, debrisSizeMultiplier, debrisCountMultiplier);
        
        var dustSpeedMultiplier = CalculateMultiplier(velocityPercentage, elementFxSo.dustSpeedMaxMultiplier);
        var dustSizeMultiplier = CalculateMultiplier(sizePercentage, elementFxSo.dustSizeMaxMultiplier);
        var dustCountMultiplier = CalculateMultiplier(forcePercentage, elementFxSo.dustCountMaxMultiplier);
        ApplyChangesToParticleSystem(_dustParameters, dustSpeedMultiplier, dustSizeMultiplier, dustCountMultiplier);
    }

    private float CalculateMultiplier(float percentage, float max)
    {
        return ((max - 1) * percentage) + 1;
    }

    private void ApplyChangesToParticleSystem(FxScalingParameters fxScalingParameters,float speedMultiplier, float sizeMultiplier, float countMultiplier)
    {
        if (fxScalingParameters == null) return;
        
        var main = fxScalingParameters.ParticleSystem.main;
        main.startSpeed = new ParticleSystem.MinMaxCurve(fxScalingParameters.BaseSpeed.minValue * speedMultiplier, fxScalingParameters.BaseSpeed.maxValue * speedMultiplier);

        if (fxScalingParameters.UseScaleAsSize)
        {
            fxScalingParameters.ParticleTransform.localScale = fxScalingParameters.LocalScale * sizeMultiplier;
        }
        else
        {
            main.startSize = new ParticleSystem.MinMaxCurve(fxScalingParameters.BaseSize.minValue * sizeMultiplier, fxScalingParameters.BaseSize.maxValue * sizeMultiplier);
        }

        fxScalingParameters.ParticleSystem.emission.SetBurst(0,
            new ParticleSystem.Burst(0, 
                (short) (fxScalingParameters.BaseCount.minValue * countMultiplier), (short) (fxScalingParameters.BaseCount.maxValue * countMultiplier)));
        
        fxScalingParameters.ParticleSystem.Play();
    }
}
