using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DebugObjectPools : MonoBehaviour
{
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private float spawnBuffer;

    private bool _isSpawning;
    private float _spawnTimer;

    
    private void FixedUpdate()
    {
        if (!_isSpawning) return;

        _spawnTimer -= Time.fixedDeltaTime;
        if (_spawnTimer < 0)
        {
            Spawn();
            _spawnTimer = spawnBuffer;
        }
    }

    [Button]
    public void InitializePool()
    {
        objectPool.InitializePool();
    }

    [Button]
    public void Spawn()
    {
        var pooledObject = objectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return;
        
        pooledObject.Spawn(spawnTransform.position, spawnTransform.rotation, true);
    }

    [Button]
    public void ToggleConstantSpawn()
    {
        _isSpawning = true;
    }
}
