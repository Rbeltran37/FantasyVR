using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemy/AnimatorHitReactionHandlerData", order = 1)]
public class AnimatorHitReactionHandlerData : ScriptableObject
{
    [SerializeField] private float minDamageThresholdHead;
    [SerializeField] private float strongHitThresholdHead;
    [SerializeField] private float minDamageThresholdHips;
    [SerializeField] private float strongHitThresholdHips;
    [SerializeField] private float minDamageThresholdArms;
    [SerializeField] private float strongHitThresholdArms;
    [SerializeField] private float minDamageThresholdLegs;
    [SerializeField] private float strongHitThresholdLegs;

    private int _hitHeadFrontId;
    private int _hitHeadFrontStrongId;
    private int _hitHeadBackId;
    private int _hitHeadBackStrongId;
    private int _hitHeadLeftId;
    private int _hitHeadLeftStrongId;
    private int _hitHeadRightId;
    private int _hitHeadRightStrongId;
    private int _hitHipsFrontId;
    private int _hitHipsFrontStrongId;
    private int _hitHipsBackId;
    private int _hitHipsBackStrongId;
    private int _hitHipsLeftId;
    private int _hitHipsLeftStrongId;
    private int _hitHipsRightId;
    private int _hitHipsRightStrongId;
    private int _hitArmLeftId;
    private int _hitArmLeftStrongId;
    private int _hitArmRightId;
    private int _hitArmRightStrongId;
    private int _hitLegLeftId;
    private int _hitLegLeftStrongId;
    private int _hitLegRightId;
    private int _hitLegRightStrongId;


    private Dictionary<int, Action<Animator, float, float>> _muscleDictionary = new Dictionary<int, Action<Animator, float, float>>();
    private Dictionary<int, int> _headWeakDictionary = new Dictionary<int, int>();
    private Dictionary<int, int> _headStrongDictionary = new Dictionary<int, int>();
    private Dictionary<int, int> _hipsWeakDictionary = new Dictionary<int, int>();
    private Dictionary<int, int> _hipsStrongDictionary = new Dictionary<int, int>();

    private const int HIPS = 0;
    private const int LEG_LEFT_LOWER = 1;
    private const int LEG_LEFT_UPPER = 2;
    private const int LEG_RIGHT_LOWER = 3;
    private const int LEG_RIGHT_UPPER = 4;
    private const int ARM_LEFT = 5;
    private const int FOREARM_LEFT = 6;
    private const int HEAD = 7;
    private const int ARM_RIGHT = 8;
    private const int FOREARM_RIGHT = 9;
    
    private const int ANGLE_SIZE_HEAD = 45;
    private const int ANGLE_SIZE_HIPS = 45;

    private const string HIT_HEAD_FRONT = "HitHeadFront";
    private const string HIT_HEAD_FRONT_STRONG = "HitHeadFrontStrong";
    private const string HIT_HEAD_BACK = "HitHeadBack";
    private const string HIT_HEAD_BACK_STRONG = "HitHeadBackStrong";
    private const string HIT_HEAD_LEFT = "HitHeadLeft";
    private const string HIT_HEAD_LEFT_STRONG = "HitHeadLeftStrong";
    private const string HIT_HEAD_RIGHT = "HitHeadRight";
    private const string HIT_HEAD_RIGHT_STRONG = "HitHeadRightStrong";
    private const string HIT_HIPS_FRONT = "HitHipsFront";
    private const string HIT_HIPS_FRONT_STRONG = "HitHipsFrontStrong";
    private const string HIT_HIPS_BACK = "HitHipsBack";
    private const string HIT_HIPS_BACK_STRONG = "HitHipsBackStrong";
    private const string HIT_HIPS_LEFT = "HitHipsLeft";
    private const string HIT_HIPS_LEFT_STRONG = "HitHipsLeftStrong";
    private const string HIT_HIPS_RIGHT = "HitHipsRight";
    private const string HIT_HIPS_RIGHT_STRONG = "HitHipsRightStrong";
    private const string HIT_ARM_LEFT = "HitArmLeft";
    private const string HIT_ARM_LEFT_STRONG = "HitArmLeftStrong";
    private const string HIT_ARM_RIGHT = "HitArmRight";
    private const string HIT_ARM_RIGHT_STRONG = "HitArmRightStrong";
    private const string HIT_LEG_LEFT = "HitLegLeft";
    private const string HIT_LEG_LEFT_STRONG = "HitLegLeftStrong";
    private const string HIT_LEG_RIGHT = "HitLegRight";
    private const string HIT_LEG_RIGHT_STRONG = "HitLegRightStrong";



    private void OnEnable()
    {
        _hitHeadFrontId = Animator.StringToHash(HIT_HEAD_FRONT);
        _hitHeadFrontStrongId = Animator.StringToHash(HIT_HEAD_FRONT_STRONG);
        _hitHeadBackId = Animator.StringToHash(HIT_HEAD_BACK);
        _hitHeadBackStrongId = Animator.StringToHash(HIT_HEAD_BACK_STRONG);
        _hitHeadLeftId = Animator.StringToHash(HIT_HEAD_LEFT);
        _hitHeadLeftStrongId = Animator.StringToHash(HIT_HEAD_LEFT_STRONG);
        _hitHeadRightId = Animator.StringToHash(HIT_HEAD_RIGHT);
        _hitHeadRightStrongId = Animator.StringToHash(HIT_HEAD_RIGHT_STRONG);
        _hitHipsFrontId = Animator.StringToHash(HIT_HIPS_FRONT);
        _hitHipsFrontStrongId = Animator.StringToHash(HIT_HIPS_FRONT_STRONG);
        _hitHipsBackId = Animator.StringToHash(HIT_HIPS_BACK);
        _hitHipsBackStrongId = Animator.StringToHash(HIT_HIPS_BACK_STRONG);
        _hitHipsLeftId = Animator.StringToHash(HIT_HIPS_LEFT);
        _hitHipsLeftId = Animator.StringToHash(HIT_HIPS_LEFT_STRONG);
        _hitHipsRightId = Animator.StringToHash(HIT_HIPS_RIGHT);
        _hitHipsRightStrongId = Animator.StringToHash(HIT_HIPS_RIGHT_STRONG);
        _hitArmLeftId = Animator.StringToHash(HIT_ARM_LEFT);
        _hitArmLeftStrongId = Animator.StringToHash(HIT_ARM_LEFT_STRONG);
        _hitArmRightId = Animator.StringToHash(HIT_ARM_RIGHT);
        _hitArmRightStrongId = Animator.StringToHash(HIT_ARM_RIGHT_STRONG);
        _hitLegLeftId = Animator.StringToHash(HIT_LEG_LEFT);
        _hitLegLeftStrongId = Animator.StringToHash(HIT_LEG_LEFT_STRONG);
        _hitLegRightId = Animator.StringToHash(HIT_LEG_RIGHT);
        _hitLegRightStrongId = Animator.StringToHash(HIT_LEG_RIGHT_STRONG);

        _muscleDictionary.Add(HIPS, HitHips);
        _muscleDictionary.Add(LEG_LEFT_LOWER, HitLegLeftLower);
        _muscleDictionary.Add(LEG_LEFT_UPPER, HitLegLeftUpper);
        _muscleDictionary.Add(LEG_RIGHT_LOWER, HitLegRightLower);
        _muscleDictionary.Add(LEG_RIGHT_UPPER, HitLegRightUpper);
        _muscleDictionary.Add(ARM_LEFT, HitArmLeft);
        _muscleDictionary.Add(FOREARM_LEFT, HitForearmLeft);
        _muscleDictionary.Add(HEAD, HitHead);
        _muscleDictionary.Add(ARM_RIGHT, HitArmRight);
        _muscleDictionary.Add(FOREARM_RIGHT, HitForearmRight);
        
        _headWeakDictionary.Add(0, _hitHeadFrontId);
        _headWeakDictionary.Add(1, _hitHeadLeftId);
        _headWeakDictionary.Add(2, _hitHeadLeftId);
        _headWeakDictionary.Add(-1, _hitHeadRightId);
        _headWeakDictionary.Add(-2, _hitHeadRightId);
        _headWeakDictionary.Add(3, _hitHeadBackId);
        _headWeakDictionary.Add(-3, _hitHeadBackId);

        _headStrongDictionary.Add(0, _hitHeadFrontStrongId);
        _headStrongDictionary.Add(1, _hitHeadLeftStrongId);
        _headStrongDictionary.Add(2, _hitHeadLeftStrongId);
        _headStrongDictionary.Add(-1, _hitHeadRightStrongId);
        _headStrongDictionary.Add(-2, _hitHeadRightStrongId);
        _headStrongDictionary.Add(3, _hitHeadBackStrongId);    
        _headStrongDictionary.Add(-3, _hitHeadBackStrongId);
        
        _hipsWeakDictionary.Add(0, _hitHipsFrontId);
        _hipsWeakDictionary.Add(1, _hitHipsLeftId);
        _hipsWeakDictionary.Add(2, _hitHipsLeftId);
        _hipsWeakDictionary.Add(-1, _hitHipsRightId);
        _hipsWeakDictionary.Add(-2, _hitHipsRightId);
        _hipsWeakDictionary.Add(3, _hitHipsBackId);
        _hipsWeakDictionary.Add(-3, _hitHipsBackId);
        
        _hipsStrongDictionary.Add(0, _hitHipsFrontStrongId);
        _hipsStrongDictionary.Add(1, _hitHipsLeftStrongId);
        _hipsStrongDictionary.Add(2, _hitHipsLeftStrongId);
        _hipsStrongDictionary.Add(-1, _hitHipsRightStrongId);
        _hipsStrongDictionary.Add(-2, _hitHipsRightStrongId);
        _hipsStrongDictionary.Add(3, _hitHipsBackStrongId);
        _hipsStrongDictionary.Add(-3, _hitHipsBackStrongId);
    }

    public void SetAnimation(Animator animator, int muscleIndex, float damage, Vector3 referenceForward, Vector3 referencePoint, Vector3 hitPoint)
    {
        if (DebugLogger.IsNullError(animator, this)) return;
        
        if (!_muscleDictionary.ContainsKey(muscleIndex))
        {
            DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"_muscleDictionary does not contain muscleIndex={muscleIndex} key.", this);
            return;
        }
        
        var hitAngle = GetHitAngle(referenceForward, referencePoint, hitPoint);        //TODO don't call yet

        _muscleDictionary[muscleIndex].Invoke(animator, damage, hitAngle);
    }

    private static float GetHitAngle(Vector3 referenceForward, Vector3 referencePoint, Vector3 hitPoint)
    {
        var forwardVector = referenceForward;
        var vector1 = new Vector2(forwardVector.x, forwardVector.z);
        var headingOfHit = VectorMath.GetTargetHeading(referencePoint, hitPoint);
        var vector2 = new Vector2(headingOfHit.x, headingOfHit.z);
        var hitAngle = VectorMath.AngleBetweenVector2(vector1, vector2);

        return hitAngle;
    }

    private void HitHips(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdHips) return;

        var directionIndex = GetIndexHips(angle);
        var triggerId = GetTriggerIdHips(damage, directionIndex);
        
        animator.SetTrigger(triggerId);
    }

    private void HitLegLeftLower(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdLegs) return;

        var triggerId = GetTriggerIdLegLeft(damage);
        animator.SetTrigger(triggerId);
    }

    private void HitLegLeftUpper(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdLegs) return;
        
        var triggerId = GetTriggerIdLegLeft(damage);
        animator.SetTrigger(triggerId);
    }

    private void HitLegRightLower(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdLegs) return;
        
        var triggerId = GetTriggerIdLegRight(damage);
        animator.SetTrigger(triggerId);
    }

    private void HitLegRightUpper(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdLegs) return;
        
        var triggerId = GetTriggerIdLegRight(damage);
        animator.SetTrigger(triggerId);
    }

    private void HitArmLeft(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdArms) return;
        
        var triggerId = GetTriggerIdArmLeft(damage);
        animator.SetTrigger(triggerId);
    }

    private void HitForearmLeft(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdArms) return;
        
        var triggerId = GetTriggerIdArmLeft(damage);
        animator.SetTrigger(triggerId);
    }
    
    private void HitHead(Animator animator, float damage, float hitAngle)
    {
        if (damage < minDamageThresholdHead) return;

        var directionIndex = GetIndexHead(hitAngle);
        var triggerId = GetTriggerIdHead(damage, directionIndex);

        animator.SetTrigger(triggerId);
        
        //HeadWasHit?.Invoke();
    }

    private void HitArmRight(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdArms) return;
        
        var triggerId = GetTriggerIdArmRight(damage);
        animator.SetTrigger(triggerId);
    }

    private void HitForearmRight(Animator animator, float damage, float angle)
    {
        if (damage < minDamageThresholdArms) return;
        
        var triggerId = GetTriggerIdArmRight(damage);
        animator.SetTrigger(triggerId);
    }

    private int GetIndexHead(float hitAngle)
    {
        return  (int) (hitAngle / ANGLE_SIZE_HEAD);
    }
    
    private int GetIndexHips(float hitAngle)
    {
        return  (int) (hitAngle / ANGLE_SIZE_HIPS);
    }

    private int GetTriggerIdHead(float damage, int directionIndex)
    {
        var headDictionary = damage < strongHitThresholdHead ? _headWeakDictionary : _headStrongDictionary;
        
        if (!headDictionary.ContainsKey(directionIndex))
        {
            DebugLogger.Debug("GetTriggerIdHead", $"_directionDictionaryHead does not contain directionIndex={directionIndex} key value.", this);
            return _hitHeadBackId;
        }
        
        return headDictionary[directionIndex];
    }
    
    private int GetTriggerIdHips(float damage, int directionIndex)
    {
        var hipsDictionary = damage < strongHitThresholdHips ? _hipsWeakDictionary : _hipsStrongDictionary;
        
        if (!hipsDictionary.ContainsKey(directionIndex))
        {
            DebugLogger.Debug("GetTriggerIdHips", $"hipsDictionary does not contain directionIndex={directionIndex} key value.", this);
            return _hitHeadBackId;
        }
        
        return hipsDictionary[directionIndex];
    }

    private int GetTriggerIdLegLeft(float damage)
    {
        var triggerId = damage < strongHitThresholdLegs ? _hitLegLeftId : _hitLegLeftStrongId;
        return triggerId;
    }
    
    private int GetTriggerIdLegRight(float damage)
    {
        var triggerId = damage < strongHitThresholdLegs ? _hitLegRightId : _hitLegRightStrongId;
        return triggerId;
    }
    
    private int GetTriggerIdArmLeft(float damage)
    {
        var triggerId = damage < strongHitThresholdArms ? _hitArmLeftId : _hitArmLeftStrongId;
        return triggerId;
    }
    
    private int GetTriggerIdArmRight(float damage)
    {
        var triggerId = damage < strongHitThresholdArms ? _hitArmRightId : _hitArmRightStrongId;
        return triggerId;
    }
}
