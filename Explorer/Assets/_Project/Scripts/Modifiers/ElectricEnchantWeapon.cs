using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RootMotion.Dynamics;
using UnityEngine;

public class ElectricEnchantWeapon : WeaponAbility
{
    [SerializeField] private SkillInstance skillInstance;
    [SerializeField] private Transform castingTransform;
    [SerializeField] private ObjectPool castingFxObjectPool;
    [SerializeField] private ShockPuppetSO shockPuppetSo;
    [SerializeField] private ShockEffectSO shockEffectSo;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float enchantedAudioInterval;
    [SerializeField] [Range(0, 2)] private float enchantedHapticInterval;
    [SerializeField] private LocalizedSkillAbility localizedSkillAbility;

    private bool _isCasting;
    private bool _isEnchanted;
    private Material _electricMaterialInstance;
    private Coroutine _currentCast;
    private Coroutine _currentEnchant;
    private CastingFx _currentCastingFx;
    private PlayerHand _playerHand;
    private HammerAbilityData _hammerAbilityData;
    private SkillSO _skillSo;
    
    private Skill Skill => _hammerAbilityData.Skill;
    

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Reset();
    }

    private void OnDisable()
    {
        EndEnchantment();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_isEnchanted) return;
        
        var broadcaster = other.collider.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster) 
        {
            ShockMuscles(broadcaster);
            ShockFx(broadcaster);
        }
    }
    
    private void Initialize()
    {
        _electricMaterialInstance = meshRenderer.materials[1];
        shockEffectSo.SetNameIds();
        
        _playerHand = skillInstance.GetSkill().GetPlayerHand();
        _skillSo = skillInstance.GetSkill().SkillSo;
    }

    private void Reset()
    {
        _isCasting = false;
        _isEnchanted = false;
        _electricMaterialInstance.SetFloat(shockEffectSo.amountId, 0);
        
        if (_currentCast != null) CoroutineCaller.Instance.StopCoroutine(_currentCast);
        _currentCast = null;
        if (_currentCastingFx) _currentCastingFx.StopCasting(_skillSo);
    }

    public override void Activate(WeaponAbilityData weaponAbilityData)
    {
        if (_hammerAbilityData == null) _hammerAbilityData = (HammerAbilityData) weaponAbilityData;

        if (!CanCast()) return;

        _isCasting = true;
        _currentCast = CoroutineCaller.Instance.StartCoroutine(CastingSorceryCoroutine(_hammerAbilityData.CastingSpeed));
    }

    public override void Deactivate(WeaponAbilityData weaponAbilityData)
    {
        StopCasting();
    }
    
    private bool CanCast()
    {
        return Skill.CanCast();
    }

    private IEnumerator CastingSorceryCoroutine(float castingSpeed)
    {
        CastingFx();
        yield return new WaitForSeconds(castingSpeed);
        _currentCast = null;
        _isCasting = false;
        Cast();
    }

    private void CastingFx()
    {
        CoroutineCaller.Instance.StartCoroutine(CastingParticleFxCoroutine());
        CoroutineCaller.Instance.StartCoroutine(CastingAudioCoroutine());
        CoroutineCaller.Instance.StartCoroutine(CastingHapticsCoroutine());
    }

    private IEnumerator CastingParticleFxCoroutine()
    {
        var pooledObject = castingFxObjectPool.GetPooledObject();
        _currentCastingFx = (CastingFx) pooledObject;
        _currentCastingFx.Spawn(castingTransform, Vector3.zero, Quaternion.identity, false);
        _currentCastingFx.Casting(_skillSo);

        while (_isCasting)
        {
            yield return null;
        }
        
        _currentCastingFx.StopCasting(_skillSo);
    }
    
    private IEnumerator CastingHapticsCoroutine()
    {
        if (_skillSo.castingHapticInterval < Mathf.Epsilon) yield break;
        
        while (_isCasting)
        {
            _skillSo.PlayCastingHaptics(_playerHand.GetControllerHaptics());
            yield return new WaitForSeconds(_skillSo.castingHapticInterval);
        }
    }
    
    private IEnumerator CastingAudioCoroutine()
    {
        if (_skillSo.castingAudioInterval < Mathf.Epsilon) yield break;

        var loopAudioSource = _playerHand.GetLoopAudioSource();
        if (DebugLogger.IsNullError(loopAudioSource, this)) yield break;

        while (_isCasting)
        {
            _skillSo.PlayCastingAudio(_playerHand.GetLoopAudioSource());
            yield return new WaitForSeconds(_skillSo.castingAudioInterval);
        }
        
        loopAudioSource.Stop();
    }

    private void StopCasting()
    {
        _isCasting = false;
        if (_currentCast != null) CoroutineCaller.Instance.StopCoroutine(_currentCast);
        _currentCast = null;
        
        if (_currentCastingFx) _currentCastingFx.StopCasting(_skillSo);
    }

    private void Cast()
    {
        Skill.Cast();
        
        StopCasting();
        CastFx();
        Enchant();
        localizedSkillAbility.ActivateAtPosition(castingTransform.position);
    }

    private void CastFx()
    {
        _currentCastingFx.Cast(_skillSo);
        _skillSo.PlayCastHaptics(_playerHand.GetControllerHaptics());
        _skillSo.PlayCastAudio(_playerHand.GetBurstAudioSource());
    }

    private void Enchant()
    {
        if (_currentEnchant != null) CoroutineCaller.Instance.StopCoroutine(_currentEnchant);
        _currentEnchant = CoroutineCaller.Instance.StartCoroutine(EnchantCoroutine());
    }
    
    private IEnumerator EnchantHapticsCoroutine()
    {
        if (enchantedHapticInterval < Mathf.Epsilon) yield break;
        
        while (_isEnchanted)
        {
            _skillSo.PlayCastingHaptics(_playerHand.GetControllerHaptics());
            yield return new WaitForSeconds(enchantedHapticInterval);
        }
    }

    private IEnumerator EnchantAudioCoroutine()
    {
        if (enchantedAudioInterval < Mathf.Epsilon) yield break;

        var loopAudioSource = _playerHand.GetLoopAudioSource();
        if (DebugLogger.IsNullError(loopAudioSource, this)) yield break;

        while (_isEnchanted)
        {
            _skillSo.PlayCastingAudio(_playerHand.GetLoopAudioSource());
            yield return new WaitForSeconds(enchantedAudioInterval);
        }
        
        loopAudioSource.Stop();
    }

    private IEnumerator EnchantCoroutine()
    {
        _isEnchanted = true;
        _electricMaterialInstance.SetFloat(shockEffectSo.amountId, shockEffectSo.electricityAmount);
        CoroutineCaller.Instance.StartCoroutine(EnchantAudioCoroutine());
        CoroutineCaller.Instance.StartCoroutine(EnchantHapticsCoroutine());

        yield return new WaitForSeconds(_hammerAbilityData.Lifetime);
        
        EndEnchantment();
    }

    private void EndEnchantment()
    {
        _electricMaterialInstance.SetFloat(shockEffectSo.amountId, 0);
        _isEnchanted = false;
        _currentEnchant = null;
    }

    private void ShockMuscles(MuscleCollisionBroadcaster muscleCollisionBroadcaster)
    {
        if (DebugLogger.IsNullError(muscleCollisionBroadcaster, this)) return;
        if (DebugLogger.IsNullError(shockPuppetSo, this, "Must be set in editor.")) return;

        shockPuppetSo.ApplyShock(muscleCollisionBroadcaster);
    }

    private void ShockFx(MuscleCollisionBroadcaster muscleCollisionBroadcaster)
    {
        if (DebugLogger.IsNullError(muscleCollisionBroadcaster, this)) return;
        if (DebugLogger.IsNullError(shockPuppetSo, this, "Must be set in editor.")) return;

        var puppetMaster = muscleCollisionBroadcaster.puppetMaster;
        if (DebugLogger.IsNullError(puppetMaster, this)) return;

        var shockEffect = puppetMaster.GetComponent<ShockEffect>();
        if (DebugLogger.IsNullError(shockEffect, this)) return;

        shockEffect.ApplyShockEffect(shockPuppetSo);
    }
}
