using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Demos;
using Sirenix.OdinInspector;
using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    [SerializeField] private FreezeEffectSO freezeEffectSo;
    [SerializeField] private CustomCharacterAnimationThirdPerson customCharacterAnimationThirdPerson;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Health health;

    private Material _defaultMaterial;
    private Material _materialInstance;
    private Coroutine _iceEffectCoroutine;
    private Coroutine _radiusCoroutine;

    private bool IsSlowed => customCharacterAnimationThirdPerson.animSpeedMultiplier < 
                             customCharacterAnimationThirdPerson.defaultAnimatorSpeed;
    private float ColorChangeRate => freezeEffectSo.unSlowSpeed * freezeEffectSo.iceColorStrAdd;


    private void Start()
    {
        _defaultMaterial = skinnedMeshRenderer.material;
        freezeEffectSo.SetNameIds();
        _materialInstance = new Material(freezeEffectSo.iceEffectMaterial);
    }

    public void ApplyFreezeEffect(float slowAmount, Vector3 pointOfContact)
    {
        if (!customCharacterAnimationThirdPerson) return;
        
        if (_iceEffectCoroutine != null) CoroutineCaller.Instance.StopCoroutine(_iceEffectCoroutine);
        _iceEffectCoroutine = CoroutineCaller.Instance.StartCoroutine(IceEffectCoroutine(slowAmount, pointOfContact));
        
        if (_radiusCoroutine != null) CoroutineCaller.Instance.StopCoroutine(_radiusCoroutine);
        _radiusCoroutine = CoroutineCaller.Instance.StartCoroutine(IceRadiusCoroutine());
    }

    private IEnumerator IceEffectCoroutine(float slowAmount, Vector3 pointOfContact)
    {
        var speedChangeDifference = customCharacterAnimationThirdPerson.defaultAnimatorSpeed - slowAmount;
        
        InitializeSlowEffect(speedChangeDifference);
        InitializeColorEffect();

        var iceColorStr = freezeEffectSo.iceColorStrAdd;
        var adjustedColorChangeRate = ColorChangeRate / speedChangeDifference;
        while (IsSlowed)
        {
            customCharacterAnimationThirdPerson.animSpeedMultiplier += freezeEffectSo.unSlowSpeed;
            customCharacterAnimationThirdPerson.animator.speed = customCharacterAnimationThirdPerson.animSpeedMultiplier;

            iceColorStr -= adjustedColorChangeRate;
            _materialInstance.SetFloat(freezeEffectSo.iceColorStrId, iceColorStr);
					
            yield return new WaitForSeconds(freezeEffectSo.unSlowInterval);
        }
				
        EndIceEffect();
    }

    private void InitializeColorEffect()
    {
        _materialInstance.SetFloat(freezeEffectSo.iceColorStrId, freezeEffectSo.iceColorStrAdd);
    }

    private void InitializeSlowEffect(float speedChangeDifference)
    {
        customCharacterAnimationThirdPerson.animSpeedMultiplier =
            Mathf.Clamp(customCharacterAnimationThirdPerson.animSpeedMultiplier, 0, speedChangeDifference);
    }

    private IEnumerator IceRadiusCoroutine()
    {
        skinnedMeshRenderer.material = _materialInstance;
			
        var radius = _materialInstance.GetFloat(freezeEffectSo.radiusId);
        while (radius < freezeEffectSo.maxRadius)
        {
            radius += freezeEffectSo.iceEffectRadiusSpeed;
            _materialInstance.SetFloat(freezeEffectSo.radiusId, radius);
            yield return new WaitForSeconds(freezeEffectSo.iceEffectRadiusInterval);
        }
    }

    private void EndIceEffect()
    {
        if (!health.isAlive) return;
        
        customCharacterAnimationThirdPerson.ResetAnimatorSpeed();
        _materialInstance.SetFloat(freezeEffectSo.radiusId, 0);
        _materialInstance.SetFloat(freezeEffectSo.iceColorStrId, 0);
        
        skinnedMeshRenderer.material = _defaultMaterial;
    }
}
