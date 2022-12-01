using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public abstract class Skill : MonoBehaviour, IUsableOld
{
    [SerializeField] public SkillSO SkillSo;
    
    [SerializeField] protected Transform ThisTransform;
    [SerializeField] protected GameObject AimGameObject;
    [SerializeField] protected ObjectPool InstanceObjectPool;

    [SerializeField] private ObjectPool castingFxObjectPool;

    protected PlayerHand PlayerHand;
    protected ManaPool ManaPool;
    protected SkillCount SkillCount;
    protected bool CanCastPhased;
    
    private Coroutine _currentCast;
    private bool _isCasting;
    private float CastingTime => ModifierData.GetCurrentValue(SkillSo.ModifierTypeLookUp.Speed);
    private Vector3 _leftPosition;
    private Vector3 _rightPosition;
    private Quaternion _leftRotation;
    private Quaternion _rightRotation;
    
    public ModifierData ModifierData { get; private set; }
    public float Cooldown => ModifierData.GetCurrentValue(SkillSo.ModifierTypeLookUp.Cooldown);
    public float Cost => ModifierData.GetCurrentValue(SkillSo.ModifierTypeLookUp.Cost);
    public int Count => ModifierData.GetCurrentValueInt(SkillSo.ModifierTypeLookUp.Count);

    private const int INVALID = 0;
    

    protected virtual void Awake()
    {
        if (DebugLogger.IsNullWarning(ThisTransform, "Should be set in editor.", this))
        {
            ThisTransform = transform;
        }
        
        if (!DebugLogger.IsNullDebug(castingFxObjectPool, this))
        {
            castingFxObjectPool.InitializePool();
        }

        if (DebugLogger.IsNullDebug(AimGameObject, this)) return;       //TODO not properly catching null
        if (!AimGameObject) return;     //Fallback

        var localPosition = AimGameObject.transform.localPosition;
        _leftPosition = new Vector3(-localPosition.x, localPosition.y, localPosition.z);
        _rightPosition = localPosition;
        
        var localRotation = AimGameObject.transform.localRotation;
        _leftRotation = new Quaternion(localRotation.x, -localRotation.y, -localRotation.z, localRotation.w);
        _rightRotation = localRotation;
    }

    public virtual void Setup(PlayerHand playerHand, ManaPool manaPool)
    {
        if (DebugLogger.IsNullError(playerHand, this)) return;
        if (DebugLogger.IsNullError(manaPool, this)) return;
        if (DebugLogger.IsNullError(InstanceObjectPool, this)) return;

        PlayerHand = playerHand;
        ManaPool = manaPool;
        InstanceObjectPool.InitializePool();
    }

    public void SetModifierData(ModifierData modifierData)
    {
        ModifierData = modifierData;
    }

    public bool CanCast()
    {
        if (DebugLogger.IsNullError(ManaPool, this)) return false;
        
        return ManaPool.CanCast(Cost) && SkillCount.IsCountValid();
    }

    public virtual void StartUse()
    {
        _isCasting = true;
        
        if (SkillSo.IsInstant())
        {
            Cast();
        }
        else if (SkillSo.IsSorcery())
        {
            _currentCast = StartCoroutine(CastingSorceryCoroutine(CastingTime));
        }
        else
        {
            _currentCast = StartCoroutine(CastingPhasedCoroutine(CastingTime));
        }
    }

    private IEnumerator CastingSorceryCoroutine(float castingSpeed)
    {
        CastingFx();
        yield return new WaitForSeconds(castingSpeed);
        _currentCast = null;
        _isCasting = false;
        Cast();
    }
    
    private IEnumerator CastingPhasedCoroutine(float castingSpeed)
    {
        CastingFx();
        yield return new WaitForSeconds(castingSpeed);
        _currentCast = null;
        CanCastPhased = true;
        while (_isCasting)
        {
            yield return null;
        }
        Cast();
    }

    private void CastingFx()
    {
        PlayerHand.HoldOpen();
        StartCoroutine(CastingParticleFxCoroutine());
        StartCoroutine(CastingAudioCoroutine());
        StartCoroutine(CastingHapticsCoroutine());
    }

    private IEnumerator CastingParticleFxCoroutine()
    {
        var pooledObject = castingFxObjectPool.GetPooledObject();
        var castingFx = (CastingFx) pooledObject;
        castingFx.Spawn(ThisTransform, Vector3.zero, Quaternion.identity, false);
        castingFx.Casting(SkillSo);

        while (_isCasting)
        {
            yield return null;
        }
        
        castingFx.StopCasting(SkillSo);
    }

    private IEnumerator CastingHapticsCoroutine()
    {
        if (Math.Abs(SkillSo.castingHapticInterval - INVALID) < Mathf.Epsilon) yield break;
        
        while (_isCasting)
        {
            SkillSo.PlayCastingHaptics(PlayerHand.GetControllerHaptics());
            yield return new WaitForSeconds(SkillSo.castingHapticInterval);
        }
    }
    
    private IEnumerator CastingAudioCoroutine()
    {
        if (Math.Abs(SkillSo.castingAudioInterval - INVALID) < Mathf.Epsilon) yield break;

        var loopAudioSource = PlayerHand.GetLoopAudioSource();
        if (DebugLogger.IsNullError(loopAudioSource, this)) yield break;

        while (_isCasting)
        {
            SkillSo.PlayCastingAudio(PlayerHand.GetLoopAudioSource());
            yield return new WaitForSeconds(SkillSo.castingAudioInterval);
        }
        
        loopAudioSource.Stop();
    }

    public virtual void Cast()
    {
        if (DebugLogger.IsNullError(InstanceObjectPool, this)) return;

        var pooledObject = InstanceObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return;

        var skillInstance = (SkillInstance) pooledObject;
        if (DebugLogger.IsNullError(skillInstance, this)) return;

        skillInstance.SetSkill(this);
        ApplyModifiers(skillInstance);
        CastFx();
        SpawnSkillInstance(skillInstance);
        PayCastingCost();
        
        AimCheck();
    }

    public void ApplyModifiers(SkillInstance skillInstance)
    {
        if (DebugLogger.IsNullError(skillInstance, this)) return;

        skillInstance.ApplyModifiers(ModifierData);
    }

    private void CastFx()
    {
        if (DebugLogger.IsNullError(SkillSo, this)) return;
        if (DebugLogger.IsNullError(PlayerHand, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(castingFxObjectPool, this, "Must be set in editor.")) return;

        var pooledObject = castingFxObjectPool.GetPooledObject();
        var castingFx = (CastingFx) pooledObject;
        castingFx.Spawn(ThisTransform, Vector3.zero, Quaternion.identity, false);
        castingFx.Cast(SkillSo);
        
        SkillSo.PlayCastHaptics(PlayerHand.GetControllerHaptics());
        SkillSo.PlayCastAudio(PlayerHand.GetBurstAudioSource());
        PlayerHand.Open();
        LaunchRing();
    }

    protected virtual void SpawnSkillInstance(SkillInstance skillInstance)
    {
        skillInstance.Spawn(ThisTransform.rotation, ThisTransform.position, true);
    }
    
    private void AimCheck()
    {
        if (!AimGameObject) return;
        
        if (!CanCast()) AimGameObject.SetActive(false);
    }

    protected virtual void LaunchRing()
    {
        if (DebugLogger.IsNullError(SkillCount, this)) return;

        SkillCount.LaunchRing(ThisTransform.forward);
    }
    
    protected virtual void LaunchRing(Vector3 customVector)
    {
        if (DebugLogger.IsNullError(SkillCount, this)) return;

        SkillCount.LaunchRing(customVector);
    }

    public virtual void EndUse()
    {
        _isCasting = false;
        if (_currentCast != null) StopCoroutine(_currentCast);
        
        _currentCast = null;
    }

    public virtual void Equip()
    {
        ActivateAim();
    }

    private void ActivateAim()
    {
        if (!AimGameObject) return;

        var positionToUse = PlayerHand.IsLeftHand() ? _leftPosition : _rightPosition;
        AimGameObject.transform.localPosition = positionToUse;

        var rotationToUse = PlayerHand.IsLeftHand() ? _leftRotation : _rightRotation;
        AimGameObject.transform.localRotation = rotationToUse;
        AimGameObject.SetActive(true);
    }

    public virtual void UnEquip()
    {
        EndUse();
        
        if (!SkillCount) return;

        SkillCount.ZeroOutCount();
        
        DeactivateAim();
    }

    private void DeactivateAim()
    {
        if (!AimGameObject) return;

        AimGameObject.SetActive(false);
    }

    public bool IsTwoHanded()
    {
        if (DebugLogger.IsNullError(SkillSo, this)) return false;

        return SkillSo.isTwoHanded;
    }
    
    public bool IsUsedByOtherHand()
    {
        if (DebugLogger.IsNullError(SkillSo, this)) return false;

        return SkillSo.usedByOtherHand;
    }

    protected void PayCastingCost()
    {
        if (DebugLogger.IsNullError(ManaPool, this)) return;
        
        ManaPool.PayCastingCost(Cost);
    }
    
    public virtual void UseAbility()
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"Not implemented");
    }
    
    public void SetSkillCount(SkillCount skillCount)
    {
        SkillCount = skillCount;
    }

    public PlayerHand GetPlayerHand()
    {
        return PlayerHand;
    }
}
