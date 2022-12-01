using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class DebugPuppet : MonoBehaviour
{
    [SerializeField] private BehaviourPuppet _behaviourPuppet;
    [SerializeField] private PuppetMaster _puppetMaster;

    [Header("Behaviour Puppet")] 
    [SerializeField] [Range(0, 50)] private float _collisionThreshold = 0;
    [SerializeField] private bool _useCurveForCollisionResistence = true;
    [SerializeField] [Range(0, 2000)] private float _collisionResistance = 700;
    [SerializeField] [Range(1, 30)] private int _maxCollisions = 3;
    [SerializeField] [Range(0, 10)] private float _regainPinSpeed = 1;
    [SerializeField] [Range(0, 10)] private float _knockoutDistance = 1.5f;
    [SerializeField] [Range(0, 1)] private float _unpinnedMuscleWeight = .4f;
    [SerializeField] private float _maxRigidbodyVelocity = 7;
    [SerializeField] [Range(0, 1)] private float _pinWeightThreshold = 1f;
    [SerializeField] private float _head;
    
    
    [Header("Puppet Master")] 
    [SerializeField] private bool _fixTargetTransforms = true;
    [SerializeField] [Range(1, 10)] private int _solverIterationCount = 3;
    [SerializeField] [Range(0, 1)] private float _mappingWeight = 1;
    [SerializeField] [Range(0, 1)] private float _pinWeight = 1;
    [SerializeField] [Range(0, 1)] private float _muscleWeight = 1;
    [SerializeField] [Range(0, 1000)] private float _muscleSpring = 100;
    [SerializeField] private float _muscleDamper = 0;
    [SerializeField] [Range(0, 10)] private float _pinPower = 4;
    [SerializeField] [Range(0, 100)] private float _pinDistanceFalloff = 5;

    private const float TinyAmount = .1f;
    private const float SmallAmount = 1;
    private const float MediumAmount = 10;
    private const float LargeAmount = 100;


    // Start is called before the first frame update
    void Start()
    {
        UpdateParameters();
    }

    #region DebugUpdate
    
    private void UpdateBehaviourPuppet()
    {
        if (!_behaviourPuppet) return;
        
        _behaviourPuppet.collisionThreshold = _collisionThreshold;
        
        if (!_useCurveForCollisionResistence)
            _behaviourPuppet.collisionResistance.floatValue = _collisionResistance;
        
        _behaviourPuppet.maxCollisions = _maxCollisions;
        _behaviourPuppet.regainPinSpeed = _regainPinSpeed;
        _behaviourPuppet.knockOutDistance = _knockoutDistance;
        _behaviourPuppet.unpinnedMuscleWeightMlp = _unpinnedMuscleWeight;
        _behaviourPuppet.maxRigidbodyVelocity = _maxRigidbodyVelocity;
        _behaviourPuppet.pinWeightThreshold = _pinWeightThreshold;
    }

    private void UpdatePuppetMaster()
    {
        _puppetMaster.fixTargetTransforms = _fixTargetTransforms;
        _puppetMaster.solverIterationCount = _solverIterationCount;
        _puppetMaster.mappingWeight = _mappingWeight;
        _puppetMaster.pinWeight = _pinWeight;
        _puppetMaster.muscleWeight = _muscleWeight;
        _puppetMaster.muscleSpring = _muscleSpring;
        _puppetMaster.muscleDamper = _muscleDamper;
        _puppetMaster.pinPow = _pinPower;
        _puppetMaster.pinDistanceFalloff = _pinDistanceFalloff;
    }
    
    [Button]
    public void UpdateParameters()
    {
        UpdateBehaviourPuppet();
        UpdatePuppetMaster();
    }

    #endregion
    
    #region BehaviourPuppet

    
    public void AlterBehaviourPuppetCollisionThresholdAdd(Text text)
    {
        var value = _collisionThreshold;
        var changeAmount = TinyAmount;
        value += changeAmount;
        _collisionThreshold = value;
        UpdateBehaviourPuppet();
        text.text = "Collision Threshold++: " + _behaviourPuppet.collisionThreshold;
    }
    
    public void AlterBehaviourPuppetCollisionThresholdSubtract(Text text)
    {
        var value = _collisionThreshold;
        var changeAmount = TinyAmount;
        value -= changeAmount;
        if (value < 0)
            value = 0;
        _collisionThreshold = value;
        UpdateBehaviourPuppet();
        text.text = "Collision Threshold--: " + _behaviourPuppet.collisionThreshold;
    }

    public void AlterBehaviourPuppetCollisionResistanceAdd(Text text)
    {
        bool isIncrementing = true;
        var value = _collisionResistance;
        var changeAmount = LargeAmount;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _collisionResistance = value;
        UpdateBehaviourPuppet();
        text.text = "Collision Resistence++: " + _behaviourPuppet.collisionResistance.floatValue;
    }

    public void AlterBehaviourPuppetCollisionResistanceSubtract(Text text)
    {
        bool isIncrementing = false;
        var value = _collisionResistance;
        var changeAmount = LargeAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _collisionResistance = value;
        UpdateBehaviourPuppet();
        text.text = "Collision Resistence" + addOnString + ": " + _behaviourPuppet.collisionResistance.floatValue;
    }

    public void AlterBehaviourPuppetMaxCollisionsAdd(Text text)
    {
        bool isIncrementing = true;
        var value = _maxCollisions;
        var changeAmount = 1;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _maxCollisions = value;
        UpdateBehaviourPuppet();
        text.text = "Max Collisions" + addOnString + ": " + _behaviourPuppet.maxCollisions;
    }
    
    
    public void AlterBehaviourPuppetMaxCollisionsSubtract(Text text)
    {
        bool isIncrementing = false;
        var value = _maxCollisions;
        var changeAmount = 1;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _maxCollisions = value;
        UpdateBehaviourPuppet();
        text.text = "Max Collisions" + addOnString + ": " + _behaviourPuppet.maxCollisions;
    }
    
    public void AlterBehaviourPuppetRegainPinSpeedAdd(Text text)
    {
        bool isIncrementing = true;
        var value = _regainPinSpeed;
        var changeAmount = SmallAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _regainPinSpeed = value;
        UpdateBehaviourPuppet();
        text.text = "RegainPinSpeed" + addOnString + ": " + _behaviourPuppet.regainPinSpeed;
    }
    
    
    public void AlterBehaviourPuppetRegainPinSpeedSubtract(Text text)
    {
        bool isIncrementing = false;
        var value = _regainPinSpeed;
        var changeAmount = SmallAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _regainPinSpeed = value;
        UpdateBehaviourPuppet();
        text.text = "RegainPinSpeed" + addOnString + ": " + _behaviourPuppet.regainPinSpeed;
    }
    
    public void AlterBehaviourPuppetKnockoutDistanceAdd(Text text)
    {
        bool isIncrementing = true;
        var value = _knockoutDistance;
        var changeAmount = TinyAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _knockoutDistance = value;
        UpdateBehaviourPuppet();
        text.text = "KnockoutDistance" + addOnString + ": " + _behaviourPuppet.knockOutDistance;
    }
    
    
    public void AlterBehaviourPuppetKnockoutDistanceSubtract(Text text)
    {
        bool isIncrementing = false;
        var value = _knockoutDistance;
        var changeAmount = TinyAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _knockoutDistance = value;
        UpdateBehaviourPuppet();
        text.text = "KnockoutDistance" + addOnString + ": " + _behaviourPuppet.knockOutDistance;
    }
    
    public void AlterBehaviourPuppetUnpinnedMuscleWeightAdd(Text text)
    {
        bool isIncrementing = true;
        var value = _unpinnedMuscleWeight;
        var changeAmount = TinyAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _unpinnedMuscleWeight = value;
        UpdateBehaviourPuppet();
        text.text = "UnpinnedMuscleWeight" + addOnString + ": " + _behaviourPuppet.unpinnedMuscleWeightMlp;
    }
    
    
    public void AlterBehaviourPuppetUnpinnedMuscleWeightSubtract(Text text)
    {
        bool isIncrementing = false;
        var value = _unpinnedMuscleWeight;
        var changeAmount = TinyAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _unpinnedMuscleWeight = value;
        UpdateBehaviourPuppet();
        text.text = "UnpinnedMuscleWeight" + addOnString + ": " + _behaviourPuppet.unpinnedMuscleWeightMlp;
    }
    
    public void AlterBehaviourPuppetMaxRigidbodyVelocityAdd(Text text)
    {
        bool isIncrementing = true;
        var value = _maxRigidbodyVelocity;
        var changeAmount = SmallAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _maxRigidbodyVelocity = value;
        UpdateBehaviourPuppet();
        text.text = "MaxRigidbodyVelocity" + addOnString + ": " + _behaviourPuppet.maxRigidbodyVelocity;
    }
    
    public void AlterBehaviourPuppetMaxRigidbodyVelocitySubtract(Text text)
    {
        bool isIncrementing = false;
        var value = _maxRigidbodyVelocity;
        var changeAmount = SmallAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _maxRigidbodyVelocity = value;
        UpdateBehaviourPuppet();
        text.text = "MaxRigidbodyVelocity" + addOnString + ": " + _behaviourPuppet.maxRigidbodyVelocity;
    }
    
    public void AlterBehaviourPuppetPinWeightThresholdAdd(Text text)
    {
        bool isIncrementing = true;
        var value = _pinWeightThreshold;
        var changeAmount = TinyAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _pinWeightThreshold = value;
        UpdateBehaviourPuppet();
        text.text = "PinWeightThreshold" + addOnString + ": " + _behaviourPuppet.pinWeightThreshold;
    }
    
    public void AlterBehaviourPuppetPinWeightThresholdSubtract(Text text)
    {
        bool isIncrementing = false;
        var value = _pinWeightThreshold;
        var changeAmount = TinyAmount;
        string addOnString = "++";
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
            addOnString = "--";
        }

        if (value < 0)
            value = 0;
        _pinWeightThreshold = value;
        UpdateBehaviourPuppet();
        text.text = "PinWeightThreshold" + addOnString + ": " + _behaviourPuppet.pinWeightThreshold;
    }

    #endregion

    #region PuppetMaster

    
    public void AlterPuppetMasterFixTargetTransforms(bool state)
    {
        _fixTargetTransforms = state;
        UpdatePuppetMaster();
    }
    
    public void AlterPuppetMasterSolverIterationCount(bool isIncrementing)
    {
        var value = _solverIterationCount;
        var changeAmount = 1;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _solverIterationCount = value;
        UpdatePuppetMaster();
    }

    public void AlterPuppetMasterMappingWeight(bool isIncrementing)
    {
        var value = _mappingWeight;
        var changeAmount = TinyAmount;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _mappingWeight = value;
        UpdatePuppetMaster();
    }

    public void AlterPuppetMasterPinWeight(bool isIncrementing)
    {
        var value = _pinWeight;
        var changeAmount = TinyAmount;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _pinWeight = value;
        UpdatePuppetMaster();
    }

    public void AlterPuppetMasterMuscleWeight(bool isIncrementing)
    {
        var value = _muscleWeight;
        var changeAmount = TinyAmount;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _muscleWeight = value;
        UpdatePuppetMaster();
    }
    
    public void AlterPuppetMasterMuscleSpring(bool isIncrementing)
    {
        var value = _muscleSpring;
        var changeAmount = LargeAmount;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _muscleSpring = value;
        UpdatePuppetMaster();
    }

    public void AlterPuppetMasterMuscleDamper(bool isIncrementing)
    {
        var value = _muscleDamper;
        var changeAmount = MediumAmount;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _muscleDamper = value;
        UpdatePuppetMaster();
    }

    public void AlterPuppetMasterPinPower(bool isIncrementing)
    {
        var value = _pinPower;
        var changeAmount = SmallAmount;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _pinPower = value;
        UpdatePuppetMaster();
    }

    public void AlterPuppetMasterPinDistanceFalloff(bool isIncrementing)
    {
        var value = _pinDistanceFalloff;
        var changeAmount = SmallAmount;
        if (isIncrementing)
            value += changeAmount;
        else
        {
            value -= changeAmount;
        }

        if (value < 0)
            value = 0;
        _pinDistanceFalloff = value;
        UpdatePuppetMaster();
    }

    #endregion

}
