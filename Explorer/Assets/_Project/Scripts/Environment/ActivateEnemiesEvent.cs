using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ActivateEnemiesEvent : ArenaEvent
{
    [SerializeField] private List<EnemySpawner> enemySpawners;
    
    
    public override void Begin()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var enemySpawner in enemySpawners)
            {
                enemySpawner.StartSpawning();
            }
        }
        
        base.Begin();
    }
}
