using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class GrabAction : MonoBehaviour
{
    [SerializeField] private PlayerHand playerHand;

    private IGrabbable _currentGrabbable;
    private bool _isGrabbing;
    

    private void OnTriggerEnter(Collider other)
    {
        CheckForGrabbableObjects(other);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_isGrabbing) return;
        
        var grabbable = other.GetComponent<IGrabbable>();
        if (grabbable == null) return;
        if (_currentGrabbable != grabbable) return;
        
        _currentGrabbable = null;
        
        playerHand.Rest();
    }
    
    private void CheckForGrabbableObjects(Collider other)
    {
        if (_currentGrabbable != null) return;
        if (_isGrabbing) return;

        var grabbable = other.GetComponent<IGrabbable>();
        if (grabbable == null) return;

        _currentGrabbable = grabbable;
        
        playerHand.HoldOpen();
    }

    [Button]
    public void AttemptGrab()
    {
        _isGrabbing = true;
        
        if (_currentGrabbable != null)
        {
            playerHand.GrabObject();
        }
        else
        {
            playerHand.Close();
        }
    }

    [Button]
    public void AttemptRelease()
    {
        _isGrabbing = false;
        
        if (_currentGrabbable != null)
        {
            if (_currentGrabbable.IsGrabbedBy(playerHand.GetInteractor()))
            {
                playerHand.Open();
            }
            else
            {
                playerHand.Rest();
            }

            _currentGrabbable = null;
        }
        else
        {
            playerHand.Rest();
        }
    }
}
