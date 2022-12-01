using System;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ReadyUpCustomProperty : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;
    private Player _localPlayer;
    private int _numPlayersReady;
    private bool _hasBeenActivated;
    
    private const string IS_READY = "IsReady";
    private const string PLAYER_TAG_NAME = "Player";

    public UnityEvent HasBeenActivated;


    private void Awake()
    {
        _localPlayer = PhotonNetwork.LocalPlayer;
        _photonView = PhotonView.Get(this);

        if (DebugLogger.IsNullError(_photonView, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(_localPlayer, this)) return;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        SetIsReady(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenActivated) return;
        
        if (other.transform.CompareTag(PLAYER_TAG_NAME))
        {
            SetIsReady(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_hasBeenActivated) return;

        if (other.transform.CompareTag(PLAYER_TAG_NAME))
        {
            SetIsReady(false);
        }
    }

    private void SetIsReady(bool isReady)
    {
        var customPlayerProperties = new Hashtable();
        customPlayerProperties.Add(IS_READY, isReady);
        PhotonNetwork.LocalPlayer.SetCustomProperties(customPlayerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable hashtable)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        CheckIfAllPlayersInRoomAreReady();
    }

    private void CheckIfAllPlayersInRoomAreReady()
    {
        if (_hasBeenActivated) return;
        
        _numPlayersReady = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(IS_READY, out var isPlayerReady))
            {
                if ((bool)isPlayerReady)
                {
                    _numPlayersReady++;
                }
            }
        }

        if (!IsRoomReady()) return;
        
        Activate();
    }
    
    private bool IsRoomReady()
    {
        return _numPlayersReady == PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void Activate()
    {
        HasBeenActivated?.Invoke();
        _hasBeenActivated = true;
    }
}
