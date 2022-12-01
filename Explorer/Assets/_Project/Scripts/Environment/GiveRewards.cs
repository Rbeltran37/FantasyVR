using System;
using System.Collections;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class GiveRewards : PUNArenaEvent
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private RewardContainer rewardContainer;
    [SerializeField] private ObjectPool obeliskObjectPool;


    protected override void Awake()
    {
        base.Awake();
        
        if (DebugLogger.IsNullError(obeliskObjectPool, "Must be set in editor.", this)) return;
        
        if (PhotonNetwork.IsMasterClient) obeliskObjectPool.InitializePool();
    }

    [Button]
    public override void PopulateParameters()
    {
        base.PopulateParameters();
        
        if (!rewardContainer)
        {
            rewardContainer = gameObject.GetComponent<RewardContainer>();
            if (!rewardContainer)
            {
                rewardContainer = gameObject.AddComponent<RewardContainer>();
            }
        }

        if (!spawnTransform) spawnTransform = transform;
    }

    public override void Begin()
    {
        base.Begin();
        
        if (!PhotonNetwork.IsMasterClient) return;
        
        var pooledObject = obeliskObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return;
        pooledObject.Spawn(spawnTransform.rotation, spawnTransform.position, true);

        var obelisk = pooledObject.GetComponent<Obelisk>();
        if (DebugLogger.IsNullError(obelisk, this)) return;

        var spawnParents = obelisk.GetSpawnParents();
        
        StartCoroutine(SpawnRewardsCoroutine(spawnParents));
        
        HasEnded += obelisk.Deactivate;
    }

    private IEnumerator SpawnRewardsCoroutine(Transform[] spawnParents)
    {
        if (DebugLogger.IsNullError(rewardContainer, this, "Must be set in editor.")) yield break;

        yield return new WaitForSeconds(WaitTime);
        
        rewardContainer.SpawnRewards(spawnParents);
    }
}
