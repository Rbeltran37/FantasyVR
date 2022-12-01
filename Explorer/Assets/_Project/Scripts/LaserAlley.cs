using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAlley : MonoBehaviour
{
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;
    [SerializeField] private float totalTimeMoving;
    [SerializeField] private float distanceToMove;

    private void Start()
    {
        if(!leftWall || !rightWall)
            DebugLogger.Error("Start", "Wall transform missing.", this);
    }

    public void EnableWallMovement()
    {
        StartCoroutine(MoveWall());
    }
    
    private IEnumerator MoveWall()
    {
        Vector3 updatedLeftWallPosition = new Vector3(leftWall.position.x + distanceToMove, leftWall.position.y, leftWall.position.z);
        Vector3 updatedRightWallPosition = new Vector3(rightWall.position.x - distanceToMove, rightWall.position.y, rightWall.position.z);
        
        float elapsedTime = 0;
        float waitTime = totalTimeMoving;
        Vector3 currentLeftWallPos = leftWall.position;
        Vector3 currentRightWallPos = rightWall.position;
        
        while (elapsedTime < waitTime)
        {
            leftWall.position = Vector3.Lerp(currentLeftWallPos, updatedLeftWallPosition, (elapsedTime / waitTime));
            rightWall.position = Vector3.Lerp(currentRightWallPos, updatedRightWallPosition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }  
        
        leftWall.position = updatedLeftWallPosition;
        rightWall.position = updatedRightWallPosition;
        yield return null;
    }
}
