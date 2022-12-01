using System;
using System.Collections;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

public class MoveToLocation : PUNArenaEvent
{
    private bool _isInTrigger;

    private const string RIG_TAG = "Rig";


    private void OnTriggerEnter(Collider other)
    {
        if (_isInTrigger) return;
        
        if (IsLocalPlayer(other))
        {
            _isInTrigger = true;
            Ready();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isInTrigger) return;

        if (IsLocalPlayer(other))
        {
            _isInTrigger = false;
            UnReady();
        }
    }

    //TODO Can be optimized
    private bool IsLocalPlayer(Collider other)
    {
        if (!other.CompareTag(RIG_TAG)) return false;
        
        var photonViewReference = other.GetComponent<PhotonViewReference>();
        return photonViewReference && photonViewReference.IsMine;
    }
}
