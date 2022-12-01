using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using RootMotion.Dynamics;
using UnityEngine;

public class PUNPuppetBalanceHandler : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] private PUNPhotonViewManager punPhotonViewManager;
    [SerializeField] private PhotonView photonView;
    
    private bool _canSend = true;
    
    private bool IsMine => photonView && photonView.IsMine;

    private const float BUFFER = .5f;


    private void Awake()
    {
        ResetObject();
    }

    private void OnDestroy()
    {
        if (health)
        {
            health.WasKilled -= Disable;
        }
    }

    public void SendLoseBalance()
    {
        if (PhotonNetwork.OfflineMode) return;

        if (!_canSend) return;

        if (!IsMine) TakeOwnership();
        
        StartCoroutine(LoseBalanceBufferCoroutine());
        
        photonView.RPC(nameof(RPCSendLoseBalance), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPCSendLoseBalance()
    {
        LoseBalance();
    }

    private void LoseBalance()
    {
        if (DebugLogger.IsNullError(behaviourPuppet, "Must be set in editor.", this)) return;

        behaviourPuppet.SetState(BehaviourPuppet.State.Unpinned);
    }

    private IEnumerator LoseBalanceBufferCoroutine()
    {
        _canSend = false;
        
        yield return new WaitForSeconds(BUFFER);

        _canSend = true;
    }

    private void Disable()
    {
        _canSend = false;
    }

    private void TakeOwnership()
    {
        if (DebugLogger.IsNullError(punPhotonViewManager, "Must be set in editor.", this)) return;

        punPhotonViewManager.TakeOwnershipOfAllPhotonViews();
    }
    
    public void ResetObject()
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        
        health.WasKilled += Disable;
        _canSend = true;
    }
}
