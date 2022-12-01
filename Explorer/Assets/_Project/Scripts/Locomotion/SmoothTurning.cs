using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTurning : MonoBehaviour 
{
    [SerializeField] private LocomotionManager locomotionManager;
    [SerializeField] private Transform playArea;
    [SerializeField] private Transform headset;

    private List<UnityEngine.XR.InputDevice> _devices = new List<UnityEngine.XR.InputDevice>();

    // Start is called before the first frame update
    void Start() 
    {
        InitializeControllers();
    }

    private void FixedUpdate() {
        
        if (locomotionManager.smoothAssignedThumbstick == LocomotionManager.AssignedThumbstick.None)
            return;

        if (_devices.Count == 0)
            InitializeControllers();

        RotateTowardsTarget(SetTargetRotation());
    }
    

    public void InitializeControllers() {

        if (locomotionManager.smoothAssignedThumbstick == LocomotionManager.AssignedThumbstick.None) return;
        
        _devices = new List<UnityEngine.XR.InputDevice>();
        
        if (locomotionManager.smoothAssignedThumbstick == LocomotionManager.AssignedThumbstick.Left) {

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


    private void RotateTowardsTarget(Vector2 axisValue) {
        
        var x = Math.Abs(axisValue.x);
        if (x < locomotionManager.smoothOffSensitivity)
        {
            return;
        }

        playArea.RotateAround(headset.position, Vector3.up, 
            axisValue.x * locomotionManager.smoothTurningSpeed);
    }
}
