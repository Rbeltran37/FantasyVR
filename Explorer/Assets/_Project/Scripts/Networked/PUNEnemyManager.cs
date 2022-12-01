using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class PUNEnemyManager : EnemyManager
{
    [SerializeField] private PUNPlayerTargetManager punPlayerTargetManager;

    private static bool IsOnlineAndNotMaster => !PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient;
    
    
    protected override void Start()
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient) return;
        
        Initialize();
        
        base.Start();
    }

    private void Initialize()
    {
        if (DebugLogger.IsNullInfo(punPlayerTargetManager, this, "Should be set in editor. Attempting to find."))
        {
            punPlayerTargetManager = FindObjectOfType<PUNPlayerTargetManager>();
            if (DebugLogger.IsNullError(punPlayerTargetManager, this, "Should be set in editor. Was not found in scene.")) return;
        }
    }

    public override PooledEnemy SpawnEnemy(ObjectPool enemyObjectPool, EnemySetupSO enemySetupSo,
        Transform spawnTransform)
    {
        if (IsOnlineAndNotMaster) return null;

        var pooledObjectEnemy = base.SpawnEnemy(enemyObjectPool, enemySetupSo, spawnTransform);
        if (pooledObjectEnemy)
        {
            pooledObjectEnemy.SetPUNPlayerTargetManager(punPlayerTargetManager);
        }

        return pooledObjectEnemy;
    }

    public override PooledEnemy SpawnRandomEnemy()
    {
        if (IsOnlineAndNotMaster) return null;

        var pooledObjectEnemy =  base.SpawnRandomEnemy();
        if (pooledObjectEnemy)
        {
            pooledObjectEnemy.SetPUNPlayerTargetManager(punPlayerTargetManager);
        }

        return pooledObjectEnemy;
    }
    
    [Button]
    public override void SimpleSpawn()
    {
        if (IsOnlineAndNotMaster) return;
        
        var randomEnemy = SpawnRandomEnemy();
        randomEnemy.AttemptActivate();
    }
    
    protected override void Remove(PooledObject pooledObject)
    {
        if (IsOnlineAndNotMaster) return;
        
        if (DebugLogger.IsNullError(pooledObject, this)) return;
        if (DebugLogger.IsNullOrEmptyError(EnemiesSpawned, this)) return;
        
        if (!EnemiesSpawned.Contains(pooledObject))
        {
            DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(EnemiesSpawned)} does not contain {nameof(pooledObject)}={pooledObject}.", this);
            return;
        }
        
        EnemiesSpawned.Remove(pooledObject);
        var punPhotonViewManager = pooledObject.GetComponent<PUNPhotonViewManager>();
        if (DebugLogger.IsNullError(punPhotonViewManager, this)) return;

        punPhotonViewManager.TakeOwnershipOfAllPhotonViews();
    }
    
    [Button]
    public void RespawnAllEnemies()
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, "", this);
        
        foreach (var pooledObject in EnemiesSpawned)
        {
            pooledObject.Spawn();
        }
    }
}
