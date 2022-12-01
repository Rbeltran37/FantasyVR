using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class PooledRigidbody : PooledObject
{
    [SerializeField] protected Rigidbody ThisRigidbody;

    private bool _resetIsKinematic;
    private bool _resetUseGravity;

    
    [Button]
    public override void PopulateParameters()
    {
        base.PopulateParameters();
        
        if (!ThisRigidbody) ThisRigidbody = GetComponent<Rigidbody>();
    }
    
    public Rigidbody GetRigidbody()
    {
        return ThisRigidbody;
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (DebugLogger.IsNullWarning(ThisRigidbody, this, "Should be set in editor. Attempting to find."))
        {
            ThisRigidbody = GetComponent<Rigidbody>();
            if (DebugLogger.IsNullError(ThisRigidbody, this, "Should be set in editor. Could not be found.")) return;
        }

        _resetIsKinematic = ThisRigidbody.isKinematic;
        _resetUseGravity = ThisRigidbody.useGravity;
    }

    protected override void ResetObject()
    {
        base.ResetObject();
        
        if (DebugLogger.IsNullError(ThisRigidbody, this, "Should be set in editor.")) return;

        ThisRigidbody.velocity = Vector3.zero;
        ThisRigidbody.angularVelocity = Vector3.zero;
        ThisRigidbody.isKinematic = _resetIsKinematic;
        ThisRigidbody.useGravity = _resetUseGravity;
    }
}
