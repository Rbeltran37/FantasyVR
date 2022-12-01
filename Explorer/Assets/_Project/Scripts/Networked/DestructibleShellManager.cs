using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class DestructibleShellManager : MonoBehaviour
{
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private Transform[] spawnTransforms;

    private List<PooledObject> _pooledObjects = new List<PooledObject>();

    private const float WAIT_INTERVAL = .1f;


    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        InitializePool();
    }

    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _pooledObjects = new List<PooledObject>();
        foreach (var spawnTransform in spawnTransforms)
        {
            var pooledObject = objectPool.GetPooledObject();
            if (DebugLogger.IsNullError(pooledObject, this)) return;
            
            pooledObject.Spawn(spawnTransform.rotation, spawnTransform.position, true);
            _pooledObjects.Add(pooledObject);
        }
    }

    private void OnDisable()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var pooledObjectArray = _pooledObjects.ToArray();
        for (var i = 0; i < pooledObjectArray.Length; i++)
        {
            pooledObjectArray[i].Despawn();
        }
        _pooledObjects.Clear();
    }

    [Button]
    public void PopulateParameters()
    {
        var childCount = transform.childCount;
        spawnTransforms = new Transform[childCount];
        for (var i = 0; i < childCount; i++)
        {
            spawnTransforms[i] = transform.GetChild(i);
        }
    }

    private void InitializePool()
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsConnectedAndReady)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            
            CoroutineCaller.Instance.StartCoroutine(InitializePoolCoroutine());
        }
        else
        {
            objectPool.InitializePool();
        }
    }
    
    private IEnumerator InitializePoolCoroutine()
    {
        yield return WaitToConnect();
        
        objectPool.InitializePool();
    }

    private IEnumerator WaitToConnect()
    {
        if (PhotonNetwork.OfflineMode) yield break;
        
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return new WaitForSeconds(WAIT_INTERVAL);
        }
    }
}
