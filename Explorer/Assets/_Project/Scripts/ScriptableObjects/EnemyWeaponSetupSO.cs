using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RootMotion.FinalIK;
using UnityEngine;

[CreateAssetMenu(fileName = "_EnemyWeaponSetupSO", menuName = "ScriptableObjects/Enemy/EnemyWeaponSetupSO", order = 1)]
public class EnemyWeaponSetupSO : ScriptableObject
{
    [SerializeField] private SimpleAiData simpleAiData;
    [SerializeField] private CustomEnums.Axis aimIkAxisLeft;
    [SerializeField] [Range(0, 1)] private float clampWeightLeft;
    [SerializeField] [Range(0, 1)] private float aimIkShoulderWeightLeft;
    [SerializeField] [Range(0, 1)] private float aimIkArmWeightLeft;
    [SerializeField] [Range(0, 1)] private float aimIkForearmWeightLeft;
    [SerializeField] [Range(0, 1)] private float aimIkHandWeightLeft;
    [SerializeField] private CustomEnums.Axis aimIkAxisRight;
    [SerializeField] [Range(0, 1)] private float clampWeightRight;
    [SerializeField] [Range(0, 1)] private float aimIkShoulderWeightRight;
    [SerializeField] [Range(0, 1)] private float aimIkArmWeightRight;
    [SerializeField] [Range(0, 1)] private float aimIkForearmWeightRight;
    [SerializeField] [Range(0, 1)] private float aimIkHandWeightRight;

    public EnemyWeaponSO LeftEnemyWeaponSo;
    public EnemyWeaponSO RightEnemyWeaponSo;

    private const int SHOULDER = 0;
    private const int ARM = 1;
    private const int FOREARM = 2;
    private const int HAND = 3;


    public SimpleAiData GetSimpleAiData()
    {
        return simpleAiData;
    }

    public void SetupAimIk(AimIK aimAk, EnemyWeaponInstance enemyWeaponInstance, bool isLeft)
    {
        if (DebugLogger.IsNullError(aimAk, this)) return;
        if (DebugLogger.IsNullError(enemyWeaponInstance, this)) return;

        if (HAND > aimAk.solver.bones.Length)
        {
            DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(HAND)}={HAND} > {nameof(aimAk.solver.bones.Length)}={aimAk.solver.bones.Length}.", this);
            return;
        }

        aimAk.solver.transform = enemyWeaponInstance.AimTransform;

        var axis = isLeft ? aimIkAxisLeft : aimIkAxisRight;
        aimAk.solver.axis = CustomEnums.Instance.GetAxis(axis);

        var clampWeight = isLeft ? clampWeightLeft : clampWeightRight;
        aimAk.solver.clampWeight = clampWeight;
        
        var shoulderWeight = isLeft ? aimIkShoulderWeightLeft : aimIkShoulderWeightRight;
        var armWeight = isLeft ? aimIkArmWeightLeft : aimIkArmWeightRight;
        var forearmWeight = isLeft ? aimIkForearmWeightLeft : aimIkForearmWeightRight;
        var handWeight = isLeft ? aimIkHandWeightLeft : aimIkHandWeightRight;

        aimAk.solver.bones[SHOULDER].weight = shoulderWeight;
        aimAk.solver.bones[ARM].weight = armWeight;
        aimAk.solver.bones[FOREARM].weight = forearmWeight;
        aimAk.solver.bones[HAND].weight = handWeight;
    }
}
