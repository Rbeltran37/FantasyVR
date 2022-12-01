using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using UnityEngine;
using Random = UnityEngine.Random;

public class WallPunch : MonoBehaviour
{
    public Vector3 originPosition;
    public Vector3 positionToMoveTo;
    public float travelDistance;
    public float timeToPunch;
    public bool isPunching = false;
    public float minTime;
    public float maxTime;
    public float timeToWait;
    

    void Start()
    {
        originPosition = transform.position;
        positionToMoveTo = new Vector3(transform.position.x + travelDistance, transform.position.y, transform.position.z);
        StartCoroutine(Punch(positionToMoveTo, timeToPunch));
    }

    IEnumerator Punch(Vector3 targetPosition, float duration)
    {
        while (isPunching)
        {
            timeToWait = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(timeToWait);
            
            float time = 0;
            Vector3 startPosition = transform.position;

            while (time < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            
            StartCoroutine(Retract(originPosition, timeToPunch));
        }
    }
    
    IEnumerator Retract(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
    
}
