using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TripWire : MonoBehaviour
{
    [System.Serializable]
    public class TripWireEvent : UnityEvent { }
    public TripWireEvent TripWireActivated;
    [SerializeField] private string tagName;

    private void Start()
    {
        if(tagName == String.Empty)
            DebugLogger.Error("Start", "Tag name is missing.", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag.Equals(tagName))
        {
            TripWireActivated?.Invoke();
        }
    }
}
