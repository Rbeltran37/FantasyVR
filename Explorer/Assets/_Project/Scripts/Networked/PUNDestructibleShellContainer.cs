using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PUNDestructibleShellContainer : PooledRigidbody
{
    [SerializeField] private DestructibleShell destructibleShell;

    private bool IsMine => ThisPhotonView && ThisPhotonView.IsMine;


    protected override void Awake()
    {
        base.Awake();

        if (DebugLogger.IsNullError(destructibleShell, this, "Must be set in editor.")) return;

        destructibleShell.WasDemolished += SendDemolish;
    }

    private void OnDestroy()
    {
        if (DebugLogger.IsNullError(destructibleShell, this, "Must be set in editor.")) return;

        destructibleShell.WasDemolished -= SendDemolish;
    }

    public override void PopulateParameters()
    {
        base.PopulateParameters();

        if (!destructibleShell)
        {
            destructibleShell = GetComponent<DestructibleShell>();
            if (!destructibleShell) destructibleShell = ThisGameObject.AddComponent<DestructibleShell>();
        }
        
        destructibleShell.PopulateParameters();
    }

    public void SendDemolish(Vector3 impactOrigin, float force)
    {
        if (PhotonNetwork.OfflineMode) return;
        
        if (DebugLogger.IsNullError(ThisPhotonView, this, "Must be set in editor.")) return;
        if (!IsMine) return;

        ThisPhotonView.RPC(nameof(RPCDemolish), RpcTarget.OthersBuffered, impactOrigin, force);
    }

    [PunRPC]
    public void RPCDemolish(Vector3 impactOrigin, float force)
    {
        if (DebugLogger.IsNullError(destructibleShell, this, "Must be set in editor.")) return;
        
        destructibleShell.Demolish(impactOrigin, force);
    }
}
