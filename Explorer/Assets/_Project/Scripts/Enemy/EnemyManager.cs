using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using UnityEngine;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] protected ObjectPool enemyPrefabObjectPool;
    [SerializeField] protected EnemySetupSO[] enemySetupSos;
    
    [SerializeField] private Transform[] spawnPoints;
    
    protected List<PooledObject> EnemiesSpawned = new List<PooledObject>();
    
    private int _numSpawnPoints;


    protected virtual void Start() 
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient) return;
        
        _numSpawnPoints = spawnPoints.Length;
        
        enemyPrefabObjectPool.InitializePool();
    }
    
    public virtual PooledEnemy SpawnEnemy(ObjectPool enemyObjectPool, EnemySetupSO enemySetupSo, Transform spawnTransform)
    {
        return SpawnEnemy(enemyObjectPool, enemySetupSo, spawnTransform.position, spawnTransform.rotation);
    }
    
    public virtual PooledEnemy SpawnEnemy(ObjectPool enemyObjectPool, EnemySetupSO enemySetupSo, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (DebugLogger.IsNullError(enemyObjectPool, this)) return null;
        if (DebugLogger.IsNullError(enemySetupSo, this)) return null;

        var pooledObject = enemyObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return null;

        var pooledObjectEnemy = pooledObject as PooledEnemy;
        if (DebugLogger.IsNullError(pooledObjectEnemy, this)) return null;
        if (pooledObjectEnemy == null) return null;
        
        pooledObjectEnemy.EnemySetup(enemySetupSo);
        pooledObjectEnemy.Spawn(spawnRotation, spawnPosition, true);
        EnemiesSpawned.Add(pooledObjectEnemy);

        Subscribe(pooledObject);        //TODO May be obsolete
        return pooledObjectEnemy;
    }

    private void Subscribe(PooledObject pooledObject)
    {
        var isSubscribed = false;
        var actionList = pooledObject.WasDespawned;
        if (actionList != null)
        {
            var invocationList = pooledObject.WasDespawned.GetInvocationList();
            foreach (var invocation in invocationList)
            {
                if (invocation.GetMethodInfo().Name.Equals(nameof(Remove)))
                {
                    isSubscribed = true;
                }
            }
        }

        if (!isSubscribed) pooledObject.WasDespawned += Remove;
    }

    public virtual PooledEnemy SpawnRandomEnemy()
    {
        var randomIndex = Random.Range(0, enemySetupSos.Length);
        var randomEnemySetupSo = enemySetupSos[randomIndex];
        if (DebugLogger.IsNullError(randomEnemySetupSo, this)) return null;
        
        var randomSpawnTransform = GetRandomSpawnTransform();

        return SpawnEnemy(enemyPrefabObjectPool, randomEnemySetupSo, randomSpawnTransform);
    }

    [Button]
    public virtual void SimpleSpawn()
    {
        SpawnRandomEnemy();
    }
    
    [Button]
    public virtual void ClearEnemies()
    {
        var pooledEnemyList = EnemiesSpawned.ToList();
        foreach (var pooledObjectEnemy in pooledEnemyList) 
        {
            if (DebugLogger.IsNullWarning(pooledObjectEnemy, this)) return;

            pooledObjectEnemy.Despawn();
        }
        
        EnemiesSpawned.Clear();
    }

    private Transform GetRandomSpawnTransform()
    {
        var randomIndex = Random.Range(0, _numSpawnPoints);
        var randomSpawnPoint = spawnPoints[randomIndex];
        
        return randomSpawnPoint;
    }

    protected virtual void Remove(PooledObject pooledObject)
    {
        if (DebugLogger.IsNullError(pooledObject, this)) return;
        if (DebugLogger.IsNullOrEmptyError(EnemiesSpawned, this)) return;

        if (!EnemiesSpawned.Contains(pooledObject))
        {
            DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(EnemiesSpawned)} does not contain {nameof(pooledObject)}={pooledObject}.", this);
            return;
        }
        
        EnemiesSpawned.Remove(pooledObject);
    }
}
