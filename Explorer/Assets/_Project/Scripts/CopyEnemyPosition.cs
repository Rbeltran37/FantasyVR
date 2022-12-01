using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using UnityEngine;

public class CopyEnemyPosition : MonoBehaviour
{
    public Transform root;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (!root)
        {
            root = transform;
        }
    }

    [Button]
    [PunRPC]
    private void SyncPositionLocal()
    {
        photonView.RPC("SyncPositionRemote", RpcTarget.Others, root.position);
    }

    private void SyncPositionRemote(Vector3 position)
    {
        root.position = position;
    }

}
