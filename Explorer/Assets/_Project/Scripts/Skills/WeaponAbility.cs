using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAbility : MonoBehaviour
{
    public abstract void Activate(WeaponAbilityData weaponAbilityData);
    public abstract void Deactivate(WeaponAbilityData weaponAbilityData);
}
