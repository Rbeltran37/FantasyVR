using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArenaModule : ArenaObject
{
    [SerializeField] private Transform propTransform;
    [SerializeField] private float activatedHeight = 2;
    [SerializeField] private float deactivatedHeight;
    [SerializeField] [MinMaxFloatRange(.01f, .1f)] private RangedFloat moveSpeed;

    private bool _isMoving;
    private float _currentSpeed;

    private const float WAIT_INTERVAL = .01f;
    
    private void Awake()
    {
        PopulateParameters();
    }
    
    public override IEnumerator Activate(Action callback)
    {
        if (_isMoving) yield break;
        
        if (IsActivated())
        {
            DebugLogger.Warning("Activate", $"ArenaModule is already activated.", this);
            yield break;
        }

        _isMoving = true;
        _currentSpeed = Random.Range(moveSpeed.minValue, moveSpeed.maxValue);
        
        while (propTransform.position.y < activatedHeight)
        {
            propTransform.position += Vector3.up * _currentSpeed;

            if (propTransform.position.y > activatedHeight) break;

            yield return new WaitForSeconds(WAIT_INTERVAL);
        }

        SetToActivatedHeight();
        
        _isMoving = false;

        callback?.Invoke();
    }

    public override IEnumerator Deactivate(Action callback)
    {
        if (_isMoving) yield break;
        
        if (IsDeactivated())
        {
            DebugLogger.Warning("Deactivate", $"ArenaModule is already deactivated.", this);
            yield break;
        }
        
        _isMoving = true;
        _currentSpeed = Random.Range(moveSpeed.minValue, moveSpeed.maxValue);
        
        while (propTransform.position.y > deactivatedHeight)
        {
            
            propTransform.position += Vector3.down * _currentSpeed;
            
            if (propTransform.position.y < deactivatedHeight) break;
            
            yield return new WaitForSeconds(WAIT_INTERVAL);
        }
        
        SetToDeactivatedHeight();
        
        _isMoving = false;
        
        callback?.Invoke();
    }

    public override void SetToActivatedPosition()
    {
        SetToActivatedHeight();
    }

    public override void SetToDeactivatedPosition()
    {
        SetToDeactivatedHeight();
    }

    private void SetToActivatedHeight()
    {
        if (!propTransform) propTransform = transform;

        var obstaclePosition = propTransform.position;
        propTransform.position = new Vector3(obstaclePosition.x, activatedHeight, obstaclePosition.z);
    }
    
    private void SetToDeactivatedHeight()
    {
        if (!propTransform) propTransform = transform;
        
        var obstaclePosition = propTransform.position;
        propTransform.position = new Vector3(obstaclePosition.x, deactivatedHeight, obstaclePosition.z);
    }

    public override void PopulateParameters()
    {
        if (!propTransform) propTransform = transform;
    }

    [Button]
    private void SetDeactivatedHeightAsCurrentHeight()
    {
        deactivatedHeight = transform.position.y;
    }
    
    [Button]
    private void SetActivatedHeightAsCurrentHeight()
    {
        activatedHeight = transform.position.y;
    }

    private bool IsActivated()
    {
        return Math.Abs(propTransform.position.y - activatedHeight) < Mathf.Epsilon;
    }
    
    private bool IsDeactivated()
    {
        return Math.Abs(propTransform.position.y - deactivatedHeight) < Mathf.Epsilon;
    }
}
