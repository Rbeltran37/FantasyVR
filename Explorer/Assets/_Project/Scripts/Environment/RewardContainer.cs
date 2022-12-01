using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class RewardContainer : MonoBehaviour
{
    [SerializeField] private PhotonView thisPhotonView;
    [SerializeField] private PUNReadyUp punReadyUp;
    [SerializeField] private PUNPlayerTargetManager punPlayerTargetManager;
    
    private List<Reward> _spawnedRewards = new List<Reward>();

    public delegate List<PooledObject> RewardSpawnHandler();

    public RewardSpawnHandler SpawnCalled;
    
    private const int NUM_REWARDS_SPAWNED = 3;
    
    
    [Button]
    public void PopulateParameters()
    {
        if (!punPlayerTargetManager) punPlayerTargetManager = FindObjectOfType<PUNPlayerTargetManager>();

        if (!thisPhotonView)
        {
            thisPhotonView = GetComponent<PhotonView>();
            if (!thisPhotonView) thisPhotonView = gameObject.AddComponent<PhotonView>();
        }
    }

    [Button]
    public void SpawnRewards(Transform[] spawnParents)
    {
        if (_spawnedRewards.Count > 0) ClearRewards();
        
        if (DebugLogger.IsNullError(punPlayerTargetManager, this, "Must be set in editor.")) return;

        var spawnParentCount = spawnParents.Length;
        var numPlayers = punPlayerTargetManager.GetNumPlayers();
        if (spawnParentCount < numPlayers)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(spawnParentCount)}={spawnParentCount} < {nameof(numPlayers)}={numPlayers}");
            return;
        }

        var playerList = PhotonNetwork.PlayerList;
        foreach (var player in playerList)
        {
            var playerIndex = punPlayerTargetManager.GetPlayerIndex(player);
            var spawnParent = spawnParents[playerIndex];
            var spawnPositions = GetSpawnPositions(spawnParent);
            
            if (player.IsLocal) SpawnRewardsLocally(spawnPositions);
            else SendSpawnRewards(spawnPositions, player);
        }
    }

    private Vector3[] GetSpawnPositions(Transform spawnParent)
    {
        var spawnPositions = new Vector3[NUM_REWARDS_SPAWNED];

        if (DebugLogger.IsNullError(spawnParent, this)) return spawnPositions;
        
        var childCount = spawnParent.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var child = spawnParent.GetChild(i);
            spawnPositions[i] = child.position;
        }

        return spawnPositions;
    }

    private void SendSpawnRewards(Vector3[] positions, Player player)
    {
        thisPhotonView.RPC(nameof(RPCSpawnRewards), player, positions);
    }

    [PunRPC]
    private void RPCSpawnRewards(Vector3[] positions)
    {
        SpawnRewardsLocally(positions);
    }

    private void SpawnRewardsLocally(Vector3[] positions)
    {
        var pooledObjects = SpawnCalled?.Invoke();
        if (DebugLogger.IsNullOrEmptyError(pooledObjects, this)) return;

        var currentPositionIndex = 0;
        foreach (var pooledObject in pooledObjects)
        {
            if (DebugLogger.IsNullError(pooledObject, this)) continue;

            var reward = pooledObject.GetComponent<Reward>();
            if (DebugLogger.IsNullError(reward, this)) continue;

            _spawnedRewards.Add(reward);

            reward.RewardWasUsed += ClearRewards;

            pooledObject.Spawn(positions[currentPositionIndex++], Quaternion.identity, true);
        }
    }

    private void ClearRewards()
    {
        punReadyUp.AttemptIsReady(true);
        
        foreach (var spawnedReward in _spawnedRewards)
        {
            spawnedReward.Clear();
        }
        
        _spawnedRewards.Clear();
    }
}
