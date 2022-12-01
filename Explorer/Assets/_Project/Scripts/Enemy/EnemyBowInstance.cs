using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBowInstance : EnemyWeaponInstance
{
    [SerializeField] private EnemyWeaponInstance arrowEnemyWeaponInstance;
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private ObjectPool arrowObjectPool;


    private void Awake()
    {
        arrowObjectPool.InitializePool();
    }
    
    public void FireArrow()
    {
        var pooledObject = arrowObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return;

        var arrow = (Arrow) pooledObject;
        if (DebugLogger.IsNullError(arrow, this)) return;
        
        arrow.Spawn(null, spawnTransform.position, spawnTransform.rotation, false);
        arrow.Fire(1);      //Launches at max value
    }

    public void AddKnockArrowToDropHandler(DropGearHandler dropGearHandler)
    {
        if (DebugLogger.IsNullDebug(arrowEnemyWeaponInstance, this, "Must be set in editor.")) return;
        
        dropGearHandler.Add(arrowEnemyWeaponInstance);
    }
}
