using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WeaponThrowAbility : WeaponAbility
{
    [SerializeField] protected Transform ThisTransform;
    [SerializeField] protected GameObject[] InstanceModels;
    [SerializeField] protected Collider[] InstanceColliders;
    [SerializeField] protected ObjectPool ThrownWeaponObjectPool;
    
    [SerializeField] private PhotonView thisPhotonView;

    protected bool IsReturning;
    protected float ThrowSpeed;
    protected Skill Skill;
    protected ThrownWeapon CurrentThrownWeapon;
    
    
    private void Awake()
    {
        if (DebugLogger.IsNullWarning(ThisTransform, this, "Should be set in editor")) ThisTransform = transform;
        
        if (DebugLogger.IsNullError(ThrownWeaponObjectPool, this, "Must be set in editor.")) return;
        
        ThrownWeaponObjectPool.InitializePool();
    }
    
    private void OnDisable()
    {
        if (CurrentThrownWeapon)
        {
            CurrentThrownWeapon.FinishReturn();
            ResetAbility();
        }
    }
    
    public override void Activate(WeaponAbilityData weaponAbilityData)
    {
        if (CurrentThrownWeapon) return;
        
        Skill = weaponAbilityData.Skill;
        if (!Skill.CanCast()) return;

        var modifierData = Skill.ModifierData;
        
        HideWeaponInstance();
        SendHideWeaponInstance();
        SetCurrentThrownWeapon();
        SetPlayerHand();
        ApplyModifiers(modifierData);

        SetupThrownWeapon();        //dependant on ThrownWeapon, should be overriden
        
        Throw();
    }
    
    public override void Deactivate(WeaponAbilityData weaponAbilityData)
    {
        if (IsReturning) return;

        Return();
    }
    
    private void FinishReturn()
    {
        ResetAbility();

        Skill.Cast();
    }

    private void ResetAbility()
    {
        IsReturning = false;
        
        CurrentThrownWeapon = null;

        ShowWeaponInstance();
        SendShowWeaponInstance();
    }

    private void ShowWeaponInstance()
    {
        foreach (var instanceCollider in InstanceColliders)
        {
            instanceCollider.enabled = true;
        }

        foreach (var instanceModel in InstanceModels)
        {
            instanceModel.SetActive(true);
        }
    }

    private void SendShowWeaponInstance()
    {
        if (DebugLogger.IsNullError(thisPhotonView, "Must be set in editor." , this)) return;
        
        thisPhotonView.RPC(nameof(RPCShowWeaponInstance), RpcTarget.Others);
    }

    [PunRPC]
    protected void RPCShowWeaponInstance()
    {
        ShowWeaponInstance();
    }
    
    private void HideWeaponInstance()
    {
        foreach (var instanceCollider in InstanceColliders)
        {
            instanceCollider.enabled = false;
        }
        
        foreach (var instanceModel in InstanceModels)
        {
            instanceModel.SetActive(false);
        }
    }

    private void SendHideWeaponInstance()
    {
        if (DebugLogger.IsNullError(thisPhotonView, "Must be set in editor." , this)) return;

        thisPhotonView.RPC(nameof(RPCHideWeaponInstance), RpcTarget.Others);
    }

    [PunRPC]
    protected void RPCHideWeaponInstance()
    {
        HideWeaponInstance();
    }
    
    protected virtual void SetCurrentThrownWeapon()
    {
        var pooledObject = ThrownWeaponObjectPool.GetPooledObject();
        CurrentThrownWeapon = (ThrownWeapon) pooledObject;
        if (DebugLogger.IsNullError(CurrentThrownWeapon, this)) return;
        
        CurrentThrownWeapon.HasReturned = null;
        CurrentThrownWeapon.HasReturned += FinishReturn;
    }
    
    private void SetPlayerHand()
    {
        CurrentThrownWeapon.SetPlayerHand(Skill);
    }
    
    private void ApplyModifiers(ModifierData modifierData)
    {
        CurrentThrownWeapon.ApplyModifiers(modifierData);
    }

    protected virtual void SetupThrownWeapon()
    {
        SetThrowSpeed();
        return;
    }
    
    protected virtual void SetThrowSpeed()
    {
        ThrowSpeed = Skill.ModifierData.GetCurrentValue(Skill.SkillSo.ModifierTypeLookUp.Speed);
        CurrentThrownWeapon.SetThrowSpeed(ThrowSpeed);
    }
    
    protected virtual void Throw()
    {
        CurrentThrownWeapon.SetScale(ThisTransform.localScale);
        CurrentThrownWeapon.SetReturnTarget(ThisTransform);
        CurrentThrownWeapon.Spawn(null, ThisTransform.position, ThisTransform.rotation, true);
    }

    private void Return()
    {
        if (!CurrentThrownWeapon) return;
        
        CurrentThrownWeapon.StartReturn();
        
        IsReturning = true;
    }
}
