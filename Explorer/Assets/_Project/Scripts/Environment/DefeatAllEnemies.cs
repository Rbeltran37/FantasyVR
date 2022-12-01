using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class DefeatAllEnemies : ArenaEvent
{
    [SerializeField] private List<EnemySpawner> enemySpawners;

    private int _numEnemySpawnersActive;

    
    protected override void Awake()
    {
        _numEnemySpawnersActive = enemySpawners.Capacity;

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var enemySpawner in enemySpawners)
            {
                enemySpawner.AllEnemiesHaveBeenKilled += RegisterEnemySpawnerCleared;
            }
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }


    private void RegisterEnemySpawnerCleared()
    {
        _numEnemySpawnersActive--;
        
        if (_numEnemySpawnersActive == 0)
        {
            End();
        }
    }

    [Button]
    private void PopulateEnemySpawners(GameObject spawnerParent)
    {
        if (!spawnerParent) spawnerParent = gameObject;
        
        enemySpawners = new List<EnemySpawner>();
        var tempSpawners = spawnerParent.GetComponentsInChildren<EnemySpawner>();
        foreach (var spawner in tempSpawners)
        {
            enemySpawners.Add(spawner);
        }
    }
}
