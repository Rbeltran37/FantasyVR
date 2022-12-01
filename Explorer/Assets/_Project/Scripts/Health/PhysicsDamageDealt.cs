using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zinnia.Tracking.Velocity;

public class PhysicsDamageDealt : PUNDamageDealt
{
    [SerializeField] private AverageVelocityEstimator averageVelocityEstimator;
    [SerializeField] private RangedInt velocityRangedInt;
    
    
    public void SetAverageVelocityEstimator(AverageVelocityEstimator averageVelocityEstimator)
    {
        this.averageVelocityEstimator = averageVelocityEstimator;
    }

    public float GetDamageMultiplier()
    {
        var velocityMagnitude = averageVelocityEstimator.GetVelocity().magnitude;
        var velocityRange = velocityRangedInt.maxValue - velocityRangedInt.minValue;
        var velocityPercentage = velocityMagnitude / velocityRange;
        velocityPercentage = Mathf.Clamp(velocityPercentage, velocityRangedInt.minValue, velocityRangedInt.maxValue);

        var damage = damageDealtData.damage * velocityPercentage;

        return damage;
    }
}
