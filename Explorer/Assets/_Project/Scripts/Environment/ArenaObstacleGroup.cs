using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ArenaObstacleGroup : MonoBehaviour
{
    [SerializeField] private List<ArenaObstacle> obstacles = new List<ArenaObstacle>();

    private int _numObstaclesMoving;
    private bool _isBeingLowered;
    private bool _isBeingRaised;
    
    public delegate void MovementHandler();

    public event MovementHandler HasBeenLowered;
    
    public event MovementHandler HasBeenRaised;

    
    [Button]
    public void RaiseObstacles()
    {
        _isBeingRaised = true;
        
        foreach (var obstacle in obstacles)
        {
            _numObstaclesMoving++;
            StartCoroutine(obstacle.RaiseObstacleCoroutine(RegisterFinishedMoving));
        }
        
        CheckIfGroupFinishedMoving();
    }

    [Button]
    public void LowerObstacles()
    {
        _isBeingLowered = true;
        
        foreach (var obstacle in obstacles)
        {
            _numObstaclesMoving++;
            StartCoroutine(obstacle.LowerObstacleCoroutine(RegisterFinishedMoving));
        }
        
        CheckIfGroupFinishedMoving();
    }
    
    [Button]
    public void SetObstaclesToRaisedHeight()
    {
        foreach (var obstacle in obstacles)
        {
            obstacle.SetToRaisedHeight();
        }
    }
    
    [Button]
    public void SetObstaclesToLoweredHeight()
    {
        foreach (var obstacle in obstacles)
        {
            obstacle.SetToLoweredHeight();
        }
    }
    
    [Button]
    public void PopulateParameters(GameObject obstacleParent)
    {
        if (!obstacleParent) obstacleParent = gameObject;
        
        obstacles = new List<ArenaObstacle>();
        var tempObstacles = obstacleParent.GetComponentsInChildren<ArenaObstacle>();
        foreach (var obstacle in tempObstacles)
        {
            obstacle.PopulateParameters();
            obstacles.Add(obstacle);
        }
    }

    private void RegisterFinishedMoving()
    {
        _numObstaclesMoving--;
        
        CheckIfGroupFinishedMoving();
    }

    private void CheckIfGroupFinishedMoving()
    {
        if (_numObstaclesMoving > 0) return;
        
        if (_isBeingLowered)
        {
            HasBeenLowered?.Invoke();
            _isBeingLowered = false;
        }
        else if (_isBeingRaised)
        {
            HasBeenRaised?.Invoke();
            _isBeingRaised = false;
        }

        _numObstaclesMoving = 0;
    }
}
