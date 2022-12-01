using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class DebugMultiplayer : MonoBehaviour
{
    [SerializeField] private MoveToLocation[] moveToLocations;

    private bool _isActive;
    private Transform _localCameraRig;

    private const int TRIGGER_EVENTS_BUFFER = 5;


    private void Awake()
    {
        CoroutineCaller.WaitToConnect(TriggerEvents);
    }
    
    [Button]
    public void PopulateParameters()
    {
        moveToLocations = FindObjectsOfType<MoveToLocation>();
    }

    private void TriggerEvents()
    {
        if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(TriggerEventsCoroutine());
        }
    }

    [Button]
    public void MoveToNextLocation()
    {
        if (!_localCameraRig)
        {
            var camera = Camera.main;
            if (camera)
            {
                var cameraTransform = camera.transform;
                if (cameraTransform) _localCameraRig = cameraTransform.parent;
            }
        }
        
        if (!_localCameraRig) return;
        
        foreach (var moveToLocation in moveToLocations)
        {
            if (moveToLocation.gameObject.activeSelf)
            {
                _localCameraRig.position = moveToLocation.transform.position;
                return;
            }
        }
    }

    [Button]
    public void GrabReward()
    {
        var rewards = FindObjectsOfType<Reward>();
        foreach (var reward in rewards)
        {
            if (reward.gameObject.activeSelf)
            {
                var photonView = reward.GetComponent<PhotonView>();
                if (photonView.IsMine)
                {
                    reward.RewardWasUsed.Invoke();
                    return;
                }
            }
        }
    }

    private IEnumerator TriggerEventsCoroutine()
    {
        while (_isActive)
        {
            MoveToNextLocation();
            GrabReward();
            yield return new WaitForSeconds(TRIGGER_EVENTS_BUFFER);
        }
    }

    [Button]
    public void ToggleActivation()
    {
        _isActive = !_isActive;
        TriggerEvents();
    }
}
