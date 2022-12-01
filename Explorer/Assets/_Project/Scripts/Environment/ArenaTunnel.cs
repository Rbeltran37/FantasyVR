using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ArenaTunnel : MonoBehaviourPunCallbacks
{
    public Dictionary<string, bool> playerReadyList = new Dictionary<string, bool>();
    public List<string> readyNames = new List<string>();
    public int readyCount = 0;
    private const string ARENA = "Networked Collesium";
    private LevelLoader levelLoader;
    
    private void Awake()
    {
        levelLoader = GameObject.Find("[Scene Manager]").GetComponent<LevelLoader>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var customPlayerProperties = new ExitGames.Client.Photon.Hashtable();
        customPlayerProperties.Add("IsReady", true);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customPlayerProperties);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var customPlayerProperties = new ExitGames.Client.Photon.Hashtable();
        customPlayerProperties.Add("IsReady", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customPlayerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        readyCount = 0;
        
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue("IsReady", out isPlayerReady))
            {
                Debug.Log(p.NickName + " --- " + (bool)isPlayerReady);
                if ((bool)isPlayerReady)
                {
                    Debug.Log(p.NickName + " is READY.");
                    readyCount++;
                    readyNames.Add(p.NickName);
                }else
                {
                    Debug.Log(p.NickName + " is NOT READY.");
                    readyCount--;
                    readyNames.Remove(p.NickName);
                }
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                Debug.Log("This is where we initiate loading.");
            }
        }

        /*if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue("IsReady", out isPlayerReady))
                {
                    Debug.Log(p.NickName + " --- " + (bool)isPlayerReady);
                    if ((bool)isPlayerReady)
                    {
                        Debug.Log(p.NickName + " is READY.");
                        readyCount++;
                        readyNames.Add(p.NickName);
                    }else
                    {
                        Debug.Log(p.NickName + " is NOT READY.");
                        readyCount--;
                        readyNames.Remove(p.NickName);
                    }
                }
            }
            
            if (readyCount == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                Debug.Log("Ready count equals number of players in the room.");
                
                if (levelLoader)
                {
                    Debug.Log("This is where we would initiate load sequence");
                    
                    //Might need to add a pause here to wait for RPC to reach clients
                    //levelLoader.LoadNetworkedLevel(ARENA);
                }
                else
                {
                    Debug.Log("Level Loader not found in " + this.name);
                }
            }
        }*/
    }

    
}
