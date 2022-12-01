using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerSkill : Weapon
{
    private HammerInstance _hammerInstance;


    public override void Equip()
    {
        base.Equip();

        _hammerInstance = (HammerInstance) WeaponInstance;
    }

    public override void StartUse()
    {
        if (DebugLogger.IsNullError(_hammerInstance, this)) return;
        
        _hammerInstance.UseAbility();
    }

    public override void EndUse()
    {
        if (!_hammerInstance) return;

        _hammerInstance.DeactivateWeaponAbility();
    }

    public override void Cast()
    {
        LaunchRing();
        PayCastingCost();
    }
}
