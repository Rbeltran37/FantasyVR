using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonViewReference : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;

    public bool IsMine => photonView.IsMine;
    public int ViewID => photonView.ViewID;
    public Player Owner => photonView.Owner;
}
