using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] [Range(0,MAX_ROLL)] private int lootDropProbability;
    [SerializeField] private Loot[] loots;
    
    private float _sum;
    
    private const int MAX_ROLL = 100;
    
    [SerializableAttribute] internal class Loot
    {
        public ObjectPool LootObjectPool;
        public int Probability;
    }
    
    
    private void Awake()
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient) return;
        
        foreach (var loot in loots)
        {
            _sum += loot.Probability;
            loot.LootObjectPool.InitializePool();
        }
    }
    
    [Button]
    public void RollDrop()
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient) return;

        var roll = Random.Range(0, MAX_ROLL);
        if (roll < lootDropProbability)
        {
            RollLoot();
        }
    }

    private void RollLoot()
    {
        var total = 0f;
        var roll = Random.Range(0, _sum);
        foreach (var loot in loots)
        {
            total += loot.Probability;
            if (roll < total)
            {
                var pooledLoot = loot.LootObjectPool.GetPooledObject();
                pooledLoot.Spawn(spawnTransform.rotation, spawnTransform.position, false);
                return;
            }
        }
    }
}
