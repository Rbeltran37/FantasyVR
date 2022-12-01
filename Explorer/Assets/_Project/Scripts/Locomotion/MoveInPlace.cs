using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveInPlace : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private LocomotionManager locomotionManager;
    [SerializeField] private RigidbodyStickLocomotion rigidbodyStickLocomotion;
    [SerializeField] private Transform playArea;
    [SerializeField] private Transform headset;
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;
    

    private bool isMovementActivated;
    private int averagePeriod;
    private List<Transform> trackedObjects = new List<Transform>();
    private Dictionary<Transform, List<float>> movementList = new Dictionary<Transform, List<float>>();
    private Dictionary<Transform, float> previousYPositions = new Dictionary<Transform, float>();
    private Vector3 initialGaze;
    private float currentSpeed;
    private Vector3 currentDirection;
    private Vector3 previousDirection;


    private void Awake() {

        currentDirection = Vector3.zero;
        previousDirection = Vector3.zero;
        averagePeriod = 60;
        currentSpeed = 0f;

        trackedObjects.Add(leftController);
        trackedObjects.Add(rightController);
        movementList.Add(leftController, new List<float>());
        movementList.Add(rightController, new List<float>());
        previousYPositions.Add(leftController, 0);
        previousYPositions.Add(rightController, 0);

        isMovementActivated = false;

        if (rigidbodyStickLocomotion)
        {
            rigidbodyStickLocomotion.IsRunning += EngageButtonPressed;
            rigidbodyStickLocomotion.IsNotActive += EngageButtonReleased;
        }

        enabled = false;
    }

    private void OnDestroy()
    {
        if (rigidbodyStickLocomotion)
        {
            rigidbodyStickLocomotion.IsRunning -= EngageButtonPressed;
            rigidbodyStickLocomotion.IsNotActive -= EngageButtonReleased;
        }
    }


    void FixedUpdate()
    {
        // If Move In Place is currently engaged.
        if (isMovementActivated) {
            // Initialize the list average.
            float speed = Mathf.Clamp(((locomotionManager.moveInPlaceSpeedScale * 350) * 
                                       (CalculateListAverage() / 2)), 0f, locomotionManager.moveInPlaceMaxSpeed);
            previousDirection = currentDirection;

            if (locomotionManager.moveInPlaceUseHeadset)
            {
                currentDirection = headset.forward;
            }
            else
            {
                currentDirection = CalculateControllerRotationDirection(DetermineAverageControllerRotation() * Vector3.forward);
            }
            // Update our current speed.
            currentSpeed = speed;
        }
        else if (currentSpeed > 0f) {
            currentSpeed -= locomotionManager.moveInPlaceDeceleration;
        }
        else {
            currentSpeed = 0f;
            currentDirection = Vector3.zero;
            previousDirection = Vector3.zero;
        }

        SetDeltaTransformData();
        MovePlayArea(currentDirection, currentSpeed);
    }


    public void EngageButtonPressed()
    {
        enabled = true;
        isMovementActivated = true;
    }

    public void EngageButtonReleased()
    {

        currentSpeed = 0;
        currentDirection = Vector3.zero;
        previousDirection = Vector3.zero;
        isMovementActivated = false;
        enabled = false;
    }


    private void MovePlayArea(Vector3 moveDirection, float moveSpeed) {
        Vector3 movement = (moveDirection * moveSpeed) * Time.fixedDeltaTime;
        if (playArea != null) {
            Vector3 finalPosition = new Vector3(movement.x + playArea.position.x, playArea.position.y, movement.z + playArea.position.z);
            playArea.position = finalPosition;
        }
    }

    private float CalculateListAverage() {
        float listAverage = 0;

        for (int i = 0; i < trackedObjects.Count; i++) {
            Transform trackedObj = trackedObjects[i];
            // Get the amount of Y movement that's occured since the last update.
            float previousYPosition = previousYPositions[trackedObjects[i]];
            float deltaYPostion = Mathf.Abs(previousYPosition - trackedObj.transform.localPosition.y);

            // Convenience code.
            List<float> trackedObjList = movementList[trackedObjects[i]];

            // Cap off the speed.
            if (deltaYPostion > locomotionManager.moveInPlaceSensitivity) {
                trackedObjList.Add(locomotionManager.moveInPlaceSensitivity);
            }
            else {
                trackedObjList.Add(deltaYPostion);
            }

            // Keep our tracking list at m_averagePeriod number of elements.
            if (trackedObjList.Count > averagePeriod) {
                trackedObjList.RemoveAt(0);
            }

            // Average out the current tracker's list.
            float sum = 0;
            for (int j = 0; j < trackedObjList.Count; j++) {
                float diffrences = trackedObjList[j];
                sum += diffrences;
            }
            float avg = sum / averagePeriod;

            // Add the average to the the list average.
            listAverage += avg;
        }

        return listAverage;
    }

    private Quaternion DetermineAverageControllerRotation() {
        // Build the average rotation of the controller(s)
        Quaternion newRotation;

        // Both controllers are present
        if (leftController != null && rightController != null) {
            newRotation = AverageRotation(leftController.transform.rotation, rightController.transform.rotation);
        }
        // Left controller only
        else if (leftController != null && rightController == null) {
            newRotation = leftController.transform.rotation;
        }
        // Right controller only
        else if (rightController != null && leftController == null) {
            newRotation = rightController.transform.rotation;
        }
        // No controllers!
        else {
            newRotation = Quaternion.identity;
        }

        return newRotation;
    }

    // Returns the average of two Quaternions
    private Quaternion AverageRotation(Quaternion rot1, Quaternion rot2) {
        return Quaternion.Slerp(rot1, rot2, 0.5f);
    }


    private Vector3 CalculateControllerRotationDirection(Vector3 calculatedControllerDirection) {
        return (Vector3.Angle(previousDirection, calculatedControllerDirection) <= 90f ? calculatedControllerDirection : previousDirection);
    }

    private void SetDeltaTransformData() {
        for (int i = 0; i < trackedObjects.Count; i++) {
            Transform trackedObj = trackedObjects[i];
            // Get delta postions and rotations
            previousYPositions[trackedObj] = trackedObj.transform.localPosition.y;
        }
    }
}
