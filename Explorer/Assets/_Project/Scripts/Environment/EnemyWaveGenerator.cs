using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyWaveGenerator : MonoBehaviour
{
    [SerializeField] private GameObject thisGameObject;
    [SerializeField] private Transform thisTransform;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private ObjectPool portalObjectPool;
    [SerializeField] private ObjectPool enemySpawnerObjectPool;
    [SerializeField] private EnemySpawner.EnemyType[] enemyTypes;
    [SerializeField] private SpawnBounds[] spawnBounds;
    [SerializeField] private int numberOfEnemiesToMaintain = 1;
    [SerializeField] private float respawnTime = 3f;
    
    private bool _isActive;
    private bool _hasSpawnedEnemy;
    private int _numActive;
    private Coroutine _coroutine;
    
    private const string SPAWN_TRANSFORM = "spawntransform";
    
    public Action AllEnemiesHaveBeenKilled;

    [Serializable] internal class SpawnBounds
    {
        public Transform spawnTransform;
        public Vector3 sizeOfBoundaries;
        
        private Vector3 _lowerBounds;
        private Vector3 _upperBounds;
        
        public Quaternion Rotation => spawnTransform.rotation;

        public SpawnBounds()
        {
            spawnTransform = null;
            sizeOfBoundaries = Vector3.one;
        }
        
        public void SetBounds()
        {
            var transformPosition = spawnTransform.position;
            var adjustedLowerX = transformPosition.x - sizeOfBoundaries.x / 2;
            var adjustedLowerY = transformPosition.y - sizeOfBoundaries.y / 2;
            var adjustedLowerZ = transformPosition.z - sizeOfBoundaries.z / 2;
            _lowerBounds = new Vector3(adjustedLowerX, adjustedLowerY, adjustedLowerZ);

            var adjustedUpperX = transformPosition.x + sizeOfBoundaries.x / 2;
            var adjustedUpperY = transformPosition.y + sizeOfBoundaries.y / 2;
            var adjustedUpperZ = transformPosition.z + sizeOfBoundaries.z / 2;
            _upperBounds = new Vector3(adjustedUpperX, adjustedUpperY, adjustedUpperZ);
        }
        
        public Vector3 GetRandomSpawnPosition()
        {
            var x = Random.Range(_lowerBounds.x, _upperBounds.x);
            var y = Random.Range(_lowerBounds.y, _upperBounds.y);
            var z = Random.Range(_lowerBounds.z, _upperBounds.z);
        
            return new Vector3(x, y, z);
        }
    }
    
    [Serializable] internal class EnemyType
    {
        public ObjectPool enemyObjectPool;
        public EnemySetupSO[] enemySetupSos;

        public EnemySetupSO GetRandomEnemySetupSo()
        {
            var randomIndex = Random.Range(0, enemySetupSos.Length);
            return enemySetupSos[randomIndex];
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(thisTransform.position, Vector3.one);
        
        Gizmos.color = Color.cyan;
        foreach (var spawnBound in spawnBounds)
        {
            Gizmos.DrawWireCube(spawnBound.spawnTransform.position, spawnBound.sizeOfBoundaries);
        }
    }

    private void Awake()
    {
        if (!thisTransform) thisTransform = transform;
        
        if (DebugLogger.IsNullWarning(enemyManager, this, "Should be set in editor. Attempting to find..."))
        {
            enemyManager = FindObjectOfType<EnemyManager>();
            if (DebugLogger.IsNullError(enemyManager, this, "Should be set in editor. Unable to find.")) return;
        }
        
        if (DebugLogger.IsNullError(enemySpawnerObjectPool, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(portalObjectPool, "Must be set in editor.", this)) return;

        enemySpawnerObjectPool.InitializePool();
        portalObjectPool.InitializePool();

        foreach (var enemyType in enemyTypes)
        {
            if (DebugLogger.IsNullError(enemyType, this)) return;
            if (DebugLogger.IsNullError(enemyType.enemyObjectPool, this)) return;
            
            enemyType.enemyObjectPool.InitializePool();
        }
    }

    [Button]
    public void PopulateParameters()
    {
        if (!thisGameObject) thisGameObject = gameObject;
        if (!thisTransform) thisTransform = transform;
        
        if (!enemyManager) enemyManager = FindObjectOfType<EnemyManager>();
        
        var tempSpawnTransforms = new List<Transform>();
        var childCount = transform.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name.ToLower().Contains(SPAWN_TRANSFORM))
            {
                tempSpawnTransforms.Add(child);
            }
        }

        spawnBounds = new SpawnBounds[tempSpawnTransforms.Count];
        for (var i = 0; i < spawnBounds.Length; i++)
        {
            spawnBounds[i] = new SpawnBounds();
            spawnBounds[i].spawnTransform = tempSpawnTransforms[i];
        }
    }

    public void Activate()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        SetBounds();

        var pooledObject = enemySpawnerObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledObject, this)) return;

        var enemySpawnerStructure = pooledObject as EnemySpawnerStructure;
        if (DebugLogger.IsNullError(enemySpawnerStructure, this)) return;
        
        enemySpawnerStructure.SetEnemyWaveGenerator(this);
        enemySpawnerStructure.Spawn(thisTransform.rotation, thisTransform.position, true);
    }
    
    public void Deactivate()
    {
        _isActive = false;
        
        if (_coroutine != null) CoroutineCaller.Instance.StopCoroutine(_coroutine);
        
        if (_numActive == 0)
        {
            AllEnemiesHaveBeenKilled?.Invoke();
            AllEnemiesHaveBeenKilled = null;
            return;
        }
    }

    private void SetBounds()
    {
        foreach (var spawnBound in spawnBounds)
        {
            spawnBound.SetBounds();
        }
    }

    public void StartSpawning()
    {
        if (_isActive) return;
        
        _isActive = true;

        if (!PhotonNetwork.IsMasterClient) return;

        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (!CanSpawnEnemy()) return;
        
        _coroutine = CoroutineCaller.Instance.StartCoroutine(SpawnEnemyCoroutine());
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        if (DebugLogger.IsNullError(enemyManager, this, "Must be set in editor.")) yield break; 
        
        if (!CanSpawnEnemy()) yield break;
        if (_hasSpawnedEnemy) yield return new WaitForSeconds(respawnTime);
        if (!CanSpawnEnemy()) yield break;
        
        var randomEnemyType = GetRandomEnemyType();
        if (DebugLogger.IsNullError(randomEnemyType, this)) yield break;
        var currentEnemyObjectPool = randomEnemyType.enemyObjectPool;
        var currentEnemySetup = randomEnemyType.GetRandomEnemySetupSo();
        
        var randomIndex = Random.Range(0, spawnBounds.Length);
        var randomBounds = spawnBounds[randomIndex];
        var spawnPosition = randomBounds.GetRandomSpawnPosition();
        var spawnRotation = randomBounds.Rotation;
        
        var pooledEnemy = enemyManager.SpawnEnemy(currentEnemyObjectPool, currentEnemySetup, spawnPosition, spawnRotation);
        if (DebugLogger.IsNullError(pooledEnemy, this)) yield break;
        
        pooledEnemy.SubscribeToWasKilled(RegisterEnemyDeath);
        
        var pooledPortal = portalObjectPool.GetPooledObject();
        if (DebugLogger.IsNullError(pooledPortal, this)) yield break;
        
        pooledPortal.Spawn(spawnRotation, spawnPosition, true);

        _hasSpawnedEnemy = true;
        _numActive++;
        
        if (CanSpawnEnemy()) yield return CoroutineCaller.Instance.StartCoroutine(SpawnEnemyCoroutine());
    }

    private EnemySpawner.EnemyType GetRandomEnemyType()
    {
        var randomIndex = Random.Range(0, enemyTypes.Length);
        return enemyTypes[randomIndex];
    }

    private bool CanSpawnEnemy()
    {
        if (!_isActive) return false;

        return _numActive < numberOfEnemiesToMaintain;
    }

    private void RegisterEnemyDeath()
    {
        _numActive--;

        if (!_isActive && _numActive == 0)
        {
            AllEnemiesHaveBeenKilled?.Invoke();
            AllEnemiesHaveBeenKilled = null;
            return;
        }
        
        SpawnEnemy();
    }
}
