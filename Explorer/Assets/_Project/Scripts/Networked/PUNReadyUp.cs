using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PUNReadyUp : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private PUNPlayerTargetManager punPlayerTargetManager;

    private int _numPlayersReady;
    private bool _wasActivated;

    public Action WasActivated;
    
    
    [Button]
    public void PopulateParameters()
    {
        if (!punPlayerTargetManager) punPlayerTargetManager = FindObjectOfType<PUNPlayerTargetManager>();
        
        if (!photonView)
        {
            photonView = gameObject.GetComponent<PhotonView>();
            if (!photonView)
            {
                photonView = gameObject.AddComponent<PhotonView>();
            }
        }
    }

    public void AttemptIsReady(bool isReady)
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(isReady)}={isReady}",this);

        if (_wasActivated) return;
        
        SendIsReady(isReady);
        IsReady(isReady);
    }

    private void SendIsReady(bool isReady)
    {
        if (PhotonNetwork.IsMasterClient) return;
        
        photonView.RPC(nameof(RPCIsReady), RpcTarget.MasterClient, isReady);
    }

    [PunRPC]
    private void RPCIsReady(bool isReady)
    {
        IsReady(isReady);
    }

    private void IsReady(bool isReady)
    {
        if (isReady) IncrementPlayerCount();
        else DecrementPlayerCount();
    }

    private void IncrementPlayerCount()
    {
        _numPlayersReady++;
        if (_numPlayersReady == punPlayerTargetManager.GetNumPlayers())
        {
            WasActivated?.Invoke();
            _wasActivated = true;
        }
    }

    private void DecrementPlayerCount()
    {
        _numPlayersReady--;
    }
}
