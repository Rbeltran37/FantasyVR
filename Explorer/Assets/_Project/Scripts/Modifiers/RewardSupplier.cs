using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class RewardSupplier : MonoBehaviour
{
    [SerializeField] private RewardFactory rewardFactory;
    [SerializeField] private List<RewardContainer> rewardContainers = new List<RewardContainer>();

    
    private void Awake()
    {
        foreach (var rewardContainer in rewardContainers)
        {
            rewardContainer.SpawnCalled += SpawnRewards;
        }
    }

    [Button]
    public void PopulateParameters(Transform levelParent)
    {
        if (!levelParent) levelParent = transform;
        
        rewardContainers = new List<RewardContainer>();
        var containers = levelParent.GetComponentsInChildren<RewardContainer>();
        foreach (var container in containers)
        {
            rewardContainers.Add(container);
        }
    }

    //Unique Rewards, no duplicates
    private List<PooledObject> SpawnRewards()
    {
        if (!rewardFactory)
        {
            DebugLogger.Error(nameof(SpawnRewards), $"{nameof(rewardFactory)} is null. Must be set in editor.", this);
            return null;
        }
        
        var rewardList = new List<PooledObject>();
        for (var rewardsBuilt = 0; rewardsBuilt < 3; rewardsBuilt++)
        {
            rewardList.Add(rewardFactory.Build());
        }
        
        return rewardList;
    }
}
