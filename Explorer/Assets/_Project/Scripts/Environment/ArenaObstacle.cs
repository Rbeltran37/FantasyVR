using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ArenaObstacle : MonoBehaviour
{
    [SerializeField] private Transform obstacleTransform;
    [SerializeField] private float raisedHeight = 2;
    [SerializeField] private float loweredHeight;
    [SerializeField] [MinMaxFloatRange(.01f, .1f)] private RangedFloat moveSpeed;

    private bool _isMoving;
    private float _currentSpeed;
    
    
    private void Awake()
    {
        if (!obstacleTransform)
        {
            enabled = false;
            return;
        }
        
        //_loweredHeight = obstacleTransform.transform.position.y;
    }
    
    public IEnumerator RaiseObstacleCoroutine(Action callback)
    {
        if (_isMoving) yield break;

        _isMoving = true;
        _currentSpeed = Random.Range(moveSpeed.minValue, moveSpeed.maxValue);
        
        while (obstacleTransform.position.y < raisedHeight)
        {
            obstacleTransform.position += Vector3.up * _currentSpeed;

            if (obstacleTransform.position.y > raisedHeight) break;

            yield return new WaitForEndOfFrame();
        }

        SetToRaisedHeight();
        
        _isMoving = false;

        callback?.Invoke();
    }

    public IEnumerator LowerObstacleCoroutine(Action callback)
    {
        if (_isMoving) yield break;

        _isMoving = true;
        _currentSpeed = Random.Range(moveSpeed.minValue, moveSpeed.maxValue);
        
        while (obstacleTransform.position.y > loweredHeight)
        {
            
            obstacleTransform.position += Vector3.down * _currentSpeed;
            
            if (obstacleTransform.position.y < loweredHeight) break;
            
            yield return new WaitForEndOfFrame();
        }
        
        SetToLoweredHeight();
        
        _isMoving = false;
        
        callback?.Invoke();
    }
    
    public void SetToRaisedHeight()
    {
        if (!obstacleTransform) obstacleTransform = transform;

        var obstaclePosition = obstacleTransform.position;
        obstacleTransform.position = new Vector3(obstaclePosition.x, raisedHeight, obstaclePosition.z);
    }
    
    public void SetToLoweredHeight()
    {
        if (!obstacleTransform) obstacleTransform = transform;
        
        var obstaclePosition = obstacleTransform.position;
        obstacleTransform.position = new Vector3(obstaclePosition.x, loweredHeight, obstaclePosition.z);
    }

    public void PopulateParameters()
    {
        if (!obstacleTransform) obstacleTransform = transform;
    }

    [Button]
    private void SetLoweredHeightAsCurrentHeight()
    {
        loweredHeight = transform.position.y;
    }
    
    [Button]
    private void SetRaisedHeightAsCurrentHeight()
    {
        raisedHeight = transform.position.y;
    }
}
