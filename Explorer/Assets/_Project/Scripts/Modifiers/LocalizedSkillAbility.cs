using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LocalizedSkillAbility : SkillAbility
{
    public ElementFxSO elementFxSO;       //For lookup purposes
    
    [SerializeField] protected ObjectPool ObjectPool;


    protected override void Awake()
    {
        if (DebugLogger.IsNullError(ObjectPool, "Must set in editor.", this)) return;
        
        ObjectPool.InitializePool();
        
        base.Awake();
    }

    protected override void Activate()
    {
        if (Level == NOT_APPLIED) return;
        
        if (DebugLogger.IsNullError(ObjectPool, "Must be set in editor.", this)) return;

        var pooledObject = ObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return;

        var pooledSkillAbility = (PooledSkillAbility) pooledObject;
        if (DebugLogger.IsNullError(pooledSkillAbility, this)) return;
        
        pooledSkillAbility.SetElementFxSO(elementFxSO);
        pooledSkillAbility.SetValue(Value);
        pooledSkillAbility.Spawn(Quaternion.identity, SpawnPosition, true);
    }
    
    public void ActivateAtPosition(Vector3 position)
    {
        if (Level == NOT_APPLIED) return;

        SpawnPosition = position;
        
        Activate();
    }
}
