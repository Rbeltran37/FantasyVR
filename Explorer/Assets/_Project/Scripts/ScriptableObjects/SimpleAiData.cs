using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/SimpleAiData", order = 0)]
public class SimpleAiData : ScriptableObject
{
    public int numAttacks = 13;
    public int numShortAttacks = 5;
    public int numMediumAttacks = 5;
    public int numLongAttacks = 3;
    public int numProjectileAttacks = 0;
    public int isStunnedBoolId;
    public int actionIndexId;
    public int shortAttackMin = 0;
    public int shortAttackMax = 0;
    public int mediumAttackMin;
    public int mediumAttackMax;
    public int longAttackMin;
    public int longAttackMax;
    public int projectileAttackMin;
    public int projectileAttackMax;
    public int numBackUpClips = 3;
    public int backUpIndexId;
    public int blockBoolId;
    public int isParriedId;
    public int dodgeForwardTriggerId;
    public int dodgeLeftTriggerId;
    public int dodgeRightTriggerId;
    public int isPrimaryEquippedBoolId;
    public int isSecondaryEquippedBoolId;
    public int equipPrimaryTriggerId;
    public int equipSecondaryTriggerId;
    public CustomEnums.Axis blockAxis;
    public float blockClampWeight = .2f;
    public float[] blockWeights = new []{ .6f, .8f, 1, 1 };
    public CustomEnums.Axis attackAxis;
    public float attackClampWeight = .8f;
    public float[] attackWeights = new []{ .2f, .2f, .4f, .6f };
    public AnimationReferenceHelper.Handedness blockHandedness;
    [Range(0, .1f)] public float accuracy = 0;

    private const string IS_STUNNED_BOOL_STRING = "IsStunned";
    private const string ACTION_INDEX_STRING = "ActionIndex";
    private const string BACK_UP_INDEX_STRING = "BackUpIndex";
    private const string BLOCK_BOOL_STRING = "Block";
    private const string IS_PARRIED_STRING = "IsParried";
    private const string DODGE_FORWARD_TRIGGER_STRING = "DodgeForward";
    private const string DODGE_LEFT_TRIGGER_STRING = "DodgeLeft";
    private const string DODGE_RIGHT_TRIGGER_STRING = "DodgeRight";
    private const string IS_PRIMARY_EQUIPPED_STRING = "IsPrimaryWeaponEquipped";
    private const string IS_SECONDARY_EQUIPPED_STRING = "IsSecondaryWeaponEquipped";
    private const string EQUIP_PRIMARY_TRIGGER_STRING = "EquipPrimary";
    private const string EQUIP_SECONDARY_TRIGGER_STRING = "EquipSecondary";


    private void OnEnable()
    {
        InitializeAnimationIds();
    }

    public void InitializeAnimationIds()
    {
        isStunnedBoolId = Animator.StringToHash(IS_STUNNED_BOOL_STRING);
        actionIndexId = Animator.StringToHash(ACTION_INDEX_STRING);
        backUpIndexId = Animator.StringToHash(BACK_UP_INDEX_STRING);
        isParriedId = Animator.StringToHash(IS_PARRIED_STRING);
        blockBoolId = Animator.StringToHash(BLOCK_BOOL_STRING);
        dodgeForwardTriggerId = Animator.StringToHash(DODGE_FORWARD_TRIGGER_STRING);
        dodgeLeftTriggerId = Animator.StringToHash(DODGE_LEFT_TRIGGER_STRING);
        dodgeRightTriggerId = Animator.StringToHash(DODGE_RIGHT_TRIGGER_STRING);
        isPrimaryEquippedBoolId = Animator.StringToHash(IS_PRIMARY_EQUIPPED_STRING);
        isSecondaryEquippedBoolId = Animator.StringToHash(IS_SECONDARY_EQUIPPED_STRING);
        equipPrimaryTriggerId = Animator.StringToHash(EQUIP_PRIMARY_TRIGGER_STRING);
        equipSecondaryTriggerId = Animator.StringToHash(EQUIP_SECONDARY_TRIGGER_STRING);

        shortAttackMax = numShortAttacks;
        mediumAttackMin = shortAttackMax;
        mediumAttackMax = mediumAttackMin + numMediumAttacks;
        longAttackMin = mediumAttackMax;
        longAttackMax = longAttackMin + numLongAttacks;
        projectileAttackMin = longAttackMax;
        projectileAttackMax = projectileAttackMin + numProjectileAttacks;
    }
}