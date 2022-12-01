using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class PUNPlayerTargetManager : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private PUNPlayerManager punPlayerManager;

    private HashSet<PlayerTarget> _playerTargets = new HashSet<PlayerTarget>();
    private Dictionary<Player, int> _playerIndexes = new Dictionary<Player, int>();
    private int _currentPlayerIndex;

    public Action<PlayerTarget> WasAdded;
    public Action<PlayerTarget> WasRemoved;


    private void Awake()
    {
        if (DebugLogger.IsNullWarning(punPlayerManager, this, "Should be set in editor. Attempting to find..."))
        {
            punPlayerManager = FindObjectOfType<PUNPlayerManager>();
            if (DebugLogger.IsNullError(punPlayerManager, this, "Should be set in editor. Unable to find.")) return;
        }
    }

    public List<PlayerTarget> GetPlayerTargets()
    {
        return _playerTargets.ToList();
    }

    public int GetNumPlayers()
    {
        return _playerTargets.Count;
    }
    
    public void AddPlayer(GameObject playerInstance)
    {
        if (DebugLogger.IsNullError(playerInstance, this)) return;

        var playerPhotonView = playerInstance.GetComponent<PhotonView>();
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        SetPlayerTagObject(playerPhotonView);
        
        photonView.RPC(nameof(RPCAddPlayer), RpcTarget.AllBuffered, playerPhotonView.ViewID);
    }

    private void SetPlayerTagObject(PhotonView playerPhotonView)
    {
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        var playerTarget = playerPhotonView.GetComponent<PlayerTarget>();
        if (DebugLogger.IsNullError(playerTarget, this)) return;

        var player = playerPhotonView.Owner;
        player.TagObject = playerTarget;
    }

    [PunRPC]
    private void RPCAddPlayer(int photonViewId)
    {
        var playerPhotonView = PhotonNetwork.GetPhotonView(photonViewId);
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        SetPlayerTagObject(playerPhotonView);
        AttemptAddPlayerIndex(playerPhotonView.Owner);
        
        var player = playerPhotonView.Owner;
        var playerTarget = (PlayerTarget)player.TagObject;
        AddPlayerTarget(playerTarget);
    }

    private void AttemptAddPlayerIndex(Player player)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_playerIndexes.ContainsKey(player)) return;

        AddPlayerIndex(player, _currentPlayerIndex);
        SendAddPlayerIndex(player, _currentPlayerIndex);
        _currentPlayerIndex++;
    }

    private void AddPlayerIndex(Player player, int index)
    {
        if (_playerIndexes.ContainsKey(player)) return;
        
        _playerIndexes.Add(player, index);
    }

    private void SendAddPlayerIndex(Player player, int index)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        photonView.RPC(nameof(RPCAddPlayerIndex), RpcTarget.OthersBuffered, player, index);
    }

    [PunRPC]
    private void RPCAddPlayerIndex(Player player, int index)
    {
        AddPlayerIndex(player, index);
    }
    
    private void AddPlayerTarget(PlayerTarget playerTargetToAdd)
    {
        if (DebugLogger.IsNullError(playerTargetToAdd, this)) return;

        foreach (var playerTarget in _playerTargets)
        {
            if (DebugLogger.IsNullWarning(playerTargetToAdd, this)) return;

            if (playerTarget.Equals(playerTargetToAdd))
            {
                DebugLogger.Warning(MethodBase.GetCurrentMethod().Name, $"{playerTargetToAdd} is already in playerTargets.", this);
                return;
            }
        }

        _playerTargets.Add(playerTargetToAdd);
        
        WasAdded?.Invoke(playerTargetToAdd);
    }
    
    public void RemovePlayer(PlayerTarget playerTarget)
    {
        if (DebugLogger.IsNullError(playerTarget, this)) return;

        var photonViewId = playerTarget.GetPhotonViewId();
        
        photonView.RPC(nameof(RPCRemovePlayer), RpcTarget.AllBuffered, photonViewId);
    }

    [PunRPC]
    private void RPCRemovePlayer(int photonViewId)
    {
        var playerPhotonView = PhotonNetwork.GetPhotonView(photonViewId);
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        var player = playerPhotonView.Owner;
        var playerTarget = (PlayerTarget)player.TagObject;
        RemovePlayerTarget(playerTarget);
    }
    
    private void RemovePlayerTarget(PlayerTarget playerTargetToRemove)
    {
        _playerTargets.Remove(playerTargetToRemove);
        WasRemoved?.Invoke(playerTargetToRemove);
    }
    
    public PlayerTarget GetRandomPlayerTarget()
    {
        if (DebugLogger.IsNullOrEmptyWarning(_playerTargets, this)) return null;
        
        var randomIndex = Random.Range(0, _playerTargets.Count);
        return _playerTargets.ToList()[randomIndex];
    }

    public int GetPlayerIndex(Player player)
    {
        if (!_playerIndexes.ContainsKey(player)) return _currentPlayerIndex;
        
        return _playerIndexes[player];
    }
}
