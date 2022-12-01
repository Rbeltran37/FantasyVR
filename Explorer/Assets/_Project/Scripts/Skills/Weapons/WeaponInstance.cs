using System;
using System.Collections;
using UnityEngine;
using Zinnia.Tracking.Velocity;

public class WeaponInstance : SkillInstance
{
    [SerializeField] protected Rigidbody PooledRigidbody;
    [SerializeField] protected Collider PooledCollider;
    [SerializeField] protected WeaponAbility WeaponAbility;

    [SerializeField] private WeaponReferenceHelper weaponReferenceHelper;

    protected ModifierData ModifierData;
    protected WeaponAbilityData WeaponAbilityData;
    protected AverageVelocityEstimator AverageVelocityEstimator;
    
    private bool _resetIsKinematic;
    private bool _resetUseGravity;
    private bool _resetIsTrigger;
    
    
    public override void SetSkill(Skill skill)
    {
        base.SetSkill(skill);

        SetModifierData(skill.ModifierData);

        AverageVelocityEstimator = skill.GetPlayerHand().GetAverageVelocityEstimator();
    }
    
    public Rigidbody GetRigidbody()
    {
        if (DebugLogger.IsNullWarning(PooledRigidbody, this))
        {
            PooledRigidbody = GetComponent<Rigidbody>();
            if (DebugLogger.IsNullError(PooledRigidbody, this)) return null;
        }
        
        return PooledRigidbody;
    }

    public Collider GetCollider()
    {
        if (DebugLogger.IsNullError(PooledCollider, this)) return null;
        
        return PooledCollider;
    }
    
    public WeaponReferenceHelper GetWeaponReferenceHelper()
    {
        return weaponReferenceHelper;
    }
    
    protected override void Initialize()
    {
        base.Initialize();

        if (PooledRigidbody)
        {
            _resetIsKinematic = PooledRigidbody.isKinematic;
            _resetUseGravity = PooledRigidbody.useGravity;
        }

        if (PooledCollider)
        {
            _resetIsTrigger = PooledCollider.isTrigger;
        }
    }

    protected override void ResetObject()
    {
        base.ResetObject();

        if (PooledRigidbody)
        {
            PooledRigidbody.velocity = Vector3.zero;
            PooledRigidbody.angularVelocity = Vector3.zero;
            PooledRigidbody.isKinematic = _resetIsKinematic;
            PooledRigidbody.useGravity = _resetUseGravity;
        }
        
        if (PooledCollider)
        {
            PooledCollider.isTrigger = _resetIsTrigger;
        }
    }

    private void SetModifierData(ModifierData modifierData)
    {
        ModifierData = modifierData;
    }

    public virtual void UseAbility()
    {
        if (DebugLogger.IsNullError(WeaponAbilityData, this)) return;

        WeaponAbility.Activate(WeaponAbilityData);
    }
}
