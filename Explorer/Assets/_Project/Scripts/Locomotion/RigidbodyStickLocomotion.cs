using System;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyStickLocomotion : MonoBehaviour
{
    [SerializeField] private Transform playArea;
    [SerializeField] private Transform headset;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    [SerializeField] private Transform velocityTargetTransform;
    [SerializeField] private Rigidbody playAreaRigidBody;
    [SerializeField] private ControllerAxisValues controllerAxisValues;
    [SerializeField] private Jump jump;
    [SerializeField] private LocomotionManager locomotionManager;

    private bool _isAssigned;
    private bool _useLeftController;
    private Transform _currentController;

    public Action<float, float> WasActivated;
    public Action IsRunning;
    public Action IsNotActive;
    
    
    private void Awake()
    {
        if (DebugLogger.IsNullError(playArea, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(playAreaRigidBody, "Should be set in editor.", this)) return;
        if (DebugLogger.IsNullError(velocityTargetTransform, "Should be set in editor.", this)) return;
        if (DebugLogger.IsNullError(locomotionManager, "Should be set in editor.", this)) return;

        velocityTargetTransform.SetParent(playArea);
        
        InitializeController();
    }

    private void FixedUpdate() {
        
        if (!_isAssigned) return;
        if (jump.IsJumping) return;
        
        var thumbStickAxisValue = controllerAxisValues.GetAxisValueFromController(_useLeftController);
        SetVelocityTargetPosition(thumbStickAxisValue);
        SetRigidBodyVelocity(thumbStickAxisValue);
    }

    public void InitializeController() {
        
        _isAssigned = locomotionManager.stickAssignedThumbstick != LocomotionManager.AssignedThumbstick.None;
        if (!_isAssigned) return;

        _useLeftController = locomotionManager.stickAssignedThumbstick == LocomotionManager.AssignedThumbstick.Left;
        _currentController = _useLeftController ? leftController : rightController;
    }

    private void SetVelocityTargetPosition(Vector2 axisValue)
    {
        velocityTargetTransform.localPosition = Vector3.zero;

        if (axisValue.Equals(new Vector2()) ||
            !(Mathf.Abs(axisValue.x) > locomotionManager.stickLocomotionOffSensitivity ||
              Mathf.Abs(axisValue.y) > locomotionManager.stickLocomotionOffSensitivity))
        {
            IsNotActive?.Invoke();
            return;
        }
        
        //ELSE STATEMENT WAS ADDED IN ORDER TO DETECT LOCOMOTION ACTIVATION.
        WasActivated?.Invoke(axisValue.x, axisValue.y);

        float currentLocalEulerY;
        if (locomotionManager.stickUseHeadsetForward)
        {
            if (!headset) return;
            currentLocalEulerY  = headset.localEulerAngles.y;
        }
        else
        {
            if (!_currentController) return;
            currentLocalEulerY = _currentController.localEulerAngles.y;
        }

        var thumbStickVector3 = new Vector3(axisValue.x, 0, axisValue.y);
        var currentLocalEulerYVector3 = new Vector3(0, currentLocalEulerY, 0);
        velocityTargetTransform.localPosition += Quaternion.Euler(currentLocalEulerYVector3) * thumbStickVector3;
    }

    private void SetRigidBodyVelocity(Vector2 axisValue) {

        var playAreaPosition = playArea.position;
        var velocityTargetPosition = velocityTargetTransform.position;
        var velocityVector = velocityTargetPosition - playAreaPosition;
        
        var augmentedVelocity = velocityVector * locomotionManager.stickLocomotionSpeed;
        var oldVelocity = playAreaRigidBody.velocity;
        var newVelocity = new Vector3(augmentedVelocity.x, oldVelocity.y, augmentedVelocity.z);

        playAreaRigidBody.velocity = newVelocity;
        if (locomotionManager.useMoveInPlace)
        {
            if (axisValue.y > locomotionManager.runAxisThreshold)
            {
                IsRunning?.Invoke();
            }
            else
            {
                IsNotActive?.Invoke();
            }
        }
    }
}
