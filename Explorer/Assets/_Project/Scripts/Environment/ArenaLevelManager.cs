using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class ArenaLevelManager : MonoBehaviour
{
    [SerializeField] private PhotonView thisPhotonView;
    [SerializeField] private LocalNavMeshBuilder localNavMeshBuilder;
    [SerializeField] private List<Round> rounds = new List<Round>();
    [SerializeField] private ArenaLevel thisLevel;
    [SerializeField] private ArenaLevel nextLevel;

    private int _numRounds;
    private int _currentRound;
    private int _numEvents;
    private int _currentEvent;
    private ArenaEvent _currentArenaEvent;

    [SerializableAttribute] internal class Round
    {
        public ArenaRound ArenaRound;
        public List<ArenaEvent> ArenaEvents;

        internal Round(ArenaRound arenaRound)
        {
            ArenaRound = arenaRound;
        }
        
        [Button]
        internal void PopulateParameters()
        {
            if (DebugLogger.IsNullError(ArenaRound, "Must be set in editor.")) return;

            ArenaEvents = new List<ArenaEvent>();
            var arenaRoundTransform = ArenaRound.transform;
            var childCount = arenaRoundTransform.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = arenaRoundTransform.GetChild(i);
                var arenaEvent = child.GetComponent<ArenaEvent>();
                if (arenaEvent) ArenaEvents.Add(arenaEvent);
            }
        }

        internal void BeginRound()
        {
            if (DebugLogger.IsNullError(ArenaRound, "Must be set in editor.")) return;
            
            ArenaRound.BeginRound();
        }
    }

    private void Awake()
    {
        if (DebugLogger.IsNullOrEmptyError(rounds, "Must be set in editor.", this)) return;
        
        _numRounds = rounds.Count;
        _numEvents = rounds[0].ArenaEvents.Count;
    }

    private void Start()
    {
        CoroutineCaller.WaitToConnect(Initialize);
        UpdateNavMesh();
    }

    private void OnEnable()
    {
        CoroutineCaller.WaitToConnect(SendEnableLevel);
    }

    private void OnDisable()
    {
        SendDisableLevel();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    [Button]
    public void PopulateParameters()
    {
        if (!thisPhotonView)
        {
            thisPhotonView = GetComponent<PhotonView>();
            if (!thisPhotonView) thisPhotonView = gameObject.AddComponent<PhotonView>();
        }
        
        rounds = new List<Round>();
        var arenaRounds = GetComponentsInChildren<ArenaRound>();
        foreach (var arenaRound in arenaRounds)
        {
            var round = new Round(arenaRound);
            round.PopulateParameters();
            rounds.Add(round);
        }
    }
    
    private void EnableLevelLocal()
    {
        gameObject.SetActive(true);
    }

    private void SendEnableLevel()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        thisPhotonView.RPC(nameof(RPCEnableLevel), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPCEnableLevel()
    {
        EnableLevelLocal();
    }

    private void DisableLevelLocal()
    {
        gameObject.SetActive(false);
    }

    private void SendDisableLevel()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        thisPhotonView.RPC(nameof(RPCDisableLevel), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPCDisableLevel()
    {
        DisableLevelLocal();
    }
    
    private void Initialize()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        AttemptBeginNextRound();
    }
    
    private void UpdateNavMesh()
    {
        if (DebugLogger.IsNullError(localNavMeshBuilder, this, "Must be set in editor.")) return;

        localNavMeshBuilder.UpdateNavMesh(true);
    }

    private void AttemptBeginNextRound()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (_currentRound > 0) UnsubscribeFromEvents();
        if (_currentRound >= _numRounds)
        {
            SendBeginNextLevel();
            BeginNextLevel();
            return;
        }
        
        SubscribeToEvents();
        SendBeginNextRound();
        BeginNextRound();
        SendBeginNextEvent();
        BeginNextEvent();
    }
    
    private void SubscribeToEvents()
    {
        var round = rounds[_currentRound];
        var arenaRound = round.ArenaRound;
        if (DebugLogger.IsNullError(arenaRound, this)) return;
            
        arenaRound.RoundHasEnded += AttemptBeginNextRound;

        var arenaEvents = round.ArenaEvents;
        foreach (var arenaEvent in arenaEvents)
        {
            arenaEvent.HasEnded += AttemptBeginNextEvent;
        }
    }

    private void SendBeginNextRound()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        thisPhotonView.RPC(nameof(RPCBeginNextRound), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPCBeginNextRound()
    {
        BeginNextRound();
    }

    private void BeginNextRound()
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(_currentRound)}={_currentRound} {nameof(_numRounds)}={_numRounds}", this);

        _currentEvent = 0;
        _numEvents = rounds[_currentRound].ArenaEvents.Count;

        if (_currentRound >= _numRounds)
        {
            BeginNextLevel();
            return;
        }
        
        rounds[_currentRound].BeginRound();
    }

    private void BeginNextLevel()
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, "Called", this);
        if (!nextLevel) return;
        
        nextLevel.StartLevel();
        thisLevel.EndLevel();
    }

    private void SendBeginNextLevel()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        if (!nextLevel) return;
        
        thisPhotonView.RPC(nameof(RPCBeginNextLevel), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPCBeginNextLevel()
    {
        BeginNextLevel();
    }

    private void IterateRound()
    {
        _currentRound++;
    }

    private void UnsubscribeFromEvents()
    {
        var round = rounds[_currentRound - 1];
        var arenaRound = round.ArenaRound;
        if (DebugLogger.IsNullError(arenaRound, this)) return;
            
        arenaRound.RoundHasEnded -= AttemptBeginNextRound;

        var arenaEvents = round.ArenaEvents;
        foreach (var arenaEvent in arenaEvents)
        {
            arenaEvent.HasEnded -= AttemptBeginNextEvent;
        }
    }

    private void AttemptBeginNextEvent()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        SendBeginNextEvent();
        BeginNextEvent();
    }

    private void SendBeginNextEvent()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        thisPhotonView.RPC(nameof(RPCBeginNextEvent), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPCBeginNextEvent()
    {
        BeginNextEvent();
    }
    
    private void BeginNextEvent()
    {
        if (_currentEvent >= _numEvents)
        {
            IterateRound();
            AttemptBeginNextRound();
            return;
        }
        
        if (_currentArenaEvent) DisableLastEvent();

        _currentArenaEvent = rounds[_currentRound].ArenaEvents[_currentEvent];
        _currentArenaEvent.Begin();
        _currentEvent++;
    }

    private void DisableLastEvent()
    {
        _currentArenaEvent.Disable();
    }
}
