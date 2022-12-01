using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShieldSkill : Weapon
{
    private Vector3 _aimPosition;
    private Quaternion _aimRotation;
    private ShieldInstance _shieldInstance;


    protected override void Awake()
    {
        base.Awake();

        _aimPosition = AimGameObject.transform.localPosition;
        _aimRotation = AimGameObject.transform.localRotation;
    }

    public override void Equip()
    {
        base.Equip();
        
        WeaponInstance.SetSkill(this);
        _shieldInstance = (ShieldInstance) WeaponInstance;
        WeaponInstance = _shieldInstance;
    }

    public override void StartUse()
    {
        if (DebugLogger.IsNullError(_shieldInstance, this)) return;
        
        WeaponInstance.SetSkill(this);
        _shieldInstance.UseAbility();
    }

    public override void EndUse()
    {
        if (!_shieldInstance) return;

        _shieldInstance.DeactivateWeaponAbility();
    }

    public override void Cast()
    {
        LaunchRing();
        PayCastingCost();
    }

    protected override void SetGrabTransformAndRigid(PlayerHand playerHand)
    {
        PuppetGrabTransform = playerHand.GetPuppetForearmTransform();
        PuppetGrabRigid = playerHand.GetPuppetForearmRigidbody();
        
        AimGameObject.transform.SetParent(playerHand.GetPuppetForearmTransform());
        AimGameObject.transform.localPosition = _aimPosition;
        AimGameObject.transform.localRotation = _aimRotation;
    }
}