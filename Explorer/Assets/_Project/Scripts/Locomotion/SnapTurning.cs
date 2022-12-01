using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SnapTurning : MonoBehaviour {

    [SerializeField] private LocomotionManager locomotionManager;
    
    //Changed from private to public for accessibility in CopyPosition.cs
    [SerializeField] public Transform playArea;
    [SerializeField] public Transform headset;

    private List<UnityEngine.XR.InputDevice> _devices = new List<UnityEngine.XR.InputDevice>();
    private bool _isTurning = false;
    private int _currentTurnAngle = 90;


    void Start() {
        InitializeControllers();
        SetTurnAngle();
    }

    private void FixedUpdate() {
        
        if (!locomotionManager ||
            locomotionManager.snapAssignedThumbstick == LocomotionManager.AssignedThumbstick.None)
            return;

        if (_devices.Count == 0)
            InitializeControllers();
        
        RotateToTarget(SetTargetRotation());
    }

    public void InitializeControllers() {

        if (!locomotionManager ||
            locomotionManager.snapAssignedThumbstick == LocomotionManager.AssignedThumbstick.None)
            return;
        
        if (locomotionManager.snapAssignedThumbstick == LocomotionManager.AssignedThumbstick.Left) {

            UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.LeftHanded, _devices);
        }
        else {

            UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.RightHanded, _devices);
        }
    }


    private Vector2 SetTargetRotation() {

        var axisValue = new Vector2();
        foreach (var device in _devices) {

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis,
                                          out axisValue)
                && !axisValue.Equals(new Vector2())) {

                return axisValue;
            }
        }

        return axisValue;
    }

    private void RotateToTarget(Vector2 axisValue)
    {
        var x = Math.Abs(axisValue.x);
        if (x < locomotionManager.snapOffSensitivity)
        {
            _isTurning = false;
            return;
        }
        if (x < locomotionManager.snapOnSensitivity)
        {
            return;
        }
        
        if (_isTurning)
            return;

        int angleSign = 1;
        if (axisValue.x < 0)
            angleSign = -1;

        playArea.RotateAround(headset.position, Vector3.up, angleSign * _currentTurnAngle);
        _isTurning = true;
    }

    public void SetTurnAngle()
    {
        _currentTurnAngle = (int) locomotionManager.snapTurnDegrees;
    }
}
