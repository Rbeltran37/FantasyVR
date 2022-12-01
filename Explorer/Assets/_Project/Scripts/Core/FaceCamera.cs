using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;
    [SerializeField] private FollowAxis followAxis = FollowAxis.All;

    private Transform _mainCameraTransform;
    private Dictionary<FollowAxis, Action> _methodDictionary = new Dictionary<FollowAxis, Action>();
    private bool _isInitialized;
    
    private enum FollowAxis
    {
        All,
        Vertical,
        Horizontal
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (!thisTransform)
        {
            DebugLogger.Info("Awake", $"thisTransform has not been assigned in the editor. Attempting to cache.", this);
            thisTransform = transform;
        }

        var mainCamera = Camera.main;
        if (!mainCamera) return;
        
        _mainCameraTransform = mainCamera.transform;

        _methodDictionary.Add(FollowAxis.Horizontal, FollowHorizontalWorld);
        _methodDictionary.Add(FollowAxis.Vertical, FollowVerticalWorld);
        _methodDictionary.Add(FollowAxis.All, null);

        _isInitialized = true;
    }


    private void Update()
    {
        Follow();
    }

    private void Follow()
    {
        if (!_isInitialized)
        {
            Initialize();
            return;
        }

        thisTransform.LookAt(_mainCameraTransform);
        _methodDictionary[followAxis]?.Invoke();
    }

    private void FollowHorizontalWorld()
    {
        var eulerAngles = thisTransform.eulerAngles;
        thisTransform.eulerAngles = new Vector3(0, eulerAngles.y, 0);
    }

    private void FollowVerticalWorld()
    {
        var eulerAngles = thisTransform.eulerAngles;
        thisTransform.eulerAngles = new Vector3(eulerAngles.x, 0, 0);
    }
}
