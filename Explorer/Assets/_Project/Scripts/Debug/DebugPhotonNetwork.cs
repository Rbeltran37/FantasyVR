using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DebugPhotonNetwork : MonoBehaviour
{
    [SerializeField] private PhotonView[] photonViews;
    [SerializeField] private int numPhotonViewIds;
    
    
    private void Update()
    {
        photonViews = PhotonNetwork.PhotonViews;
        numPhotonViewIds = PhotonNetwork.ViewCount;
    }
}
