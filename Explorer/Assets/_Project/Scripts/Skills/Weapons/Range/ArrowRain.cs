using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArrowRain : WeaponAbility
{
    [SerializeField] private ArrowRainData arrowRainData;
    [SerializeField] private ObjectPool spawnedArrowsObjectPool;
    
    private Skill _skill;
    private BowInstance _bowInstance;
    private Arrow _activatedArrow;
    private Vector3 _arrowSpawnOriginPosition;
    private bool _isCreatingArrows;


    private void Awake()
    {
        if (DebugLogger.IsNullError(spawnedArrowsObjectPool, this)) return;

        spawnedArrowsObjectPool.InitializePool();
    }
    
    public override void Activate(WeaponAbilityData weaponAbilityData)
    {
        if (DebugLogger.IsNullError(weaponAbilityData, this)) return;

        var bowAbilityData = (BowAbilityData) weaponAbilityData;
        if (DebugLogger.IsNullError(weaponAbilityData, this)) return;

        _activatedArrow = bowAbilityData.Arrow;
        
        if (DebugLogger.IsNullError(_activatedArrow, this)) return;
        
        _skill = bowAbilityData.Skill;
        _bowInstance = bowAbilityData.BowInstance;

        _activatedArrow.HasStopped += CreateArrows;
        _activatedArrow.HasHit += DeactivateAbility;
    }

    public override void Deactivate(WeaponAbilityData weaponAbilityData)
    {
        if (weaponAbilityData == null) return;
        if (!_activatedArrow) return;

        _activatedArrow.HasStopped -= CreateArrows;
        _activatedArrow.HasHit -= DeactivateAbility;
    }

    private void DeactivateAbility()
    {
        if (!_bowInstance || _isCreatingArrows) return;
        
        _bowInstance.DeactivateAbility();
    }

    //Spawn's arrows 
    private void CreateArrows()
    {
        _isCreatingArrows = true;
        
        CoroutineCaller.Instance.StartCoroutine(CreateArrowsCoroutine());
    }

    private IEnumerator CreateArrowsCoroutine()
    {
        var hitPosition = _activatedArrow.transform.position;

        var numArrowsSpawned = 0;
        //Sets correct rotation on arrows to at target rotation.
        for (numArrowsSpawned = 0; numArrowsSpawned < _bowInstance.GetArrowCount(); numArrowsSpawned++)
        {
            if (!CanCast()) break;
            var pooledObject = spawnedArrowsObjectPool.GetPooledObject();

            Cast();
            
            //Spawn Arrow at the correct height and rotation
            SetArrowSpawnOriginPosition(hitPosition);
            var arrowPosition = GetArrowSpawnPosition();
            
            //Rotates arrow to targetRotation
            var arrowRotation = Quaternion.Slerp(pooledObject.transform.rotation, arrowRainData.targetRotation, 1);

            pooledObject.Spawn(null, arrowPosition, arrowRotation, true);        //lifetime is set to 0 to allow the Arrow script to handle lifetime
            
            //Gets Arrow script attached to this Arrow and fires it.
            var currentArrow = pooledObject.GetComponent<Arrow>();
            if (DebugLogger.IsNullError(currentArrow, this)) yield break;
            
            currentArrow.Fire(arrowRainData.arrowPullValue);

            var randomWaitTime = Random.Range(arrowRainData.arrowSpawnInterval.minValue,
                arrowRainData.arrowSpawnInterval.maxValue);
            yield return new WaitForSeconds(randomWaitTime);
        }

        _isCreatingArrows = false;
    }

    //Sets height of arrows spawning
    private void SetArrowSpawnOriginPosition(Vector3 currentPosition)
    {
        _arrowSpawnOriginPosition = new Vector3(
            currentPosition.x, currentPosition.y + arrowRainData.spawnHeightOffset, currentPosition.z);
    }

    private Vector3 GetArrowSpawnPosition()
    {
        //Sets a random min and max range offset for arrow distance. 
        var xShift = Random.Range(_arrowSpawnOriginPosition.x - arrowRainData.positionOffsetValue, 
            _arrowSpawnOriginPosition.x + arrowRainData.positionOffsetValue);
        var zShift = Random.Range(_arrowSpawnOriginPosition.z - arrowRainData.positionOffsetValue, 
            _arrowSpawnOriginPosition.z + arrowRainData.positionOffsetValue);

        //Returns new position for arrow.
        return new Vector3(xShift, _arrowSpawnOriginPosition.y, zShift);
    }

    private bool CanCast()
    {
        return _skill.CanCast();
    }

    private void Cast()
    {
        _skill.Cast();
    }
}
