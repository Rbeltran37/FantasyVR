using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

//TODO should possibly be singleton
public class PUNPlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject networkedPlayerPrefab;
    [SerializeField] private Transform[] spawnTransforms;
    [SerializeField] private PUNPlayerTargetManager punPlayerTargetManager;

    private string _roomName = TEST_ROOM;
    private GameObject _playerInstance;
    private RoomOptions _roomSettings = new RoomOptions();

    private const byte MAX_PLAYERS = 4;        //TODO change eventually
    private const string NEW_ROOM_NAME = "NewRoom";        //TODO don't hardcode
    private const string TEST_ROOM = "TestRoom";        //TODO don't hardcode

    
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        
        if (DebugLogger.IsNullOrEmptyError(spawnTransforms, "Must be set in editor.", this)) return;
    }

    private void Start()
    {
        _roomSettings.MaxPlayers = MAX_PLAYERS;
        Connect();
        
        if (PhotonNetwork.IsConnectedAndReady && !_playerInstance)
        {
            AttemptToInstantiatePlayer();
        }
    }

    public override void OnConnectedToMaster()
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, 
            $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} has connected to master.", this);

        PhotonNetwork.JoinOrCreateRoom(_roomName, _roomSettings, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, 
            $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} Join room failed -- creating room.", this);
        
        //TODO don't hardcode room name
        PhotonNetwork.CreateRoom(NEW_ROOM_NAME, _roomSettings, TypedLobby.Default);
    }
    
    public override void OnJoinedRoom()
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, 
            $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} has joined room.", this);
        
        //TODO use ObjectPool
        //TODO don't always instantiate same player prefab, scene dependant
        AttemptToInstantiatePlayer();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        DebugLogger.Error(MethodBase.GetCurrentMethod().Name, 
            $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} Failed to create room -- room already exists", this);
    }
    
    private void Connect()
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, 
            $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer} is connecting.", this);
        
        // #Critical, we must first and foremost connect to Photon Online Server.
        //PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, 
            $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer}: otherPlayer={otherPlayer} has left room.", 
            this);
        
        if (otherPlayer.IsInactive)
        {
            DebugLogger.Info(MethodBase.GetCurrentMethod().Name, 
                $"{nameof(PhotonNetwork.LocalPlayer)}={PhotonNetwork.LocalPlayer}: otherPlayer={otherPlayer} IsInactive.", 
                this);
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, 
                $"{nameof(PhotonNetwork.IsMasterClient)}. Calling {nameof(PhotonNetwork.DestroyPlayerObjects)} on {nameof(otherPlayer)}={otherPlayer}", this);
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        }
    }

    private void AttemptToInstantiatePlayer()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            AskToInstantiatePlayer(PhotonNetwork.LocalPlayer);
            return;
        }

        var playerIndex = GetPlayerIndex(PhotonNetwork.LocalPlayer);
        var spawnTransform = spawnTransforms[playerIndex];
        var position = spawnTransform.position;
        var rotation = spawnTransform.rotation;
        
        InstantiatePlayer(position, rotation);
    }

    private void AskToInstantiatePlayer(Player player)
    {
        photonView.RPC(nameof(SendInstantiatePlayer), RpcTarget.MasterClient, (object) player);
    }

    [PunRPC]
    private void SendInstantiatePlayer(object playerObject)
    {
        var player = playerObject as Player;
        if (DebugLogger.IsNullError(player, this)) return;

        var playerIndex = GetPlayerIndex(player);
        photonView.RPC(nameof(RPCInstantiatePlayer), player, playerIndex);
    }

    private int GetPlayerIndex(Player player)
    {
        var playerNumber = punPlayerTargetManager.GetPlayerIndex(player);
        var playerIndex = Mathf.Clamp(playerNumber, 0, spawnTransforms.Length);

        return playerIndex;
    }

    [PunRPC]
    private void RPCInstantiatePlayer(int index)
    {
        var spawnTransform = spawnTransforms[index];
        var position = spawnTransform.position;
        var rotation = spawnTransform.rotation;
        
        InstantiatePlayer(position, rotation);
    }
    
    private void InstantiatePlayer(Vector3 position, Quaternion rotation)
    {
        if (_playerInstance)
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(_playerInstance)} already exists.", this);
            return;
        }

        _playerInstance = PhotonNetwork.Instantiate(networkedPlayerPrefab.name, position, rotation, 0);
        if (DebugLogger.IsNullError(_playerInstance, this)) return;
    }
}