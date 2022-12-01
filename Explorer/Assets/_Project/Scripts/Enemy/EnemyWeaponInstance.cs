using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyWeaponInstance : MonoBehaviour
{
    public GameObject ThisGameObject;
    public Transform AimTransform;
    public DamageDealt DamageDealt;
    public DropGearContainer DropGearContainer;
    public EnemyWeaponSO EnemyWeaponSo;
    private string AIM;

    public EnemyWeaponInstance()
    {
        AIM = "aim";
    }

    public bool IsBow => EnemyWeaponSo.EnemyWeaponType == EnemyWeaponSO.WeaponType.Bow;
    public bool IsShield => EnemyWeaponSo.EnemyWeaponType == EnemyWeaponSO.WeaponType.Shield;


    [Button]
    private void PopulateParameters()
    {
        if (!ThisGameObject) ThisGameObject = gameObject;
        
        if (!DamageDealt)
        {
            DamageDealt = GetComponentInChildren<DamageDealt>();
        }

        if (!AimTransform)
        {
            var childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name.ToLower().Contains(AIM)) AimTransform = child;
            }
        }
        
        if (!DropGearContainer)
        {
            DropGearContainer = GetComponentInChildren<DropGearContainer>();
            if (!DropGearContainer) DropGearContainer = ThisGameObject.AddComponent<DropGearContainer>();
        }
    }
}
