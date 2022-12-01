using System;
using UnityEngine;
using System.Collections;
 
public class FloatingObject : MonoBehaviour 
{
    [SerializeField] private float degreesPerSecond = 15.0f;
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1f;
    [SerializeField] private Transform thisTransform;
 
    // Position Storage Variables
    private Vector3 _posOffset;
    private Vector3 _tempPos;
    private float _startTime;
    

    private void OnEnable()
    {
        _posOffset = thisTransform.position;
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Spin object around Y-Axis
        thisTransform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
 
        // Float up/down with a Sin()
        _tempPos = _posOffset;
        _tempPos.y += Mathf.Sin((Time.time - _startTime) * Mathf.PI * frequency) * amplitude;
        thisTransform.position = _tempPos;
    }
}

