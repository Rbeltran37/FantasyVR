using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PUNPuppetLauncher : PuppetLauncher
{
    [SerializeField] private PUNPhotonViewManager punPhotonViewManager;
    [SerializeField] private PhotonView photonViewHips;

    public override void Launch(float impulse)
    {
        if (PhotonNetwork.OfflineMode)
        {
            base.Launch(impulse);
        }

        if (DebugLogger.IsNullError(photonViewHips, "Must be set in editor.", this)) return;

        //TransferOwnership to player launching, if player is not owner
        if (!photonViewHips.IsMine)
        {
            if (DebugLogger.IsNullError(punPhotonViewManager, "Must be set in editor.", this)) return;

            punPhotonViewManager.TakeOwnershipOfAllPhotonViews();
        }
        
        base.Launch(impulse);
    }
}
