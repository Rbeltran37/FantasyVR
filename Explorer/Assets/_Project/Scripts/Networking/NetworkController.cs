using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField] private PUNPlayerTargetManager _punPlayerTargetManager;

    public static NetworkController instance;

    [Header("Room Settings")] public RoomOptions roomSettings = new RoomOptions();
    public string _room = "Test Facility";
    public byte maxPlayers = 4;
    [SerializeField] private string gameVersion = "1";
    private LevelLoader levelLoader;

    private void Awake()
    {
        //set up singleton
        if (instance == null)
        {
            instance = this;
        }
        
        levelLoader = GameObject.Find("[Scene Manager]").GetComponent<LevelLoader>();
        
        PhotonNetwork.AutomaticallySyncScene = true;
        GameObject playerPrefab = PhotonNetwork.Instantiate("PhotonPlayer", new Vector3(0f, .1f, 0f), Quaternion.identity, 0);
    }

    private void Start()
    {
        roomSettings.MaxPlayers = maxPlayers;
        Connect();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to Photon Master Server");

        PhotonNetwork.JoinOrCreateRoom(_room, roomSettings, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join room failed -- creating room");
        PhotonNetwork.CreateRoom("NewRoom", roomSettings, TypedLobby.Default);
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log("Network Controller calling on joined room from " + SceneManager.GetActiveScene().name);
        levelLoader.DisableOverlayOnJoinedRoom();
        
        GameObject playerPrefab = PhotonNetwork.Instantiate("PhotonPlayer", new Vector3(0f, .1f, 0f), Quaternion.identity, 0);
        Debug.Log("Attempted to instantiate Photon Player Prefab");
        //LocalAddPlayerTarget(playerPrefab);
        //LocalAddPlayersInRoom();
    }

    private void LocalAddPlayersInRoom()
    {
        //_punPlayerTargetManager.LocalAddOtherPlayers();        //TODO deprecated
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room -- room already exists");
    }
    
    private void Connect()
    {
        // #Critical, we must first and foremost connect to Photon Online Server.
        //PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerTarget(otherPlayer);
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);
    }
    
    [PunRPC]
    private void NetworkedAddPlayerTarget(int actorNumber)
    {
        var player = PhotonNetwork.CurrentRoom.Players[actorNumber];
        var playerPrefab = player.TagObject as GameObject;
        if (playerPrefab)
        {
            var playerTarget = playerPrefab.GetComponent<PlayerTarget>();
            if (playerTarget)
            {
                //_punPlayerTargetManager.AddPlayerTarget(playerTarget, actorNumber);        //TODO moved method to other class
            }
        }
    }
    
    private void LocalAddPlayerTarget(GameObject player)
    {
        if (_punPlayerTargetManager && player)
        {
            var photonViewComponent = player.GetComponent<PhotonView>();
            if (photonViewComponent)
            {
                var networkedPlayer = photonViewComponent.GetComponent<NetworkedPlayer>();
                if (networkedPlayer)
                {
                    var playerPrefab = networkedPlayer.playerPrefab;
                    photonViewComponent.Owner.TagObject = playerPrefab;
                
                    var playerTarget = playerPrefab.GetComponent<PlayerTarget>();
                    if (playerTarget)
                    {
                        //_punPlayerTargetManager.AddPlayerTarget(playerTarget, photonViewComponent.OwnerActorNr);        //TODO moved method to other class
                        photonView.RPC("NetworkedAddPlayerTarget", RpcTarget.Others, photonViewComponent.OwnerActorNr);
                    }
                }
            }
        }
    }
    
    private void RemovePlayerTarget(Player otherPlayer)
    {
        if (_punPlayerTargetManager && otherPlayer != null)
        {
            var tagObject = otherPlayer.TagObject as PlayerTarget;
            if (tagObject != null)
            {
                //_punPlayerTargetManager.RemovePlayerTarget(tagObject);        //TODO moved method to other class
            }
        }
    }
}