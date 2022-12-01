using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_EnemyWeaponSO", menuName = "ScriptableObjects/Enemy/EnemyWeaponSO", order = 1)]
public class EnemyWeaponSO : ScriptableObject
{
    public WeaponType EnemyWeaponType = WeaponType.Melee;
    
    public enum WeaponType
    {
        Arrow,
        Bow,
        Melee,
        Shield,
    }
}
