using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LaunchRigidbody : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Rigidbody rigidbodyToLaunch;
    [SerializeField] private float force;
    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;
    
    
    public void SetForce(float value)
    {
        force = value;
    }

    private void OnEnable()
    {
        if (DebugLogger.IsNullWarning(rigidbodyToLaunch, this, "Should be set in editor. Attempting to find."))
        {
            rigidbodyToLaunch = GetComponent<Rigidbody>();
            if (DebugLogger.IsNullError(rigidbodyToLaunch, this, "Should be set in editor. Could not be found.")) return;
        }
        
        if (DebugLogger.IsNullWarning(thisTransform, this, "Should be set in editor."))
        {
            thisTransform = gameObject.transform;
        }

        Launch(thisTransform.forward);
    }
    
    [Button]
    public virtual void PopulateParameters()
    {
        if (!thisTransform) thisTransform = transform;
        if (!rigidbodyToLaunch) rigidbodyToLaunch = GetComponent<Rigidbody>();
    }

    public void Launch(Vector3 direction)
    {
        rigidbodyToLaunch.AddForce(direction * force, forceMode);
    }
}
