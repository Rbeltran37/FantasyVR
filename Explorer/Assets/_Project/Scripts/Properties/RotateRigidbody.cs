using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class RotateRigidbody : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbodyToRotate;
    [SerializeField] private bool useRandomRotation;
    [SerializeField] private float torqueSpeed = 15;
    [SerializeField] private Vector3 torqueRotation = new Vector3(0, 0, 90);
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;
    [SerializeField] private CustomEnums.Execution execution;


    private void Awake()
    {
        if (execution != CustomEnums.Execution.Awake) return;

        RotateRigid();
    }

    private void OnEnable()
    {
        if (execution != CustomEnums.Execution.OnEnable) return;
        
        RotateRigid();
    }

    private void Start()
    {
        if (execution != CustomEnums.Execution.Start) return;
        
        RotateRigid();
    }
    
    [Button]
    public virtual void PopulateParameters()
    {
        if (!rigidbodyToRotate) rigidbodyToRotate = GetComponent<Rigidbody>();
    }
    
    public void RotateRigid()
    {
        if (DebugLogger.IsNullError(rigidbodyToRotate, this, "Must be set in editor.")) return;
        
        var rotationToUse = useRandomRotation ? Random.onUnitSphere : torqueRotation;

        rigidbodyToRotate.AddRelativeTorque(rotationToUse * torqueSpeed, forceMode);
    }

    public void SetTorqueSpeed(float value)
    {
        torqueSpeed = value;
    }
}
