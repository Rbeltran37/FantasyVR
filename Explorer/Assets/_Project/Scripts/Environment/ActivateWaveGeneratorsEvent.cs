using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class ActivateWaveGeneratorsEvent : ArenaEvent
{
    [SerializeField] private List<EnemyWaveGenerator> enemyWaveGenerators;

    private int _numEnemyWaveGeneratorsActive;


    protected override void Awake()
    {
        base.Awake();
        
        _numEnemyWaveGeneratorsActive = enemyWaveGenerators.Count;
    }

    [Button]
    public void PopulateParameters()
    {
        var enemyWaveGeneratorsArray = GetComponentsInChildren<EnemyWaveGenerator>();
        if (enemyWaveGeneratorsArray == null) return;
        
        enemyWaveGenerators = enemyWaveGeneratorsArray.ToList();
    }
    
    public override void Begin()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var enemyWaveGenerator in enemyWaveGenerators)
            {
                enemyWaveGenerator.AllEnemiesHaveBeenKilled += RegisterEnemyWaveGeneratorsCleared;
                enemyWaveGenerator.Activate();
            }
        }
        
        Initialize();
    }
    
    private void RegisterEnemyWaveGeneratorsCleared()
    {
        _numEnemyWaveGeneratorsActive--;
        
        if (_numEnemyWaveGeneratorsActive == 0)
        {
            End();
        }
    }
}
