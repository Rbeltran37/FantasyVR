using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShieldThrowAbility : WeaponThrowAbility
{
    [SerializeField] private ModifierTypeSO multiHitModifierTypeSo;
    [SerializeField] private Transform curvePoint;
    
    private ThrownShield _currentThrownShield;
    

    protected override void SetCurrentThrownWeapon()
    {
        base.SetCurrentThrownWeapon();

        _currentThrownShield = (ThrownShield) CurrentThrownWeapon;
    }
    
    protected override void SetupThrownWeapon()
    {
        base.SetupThrownWeapon();
        
        SetNumHitsRemaining();
        
        _currentThrownShield.SetSpeed(ThrowSpeed);
        _currentThrownShield.SetCurvePoint(curvePoint);
    }
    
    private void SetNumHitsRemaining()
    {
        var numHits = Skill.ModifierData.GetCurrentValueInt(multiHitModifierTypeSo);
        _currentThrownShield.SetNumHitsRemaining(numHits);
    }
}
