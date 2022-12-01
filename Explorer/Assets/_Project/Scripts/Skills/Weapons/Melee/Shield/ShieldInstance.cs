using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShieldInstance : WeaponInstance
{

    private void Start()
    {
        WeaponAbilityData = new ShieldAbilityData(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (ModifierData != null) ApplyModifiers(ModifierData);
    }

    public void DeactivateWeaponAbility()
    {
        WeaponAbility.Deactivate(WeaponAbilityData);
    }
}
