using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

public class PUNDamageDealt : DamageDealt
{
    [SerializeField] private PhotonView photonView;

    private bool IsOnlineAndNotMine => !PhotonNetwork.OfflineMode && (!photonView || !photonView.IsMine);
    
    
    protected override void OnTriggerEnter(Collider other)
    {
        if (IsOnlineAndNotMine) return;
        
        base.OnTriggerEnter(other);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (IsOnlineAndNotMine) return;
        
        base.OnCollisionEnter(other);
    }
}
