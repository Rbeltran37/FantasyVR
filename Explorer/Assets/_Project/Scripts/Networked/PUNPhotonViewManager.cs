using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class PUNPhotonViewManager : MonoBehaviour
{
    [SerializeField] private PhotonView[] photonViews;

    
    [Button]
    public void PopulateParameters()
    {
        photonViews = GetComponentsInChildren<PhotonView>();
    }

    public void TakeOwnershipOfAllPhotonViews()
    {
        foreach (var photonView in photonViews)
        {
            if (!photonView.IsMine) photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }
}
