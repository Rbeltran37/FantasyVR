using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class PUNArenaEvent : ArenaEvent
{
    [SerializeField] private PhotonView thisPhotonView;
    [SerializeField] private PUNReadyUp punReadyUp;


    private void OnEnable()
    {
        if (DebugLogger.IsNullError(punReadyUp, "Must be set in editor.", this)) return;
        
        punReadyUp.WasActivated += End;
    }

    [Button]
    public virtual void PopulateParameters()
    {
        if (!thisPhotonView)
        {
            thisPhotonView = GetComponent<PhotonView>();
            if (!thisPhotonView) thisPhotonView = gameObject.AddComponent<PhotonView>();
        }
        
        if (!punReadyUp)
        {
            punReadyUp = gameObject.GetComponent<PUNReadyUp>();
            if (!punReadyUp)
            {
                punReadyUp = gameObject.AddComponent<PUNReadyUp>();
            }
        }
    }

    public override void Begin()
    {
        Initialize();
    }

    protected override void End()
    {
        base.End();
        
        if (!PhotonNetwork.IsMasterClient) return;
        
        thisPhotonView.RPC(nameof(RPCEnd), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    protected void RPCEnd()
    {
        base.End();
    }

    protected void Ready()
    {
        punReadyUp.AttemptIsReady(true);
    }

    protected void UnReady()
    {
        punReadyUp.AttemptIsReady(false);
    }
}
