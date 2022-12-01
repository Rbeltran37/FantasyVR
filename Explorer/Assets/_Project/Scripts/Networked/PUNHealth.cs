using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

public class PUNHealth : Health
{
    [SerializeField] private PhotonView photonView;
    
    public override void Subtract(float amount)
    {
        if (PhotonNetwork.OfflineMode)
        {
            base.Subtract(amount);
            return;
        }
        
        photonView.RPC(nameof(RPCSubtract), RpcTarget.All, amount);
    }

    [PunRPC]
    private void RPCSubtract(float amount)
    {
        base.Subtract(amount);
    }
}
