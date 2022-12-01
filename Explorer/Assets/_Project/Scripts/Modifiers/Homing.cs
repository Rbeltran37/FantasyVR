using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class Homing : SkillAbility
{
    [SerializeField] private TargetAcquisition[] targetAcquisitions;
    [SerializeField] private Transform homingTransform;
    [SerializeField] private Rigidbody homingRigidbody;
    [SerializeField] private float timeStep = .02f;
    [SerializeField] private float turnRate = .5f;
    
    private Transform _target;
    private float _currentTurnRate;
    private float _multiplier;


    private void OnEnable()
    {
        Activate();
    }

    private void OnDisable()
    {
        _multiplier = NOT_APPLIED;
    }

    public override float GetValue()
    {
        return _multiplier;
    }

    public override void SetValue(float value)
    {
        _multiplier = value;
        _currentTurnRate = turnRate * _multiplier;
    }

    protected override void Activate()
    {
        if (Level == NOT_APPLIED) return;
        
        if (DebugLogger.IsNullError(targetAcquisitions, this)) return;
        if (DebugLogger.IsNullError(homingRigidbody, this)) return;

        foreach (var targetAcquisition in targetAcquisitions)
        {
            _target = targetAcquisition.AcquireTarget(homingTransform);
            if (_target) break;
        }
        
        if (!_target) return;
        
        StartCoroutine(HomeInOnTargetCoroutine());
    }

    private IEnumerator HomeInOnTargetCoroutine()
    {
        while (_target)
        {
            SeekAndFace();
            StartCoroutine(PursueTargetCoroutine());
            yield return new WaitForSeconds(timeStep);
        }
    }

    private void SeekAndFace() 
    {
        if (!_target) return;
        
        var heading = VectorMath.GetTargetHeading(homingTransform, _target);
        float headingValue = VectorMath.IsTargetAhead(heading, homingTransform);
        if (Math.Abs(headingValue - 1) < Mathf.Epsilon) {

            headingValue = VectorMath.IsTargetToTheLeftOrRight(heading, homingTransform);
            var amountTurnedHorizontally = 0f;
            if (Math.Abs(headingValue - 1) < Mathf.Epsilon) {

                amountTurnedHorizontally = _currentTurnRate;
            }
            else if (Math.Abs(headingValue - (-1)) < Mathf.Epsilon) {

                amountTurnedHorizontally = -_currentTurnRate;
            }

            headingValue = VectorMath.IsTargetAboveOrBelow(heading, homingTransform);
            var amountTurnedVertically = 0f;
            if (Math.Abs(headingValue - 1) < Mathf.Epsilon) {

                amountTurnedVertically = _currentTurnRate;
            }
            else if (Math.Abs(headingValue - (-1)) < Mathf.Epsilon) {

                amountTurnedVertically = -_currentTurnRate;
            }

            ApplyTurn(amountTurnedVertically, amountTurnedHorizontally);
        }
    }

    private void ApplyTurn(float vertical, float horizontal) 
    {
        homingTransform.localEulerAngles += new Vector3(vertical, horizontal, 0);
    }

    private IEnumerator PursueTargetCoroutine()
    {
        yield return new WaitForFixedUpdate();
        
        if (homingRigidbody)
        {
            homingRigidbody.velocity = homingTransform.forward * homingRigidbody.velocity.magnitude;
        }
    }
}
