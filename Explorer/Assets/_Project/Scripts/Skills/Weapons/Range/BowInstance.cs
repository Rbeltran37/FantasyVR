using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class BowInstance : WeaponInstance
{
    [SerializeField] private BowSO bowSo;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform socket;
    [SerializeField] private Transform pullStart;
    [SerializeField] private Transform pullEnd;
    [SerializeField] private Transform bowGripTarget;

    public ControllerHaptics ControllerHaptics;

    private Arrow _currentArrow;
    private Coroutine _coroutine;
    private Transform _gripHand;
    private Transform _pullTarget;
    private Vector3 _initialLocalEuler;
    private Vector3 _arrowLocalPosition;
    private float _currentPullValue;
    private int _arrowCount = INFINITE_ARROWS;        //TODO possibly set to 0, create method for enemy to call for infinite arrows
    private int _lastHapticsIndex;
    private float _hapticTriggerIncrement;
    private float _drawStringPitchIncrement;
    private bool _isLeftHandGripHand = true;
    private bool _isUsingAbility;
    private float _speed;
    
    private const int INFINITE_ARROWS = Int32.MaxValue;
    private const int CAN_CAST_INTERVAL = 1;
    private const int DEACTIVATE_ABILITY_FALLBACK_DELAY = 5;

    public Action HasFired;


    protected override void Awake()
    {
        base.Awake();
        
        if (DebugLogger.IsNullError(bowSo, this)) return;
        if (DebugLogger.IsNullError(bowSo.ArrowObjectPool, this)) return;

        InitializeHapticIncrements();
        bowSo.InitializeAnimationIds();
        bowSo.ArrowObjectPool.InitializePool();

        _arrowLocalPosition = new Vector3(0, 0, bowSo.arrowSpawnLocalZPosition);
        
        WeaponAbilityData = new BowAbilityData(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        if (!_currentArrow) return;
        
        _currentArrow.Despawn();

        _currentArrow = null;
        _isUsingAbility = false;
    }

    private void Start()
    {
        _initialLocalEuler = transform.localEulerAngles;
    }

    private void Update()
    {
        // Only update if pulling, and we have an arrow
        if (!_pullTarget || !_currentArrow)
            return;

        // Pull value
        _currentPullValue = CalculatePull(_pullTarget);
        _currentPullValue = Mathf.Clamp(_currentPullValue, 0.0f, 1.0f);
        
        SetPullDirection();
        SetGripRotation();
        
        // Apply to animator
        animator.SetFloat(bowSo.pullAnimationParameterId, _currentPullValue);
    }

    private void InitializeHapticIncrements()
    {
        if (DebugLogger.IsNullError(bowSo, this, "Must be set in editor.")) return;
        
        _hapticTriggerIncrement = 1.0f / bowSo.numHapticPoints;
        _drawStringPitchIncrement = (bowSo.drawStringPitch.maxValue - bowSo.drawStringPitch.minValue) /
                                    bowSo.numHapticPoints;
    }

    public void Equip(PlayerHand playerHand, int count)
    {
        _arrowCount = count;
        
        _gripHand = playerHand.GetPuppetHandTransform();
        _isLeftHandGripHand = playerHand.IsLeftHand();
        ControllerHaptics = playerHand.GetControllerHaptics();
        _isUsingAbility = false;

        WeaponAbility.Deactivate(WeaponAbilityData);
        
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(CreateArrow(bowSo.arrowRespawnDelay));
    }

    //Used by ArcherAi
    public void CreateArrow()
    {
        _arrowCount = INFINITE_ARROWS;
        CoroutineCaller.Instance.StartCoroutine(CreateArrow(0));
    }
    
    private IEnumerator CreateArrow(float waitTime)
    {
        if (_arrowCount <= 0) yield break;

        while (Skill && !Skill.CanCast() || _isUsingAbility)
        {
            yield return new WaitForSeconds(CAN_CAST_INTERVAL);
        }
        
        if (_arrowCount <= 0) yield break;
        
        // Create, and child
        var pooledObject = bowSo.ArrowObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) yield break; 

        // Set
        _currentArrow = (Arrow) pooledObject;
        if (DebugLogger.IsNullError(_currentArrow, this)) yield break; 
            
        _currentArrow.SetParent(socket, _currentArrow.GetTransform().localPosition, _currentArrow.GetTransform().localRotation, false);
        
        // Wait
        yield return new WaitForSeconds(waitTime);
        
        _currentArrow.Spawn(socket, _arrowLocalPosition, Quaternion.identity, false);
        
        if (_arrowCount.Equals(INFINITE_ARROWS)) yield break;
        
        _arrowCount--;
    }

    private float CalculatePull(Transform pullHand)
    {
        //check if hand is ahead of pullStart position, and thus invalid
        if (Math.Abs(VectorMath.IsTargetAhead(pullHand.position - pullStart.position, pullStart) - 1) < Mathf.Epsilon)
            return 0;
        
        // Direction, and size
        var pullStartPosition = pullStart.position;
        Vector3 direction = pullEnd.position - pullStartPosition;
        float magnitude = direction.magnitude;

        // Difference
        direction.Normalize();
        Vector3 difference = pullHand.position - pullStartPosition;

        PullFeedback(difference);

        // Right angle, value of 0 -  1
        return Vector3.Dot(difference, direction) / magnitude;
    }
    
    private void PullFeedback(Vector3 difference)
    {
        float differenceMagnitude = difference.magnitude * 2;
        
        var currentHapticsIndex = CalculateCurrentHapticsIndex(differenceMagnitude);
        if (Math.Abs(currentHapticsIndex - _lastHapticsIndex) > Mathf.Epsilon)    //currentIndex is not lastIndex
        {
            SkillAudioSource.clip = bowSo.drawBackClip;
            SkillAudioSource.pitch = bowSo.drawStringPitch.minValue + (_drawStringPitchIncrement * currentHapticsIndex);
            SkillAudioSource.Play();

            if (_lastHapticsIndex > -1)    //check if arrow was already knocked
            {
                float currentAmplitude;
                float currentDuration;
                if (currentHapticsIndex == bowSo.numHapticPoints - 1)    //is string fully drawn
                {
                    currentAmplitude = bowSo.fullDrawHapticAmplitude;
                    currentDuration = bowSo.fullDrawHapticDuration;
                }
                else
                {
                    currentAmplitude = bowSo.drawStringHapticAmplitude;
                    currentDuration = bowSo.drawStringHapticDuration;
                }
            
                //Apply haptics to both hands
                if (ControllerHaptics)
                {
                    ControllerHaptics.ActivateHaptics(currentAmplitude, currentDuration);
                    ControllerHaptics.ActivateHaptics(currentAmplitude, currentDuration);
                }
            }

            _lastHapticsIndex = currentHapticsIndex;
        }
    }

    private int CalculateCurrentHapticsIndex(float differenceMagnitude)
    {
        var currentHapticsIndex = (int) (differenceMagnitude / _hapticTriggerIncrement);
        currentHapticsIndex = Mathf.Clamp(currentHapticsIndex, 0, bowSo.numHapticPoints - 1);
        return currentHapticsIndex;
    }

    private void SetPullDirection()
    {
        var pullVector = bowGripTarget.position - _pullTarget.position;
        transform.forward = pullVector;
    }
    
    private void SetGripRotation()
    {
        var sign = _isLeftHandGripHand ? -1 : 1;
        var gripAngle = sign * _gripHand.eulerAngles.x + _initialLocalEuler.z;
        
        //Avoid gimble lock by checking which side gripHand.up is of Vector3.up
        if (Vector3.Dot(_gripHand.up, Vector3.up) < 0)
        {
            gripAngle *= -1;
        }

        ThisTransform.rotation *= Quaternion.Euler(0, 0, gripAngle);
    }

    public void Pull(Transform hand)
    {
        // Simple distance check
        var distance = Vector3.Distance(hand.position, pullStart.position);

        if (distance > bowSo.grabThreshold)
            return;

        // Set
        _pullTarget = hand;
        
        //Arrow Knocking Haptics on draw hand
        if (ControllerHaptics) ControllerHaptics.GetOtherControllerHaptics().ActivateHaptics(bowSo.arrowKnockingAmplitude, bowSo.arrowKnockingDuration);
    }

    public void Release()
    {
        if (!_currentArrow) return;
        
        //load audio clip for bow release
        SkillAudioSource.clip = bowSo.releaseClip;

        // If we've pulled far enough, fire
        if (_currentPullValue > bowSo.releaseThreshold)
        {
            //After firing arrow we play the release sound of the bow
            SkillAudioSource.Play();
            FireArrow();
        }
        
        // Clear
        _pullTarget = null;

        // Pull
        _currentPullValue = 0.0f;
        animator.SetFloat(bowSo.pullAnimationParameterId, 0);
        _lastHapticsIndex = -1;

        // Create new arrow, with delay
        if (!_currentArrow) StartCoroutine(CreateArrow(bowSo.arrowRespawnDelay));

        //Fix this adjustment of resetting rotation after release
        GetTransform().localEulerAngles = _initialLocalEuler;
    }

    public void FireArrow(float pullValue)
    {
        _currentPullValue = pullValue;
        _currentArrow.SetSpeed(_speed);
        FireArrow();
    }

    private void FireArrow()
    {
        if (Skill)
        {
            Skill.ApplyModifiers(this);
            Skill.Cast();
        }
        
        _currentArrow.SetSpeed(_speed);
        _currentArrow.Fire(_currentPullValue);
        
        HasFired?.Invoke();
        _currentArrow = null;

        if (ControllerHaptics) ControllerHaptics.ActivateHaptics(bowSo.arrowReleaseHapticAmplitude, 
            bowSo.arrowReleaseHapticDuration);
    }

    [Button]
    public override void UseAbility()
    {
        if (!_currentArrow) return;

        base.UseAbility();

        _isUsingAbility = true;

        StartCoroutine(DeactivateAbilityCoroutine());
    }

    private IEnumerator DeactivateAbilityCoroutine()
    {
        yield return new WaitForSeconds(DEACTIVATE_ABILITY_FALLBACK_DELAY);

        _isUsingAbility = false;
    }

    public Arrow GetCurrentArrow()
    {
        return _currentArrow;
    }

    public int GetArrowCount()
    {
        return _arrowCount;
    }
    
    public SkillModifierContainer GetArrowSkillModifierContainer()
    {
        if (DebugLogger.IsNullError(_currentArrow, this)) return null;
        
        return _currentArrow.GetSkillModifierContainer();
    }
    
    public override void ApplyModifiers(ModifierData modifierData)
    {
        base.ApplyModifiers(modifierData);
        
        GetArrowSkillModifierContainer().ApplyModifiers(modifierData);
    }

    public void DeactivateAbility()
    {
        _isUsingAbility = false;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public float GetPullValue()
    {
        return _currentPullValue;
    }
    
    
    public BowAbilityData GetBowAbilityData()
    {
        if (WeaponAbilityData == null) WeaponAbilityData = new BowAbilityData(this);
        
        return (BowAbilityData) WeaponAbilityData;
    }
}
