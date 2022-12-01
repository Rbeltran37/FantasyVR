using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbilityData
{
    private WeaponInstance _weaponInstance;
    
    public Skill Skill => _weaponInstance.GetSkill();


    public WeaponAbilityData(WeaponInstance weaponInstance)
    {
        if (!weaponInstance) return;
        
        _weaponInstance = weaponInstance;
    }
}
