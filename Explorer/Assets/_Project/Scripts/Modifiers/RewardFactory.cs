using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

//TODO spawn other reward types, like life/mana.
//TODO don't create rewards in runtime, get from prefabs
public class RewardFactory : MonoBehaviour
{
    [SerializeField] private ObjectPool rewardObjectPool;
    [SerializeField] private ModifierTypeSO[] positiveModifierTypeSos;
    [SerializeField] private ModifierTypeSO[] negativeModifierTypeSos;

    private int _numPositiveModifiers;
    private int _numNegativeModifiers;
    private List<ModifierTypeSO> _modifierTypeSos;
    
    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 3;


    private void Awake()
    {
        _numPositiveModifiers = positiveModifierTypeSos.Length;
        _numNegativeModifiers = negativeModifierTypeSos.Length;
        
        rewardObjectPool.InitializePool();
    }

    //TODO store rewards, and reuse them
    public PooledObject Build()
    {
        if (DebugLogger.IsNullError(rewardObjectPool, this, "Must be set in editor.")) return null;
        
        var pooledObject = rewardObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return null;

        var modifierReward = pooledObject.GetComponent<ModifierReward>();        //TODO Make Generic
        if (DebugLogger.IsNullError(modifierReward, this)) return null;

        var rewardDataObjects = GetModifiers();
        modifierReward.SetModifierData(rewardDataObjects, _modifierTypeSos);

        return pooledObject;
    }

    private Dictionary<ModifierTypeSO, int> GetModifiers()
    {
        var modifiers = new Dictionary<ModifierTypeSO, int>();
        _modifierTypeSos = new List<ModifierTypeSO>();
        var rewardLevel = Random.Range(MIN_LEVEL, MAX_LEVEL + 1);
        
        // Generate Base Reward
        var baseModifier = GetPositiveModifierType();
        modifiers.Add(baseModifier, rewardLevel);
        _modifierTypeSos.Add(baseModifier);

        // Generate Negative Rewards
        var levelsRemaining = rewardLevel;
        while (levelsRemaining > MIN_LEVEL)
        {
            var negativeModifier = GetNegativeModifierRewardType();
            if (_modifierTypeSos.Contains(negativeModifier)) continue;
            
            var randomLevel = Random.Range(MIN_LEVEL, levelsRemaining);
            modifiers.Add(negativeModifier, -randomLevel);
            _modifierTypeSos.Add(negativeModifier);
            levelsRemaining -= randomLevel;
        }

        return modifiers;
    }

    private ModifierTypeSO GetPositiveModifierType()
    {
        if (DebugLogger.IsNullOrEmptyError(positiveModifierTypeSos, this, "Must be set in editor.")) return null;

        var randomIndex = Random.Range(0, _numPositiveModifiers);
        return positiveModifierTypeSos[randomIndex];
    }

    private ModifierTypeSO GetNegativeModifierRewardType()
    {
        if (DebugLogger.IsNullOrEmptyError(negativeModifierTypeSos, this, "Must be set in editor.")) return null;

        var randomIndex = Random.Range(0, _numNegativeModifiers);
        return negativeModifierTypeSos[randomIndex];
    }
}
