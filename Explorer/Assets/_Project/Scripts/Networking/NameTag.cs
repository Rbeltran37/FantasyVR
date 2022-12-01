using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityCapsuleCollider;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class NameTag : MonoBehaviourPun
{
    [SerializeField] private Text nameText;
    [SerializeField] private GameObject textObject;
    [SerializeField] private string name;
    
    public void SetID(string name)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SendID", RpcTarget.All, photonView.Owner.NickName);

            textObject.SetActive(false);
        }
        else
        {
            nameText.text = photonView.Owner.NickName;
        }
    }

    [PunRPC]
    private void SendID(string name)
    {
        nameText.text = name;
    }
}