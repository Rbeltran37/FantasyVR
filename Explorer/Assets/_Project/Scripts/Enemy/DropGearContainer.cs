using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class DropGearContainer : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private PuppetDeathHandler puppetDeathHandler;
    [SerializeField] private ObjectPool dropGearObjectPool;


    private void Awake()
    {
        dropGearObjectPool.InitializePool();
    }

    [Button]
    private void PopulateParameters()
    {
        if (!spawnTransform) spawnTransform = transform;
        if (!puppetDeathHandler)
        {
            var parent = transform.parent;
            while (parent && !puppetDeathHandler)
            {
                puppetDeathHandler = parent.GetComponent<PuppetDeathHandler>();
                if (!puppetDeathHandler) puppetDeathHandler = parent.GetComponentInChildren<PuppetDeathHandler>();
                parent = parent.parent;
            }
        }
    }

    public void SpawnDropGear()
    {
        if (DebugLogger.IsNullError(puppetDeathHandler, this, "Must be set in editor.")) return;

        var pooledObject = dropGearObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return;

        var dropGear = pooledObject.GetComponent<DropGear>();
        if (DebugLogger.IsNullError(dropGear, this)) return;

        var dissolve = dropGear.DissolveEffect;
        if (DebugLogger.IsNullError(dissolve, this)) return;
        
        puppetDeathHandler.WasTurnedOff += dissolve.Dissolve;
        pooledObject.Spawn(null, spawnTransform.position, spawnTransform.rotation, true);
    }
}
