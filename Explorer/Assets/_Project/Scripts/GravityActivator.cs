using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GravityActivator : MonoBehaviour
{
    [SerializeField] private string tagName;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float gravityDelay;
    private Vector3 _originPosition;
    private Quaternion _originRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        _originPosition = this.transform.position;
        _originRotation = this.transform.rotation;
        
        if(!rb)
            DebugLogger.Error("Start", "Rigidbody is missing.", this);
        
        if(tagName == String.Empty)
            DebugLogger.Error("Start", "Tag name is missing.", this);
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag.Equals(tagName))
        {
            StartCoroutine(DelayGravity());
        }
    }

    private IEnumerator DelayGravity()
    {
        yield return new WaitForSeconds(gravityDelay);
        EnableGravity();
    }

    private void EnableGravity()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    [Button]
    private void ResetPosition()
    {
        Transform currentTransform = transform;
        
        rb.useGravity = false;
        currentTransform.position = _originPosition;
        currentTransform.rotation = _originRotation;
        rb.isKinematic = true;
    }
}
