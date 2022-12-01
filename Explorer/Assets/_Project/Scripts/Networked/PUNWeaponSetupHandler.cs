using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PUNWeaponSetupHandler : WeaponSetupHandler
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private WeaponFeedback weaponFeedback;
    [SerializeField] private bool attachToForearm;
    
    
    protected override void OnEnable()
    {
        if (!photonView.IsMine)
        {
            if (weaponFeedback)
            {
                Destroy(weaponFeedback);
            }
            
            return;
        }
        
        base.OnEnable();
    }
    
    public override void WeaponPlacement(Rigidbody puppetHandRigid, bool isLeft)
    {
        if (DebugLogger.IsNullError(puppetHandRigid, this)) return;

        if (PhotonNetwork.OfflineMode)
        {
            base.WeaponPlacement(puppetHandRigid, isLeft);
        }

        SetPuppetHandRigid(puppetHandRigid);
        IsLeft = isLeft;
        base.WeaponPlacement(puppetHandRigid, isLeft);

        if (!photonView.IsMine) return;
        
        SendWeaponPlacement(puppetHandRigid, isLeft);
    }

    private void SendWeaponPlacement(Rigidbody puppetHandRigid, bool isLeft)
    {
        var handPhotonViewReference = puppetHandRigid.GetComponent<PhotonViewReference>();
        if (DebugLogger.IsNullDebug(handPhotonViewReference, this)) return;
        if (DebugLogger.IsNullError(photonView, this, "Must be set in editor.")) return;

        var photonViewId = handPhotonViewReference.ViewID;  //Find the remote version of this hand
        photonView.RPC(nameof(RPCWeaponPlacement), RpcTarget.OthersBuffered, photonViewId, isLeft);
    }

    [PunRPC]
    private void RPCWeaponPlacement(int handPhotonViewReferenceId, bool isLeft)
    {
        var playerPhotonView = PhotonNetwork.GetPhotonView(handPhotonViewReferenceId);
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        var puppetHandReference = playerPhotonView.GetComponent<PuppetHandReference>();
        if (DebugLogger.IsNullError(puppetHandReference, this)) return;
        
        var puppetHandRigid = attachToForearm ? puppetHandReference.GetForearmRigidbody(isLeft) : puppetHandReference.GetHandRigidbody(isLeft);
        if (DebugLogger.IsNullError(puppetHandRigid, this)) return;
        
        base.WeaponPlacement(puppetHandRigid, isLeft);
    }
}
