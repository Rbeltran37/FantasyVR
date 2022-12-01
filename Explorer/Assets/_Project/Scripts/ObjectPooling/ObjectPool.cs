using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "_ObjectPool", menuName = "ScriptableObjects/Core/ObjectPool", order = 1)]
public class ObjectPool : ScriptableObject
{
    [SerializeField] protected GameObject objectPrefab;
    [SerializeField] protected int poolSize = 10;

    protected PooledObject[] ObjectPoolArray;
    protected int NumObjectsInstantiated;

    private int _currentObject;
    
    public bool IsInitialized => ObjectPoolArray != null && ObjectPoolArray.Length > EMPTY && ObjectPoolArray[0] != null;

    public Action PoolWasFilled;

    protected const int EMPTY = 0;
    

    public virtual void InitializePool()
    {
        if (IsInitialized) return;

        _currentObject = EMPTY;
        NumObjectsInstantiated = EMPTY;
        ObjectPoolArray = new PooledObject[poolSize];
        FillPool();
    }

    private void FillPool()
    {
        for (var index = 0; index < poolSize; index++)
        {
            InstantiateObject();
        }
        
        PoolWasFilled?.Invoke();
    }
    
    protected virtual void InstantiateObject()
    {
        if (DebugLogger.IsNullError(objectPrefab, this, "Must be set in editor.")) return;

        var pooledGameObject = Instantiate(objectPrefab);
        var pooledObject = pooledGameObject.GetComponent<PooledObject>();
        if (DebugLogger.IsNullError(pooledObject, this)) return;

        ObjectPoolArray[NumObjectsInstantiated++] = pooledObject;
    }
    
    public PooledObject GetPooledObject()
    {
        if (ObjectPoolArray == null) InitializePool();
        if (ObjectPoolArray == null) return null;
        if (ObjectPoolArray.Length <= EMPTY) InstantiateObject();
        if (_currentObject >= poolSize) _currentObject = 0;
        
        if (DebugLogger.IsNullOrEmptyError(ObjectPoolArray, this)) return null;

        var pooledObject = ObjectPoolArray[_currentObject++];
        if (DebugLogger.IsNullWarning(pooledObject, this, $"Calling {MethodBase.GetCurrentMethod().Name} recursively.")) 
            return GetPooledObject();
        
        return pooledObject;
    }
}
