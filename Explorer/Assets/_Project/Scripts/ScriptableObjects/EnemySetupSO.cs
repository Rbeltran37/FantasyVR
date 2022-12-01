using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemy/EnemySetupSO", order = 1)]
public class EnemySetupSO : ScriptableObject
{
    public RuntimeAnimatorController runtimeAnimatorController;
    public ExternalBehaviorTree externalBehaviorTree;
    public EnemyWeaponSetupSO primaryEnemyWeaponSetupSo;
    public EnemyWeaponSetupSO secondaryEnemyWeaponSetupSo;
}
