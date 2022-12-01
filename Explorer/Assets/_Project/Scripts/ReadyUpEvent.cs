using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ReadyUpEvent : ArenaEvent
{
    private string _playerTagName = "Player";
    private Player _player;
    private PhotonView _photonView;
    public List<string> players = new List<string>();
    private const string ISREADY = "IsReady";
    private int count = 0;
    //private bool _roomIsReady;

    public UnityEvent roomIsReady;

    // Start is called before the first frame update
    void Start()
    {
        _player = PhotonNetwork.LocalPlayer;
        _photonView = PhotonView.Get(this);

        if (!_photonView)
            DebugLogger.Error("Start", "PhotonView is missing.", this);

        if(_player.Equals(null))
            DebugLogger.Error("Start", "Unable to find local player.", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag(_playerTagName))
        {
            var customPlayerProperties = new ExitGames.Client.Photon.Hashtable();
            customPlayerProperties.Add(ISREADY, true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(customPlayerProperties);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag(_playerTagName))
        {
            var customPlayerProperties = new ExitGames.Client.Photon.Hashtable();
            customPlayerProperties.Add(ISREADY, false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(customPlayerProperties);
        }
    }

    /*public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable hashtable)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        CheckIfAllPlayersInRoomAreReady();
    }
    */

    public void CheckIfAllPlayersInRoomAreReady()
    {
        count = 0;
        players.Clear();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(ISREADY, out isPlayerReady))
            {
                if ((bool)isPlayerReady)
                {
                    count++;
                    players.Add(p.UserId);
                }
            }
        }

        if (IsRoomReady())
        {
            roomIsReady?.Invoke();
            End();
        }
    }

    private bool IsRoomReady()
    {
        return count == PhotonNetwork.CurrentRoom.PlayerCount;
    }
}
