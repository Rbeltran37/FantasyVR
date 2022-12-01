using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SkillSynchronization : MonoBehaviour
{

    //[SerializeField] private HolsterEquipHandler holsterEquipHandler;     //TODO Obsolete, deleted class
    [SerializeField] private PhotonView photonView;
    

    private void OnEnable()
    {
        //holsterEquipHandler.skillEquip += OnHolsterSkillSync;        //TODO obsolete
    }

    private void OnDisable()
    {
        //holsterEquipHandler.skillEquip -= OnHolsterSkillSync;        //TODO obsolete
    }
    
    private void OnHolsterSkillSync()
    {
        var name = PhotonNetwork.LocalPlayer.TagObject.ToString();
        photonView.RPC("HolsterSkillSynchronizer", RpcTarget.Others, name);
    }

    [PunRPC]
    private void HolsterSkillSynchronizer(string name)
    {
        Debug.Log("The Gameobject of the player that has grabbed: " + name);
        GameObject player = GameObject.Find(name);
        
        int id = player.GetComponent<PhotonView>().ViewID;
    }
}
