using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerInstance : WeaponInstance
{
    [SerializeField] private PhysicsDamageDealt physicsDamageDealt;

    
    private void Start()
    {
        WeaponAbilityData = new HammerAbilityData(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (ModifierData != null) ApplyModifiers(ModifierData);
    }

    public override void SetSkill(Skill skill)
    {
        base.SetSkill(skill);
        
        physicsDamageDealt.SetAverageVelocityEstimator(AverageVelocityEstimator);
    }

    public void DeactivateWeaponAbility()
    {
        WeaponAbility.Deactivate(WeaponAbilityData);
    }
}
