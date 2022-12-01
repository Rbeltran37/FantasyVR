using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MultiShotArrow : SkillAbility
{
    [SerializeField] private BowInstance bowInstance;
    [SerializeField] private ObjectPool spawnedArrowsObjectPool;
    [SerializeField] private float increment = 3;
    
    private Skill _skill => _bowAbilityData.Skill;
    private BowAbilityData _bowAbilityData => bowInstance.GetBowAbilityData();
    private Arrow _activatedArrow => _bowAbilityData.Arrow;
    private Vector3 _arrowSpawnOriginPosition;


    protected override void Awake()
    {
        if (DebugLogger.IsNullError(spawnedArrowsObjectPool, this)) return;

        spawnedArrowsObjectPool.InitializePool();

        bowInstance.HasFired += Activate;
        
        base.Awake();
    }

    private bool CanCast()
    {
        return _skill.CanCast();
    }

    private void Cast()
    {
        _skill.Cast();
    }
    
    protected override void Activate()
    {
        if (Level == NOT_APPLIED) return;

        FireArrows();
    }

    private void FireArrows()
    {
        var activatedArrowTransform = _activatedArrow.transform;
        var spawnPosition = activatedArrowTransform.position;
        var startRotation = activatedArrowTransform.rotation;

        var maxArrows = Value <= _bowAbilityData.ArrowCount ? Value : _bowAbilityData.ArrowCount;

        var currentOffset = increment;
        var numArrowsSpawned = 0;
        var sign = 1;
        while (numArrowsSpawned < maxArrows && CanCast())
        {
            var pooledObject = spawnedArrowsObjectPool.GetPooledObject();

            Cast();

            var targetRotation = startRotation * Quaternion.Euler(currentOffset, 0, 0);

            pooledObject.Spawn(null, spawnPosition, targetRotation, true); //lifetime is set to 0 to allow the Arrow script to handle lifetime
            Quaternion.Slerp(pooledObject.GetTransform().rotation, targetRotation, 1);
            
            //Gets Arrow script attached to this Arrow and fires it.
            var currentArrow = pooledObject.GetComponent<Arrow>();
            if (DebugLogger.IsNullError(currentArrow, this)) return;

            currentArrow.Fire(bowInstance.GetPullValue());

            sign *= -1;
            currentOffset *= sign;
            if (currentOffset > 0) currentOffset += increment;
            numArrowsSpawned++;
        }
    }
}
