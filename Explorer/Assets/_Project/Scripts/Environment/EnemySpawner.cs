using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private EnemyType[] enemyTypes;
    [SerializeField] private SpawnType spawnType = SpawnType.Count;
    [SerializeField] private int numberOfEnemiesToMaintain = 1;
    [SerializeField] private int totalEnemiesToSpawn;
    [SerializeField] private float respawnTime = 1f;
    [SerializeField] private Vector3 sizeOfBoundaries;
    
    private bool _isActive;
    private int _totalEnemiesSpawned;
    private int _totalEnemiesKilled;
    private int _numActive;
    private Vector3 _lowerBounds;
    private Vector3 _upperBounds;
    private PooledEnemy[] _pooledEnemies;

    public Action EnemyHasBeenKilled;
    public Action AllEnemiesHaveBeenKilled;
    
    private Action _enemyHasBeenSpawned;

    public enum SpawnType
    {
        Count,
        Endless,
    }
    
    [SerializableAttribute] public class EnemyType
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
        if (!spawnTransform) spawnTransform = transform;
        
        Gizmos.DrawWireCube(spawnTransform.position, sizeOfBoundaries);
    }

    private void Awake()
    {
        if (!spawnTransform) spawnTransform = transform;
        if (DebugLogger.IsNullWarning(enemyManager, this, "Should be set in editor. Attempting to find..."))
        {
            enemyManager = FindObjectOfType<EnemyManager>();
            if (DebugLogger.IsNullError(enemyManager, this, "Should be set in editor. Unable to find.")) return;
        }
        
        _enemyHasBeenSpawned += () => _numActive++;
        _enemyHasBeenSpawned += () => _totalEnemiesSpawned++;
        EnemyHasBeenKilled += () => _numActive--;
        EnemyHasBeenKilled += () => _totalEnemiesKilled++;

        _pooledEnemies = new PooledEnemy[totalEnemiesToSpawn];
        
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

    public void StartSpawning()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        _isActive = true;
        
        for (var i = 0; i < numberOfEnemiesToMaintain; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        CoroutineCaller.Instance.StartCoroutine(SpawnEnemyCoroutine());
    }

    private IEnumerator SpawnEnemyCoroutine()
    {
        if (DebugLogger.IsNullError(enemyManager, this, "Must be set in editor.")) yield break; 
        
        if (!CanSpawnEnemy()) yield break;

        if (_totalEnemiesSpawned > 0) yield return new WaitForSeconds(respawnTime);

        var spawnPosition = GetRandomSpawnPosition();
        var spawnRotation = GetSpawnRotation();

        var randomEnemyType = GetRandomEnemyType();
        var currentEnemyObjectPool = randomEnemyType.enemyObjectPool;
        var currentEnemySetup = randomEnemyType.GetRandomEnemySetupSo();
        var pooledEnemy = enemyManager.SpawnEnemy(currentEnemyObjectPool, currentEnemySetup, spawnPosition, spawnRotation);
        if (DebugLogger.IsNullError(pooledEnemy, this)) yield break;
        
        pooledEnemy.SubscribeToWasKilled(RegisterEnemyDeath);
        _pooledEnemies[_totalEnemiesSpawned] = pooledEnemy;

        _enemyHasBeenSpawned?.Invoke();
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var clampedX = Random.Range(_lowerBounds.x, _upperBounds.x);
        var clampedY = Random.Range(_lowerBounds.y, _upperBounds.y);
        var clampedZ = Random.Range(_lowerBounds.z, _upperBounds.z);
        
        return new Vector3(clampedX, clampedY, clampedZ);
    }

    private Quaternion GetSpawnRotation()
    {
        return spawnTransform.rotation;
    }

    private EnemyType GetRandomEnemyType()
    {
        var randomIndex = Random.Range(0, enemyTypes.Length);
        return enemyTypes[randomIndex];
    }

    private bool CanSpawnEnemy()
    {
        if (!_isActive) return false;
        
        if (spawnType == SpawnType.Count &&
            _totalEnemiesSpawned >= totalEnemiesToSpawn) return false;
        
        return _numActive < numberOfEnemiesToMaintain;
    }

    private void RegisterEnemyDeath()
    {
        EnemyHasBeenKilled?.Invoke();

        if (_totalEnemiesKilled == totalEnemiesToSpawn)
        {
            AllEnemiesHaveBeenKilled?.Invoke();
            return;
        }
        
        SpawnEnemy();
    }

    public List<PooledEnemy> GetEnemies()
    {
        return _pooledEnemies.ToList();
    }
}
