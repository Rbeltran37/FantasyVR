using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

public class EnemyTargetAcquisition : MonoBehaviour
{
    [SerializeField] private PhotonView prefabPhotonView;
    [SerializeField] private Health health;
    [SerializeField] private EnemyScriptReferences enemyScriptReferences;
    [SerializeField] private PUNPuppetCollisionHandler punPuppetCollisionHandler;
    [SerializeField] private Transform aimReference;
    [SerializeField] private float reassessTargetInterval = 5;
    [SerializeField] private float aheadMultiplier = 1;
    [SerializeField] private float distanceMultiplier = .5f;
    [SerializeField] private float damageMultiplier = .1f;
    [SerializeField] private float hitEnemyLastMultiplier = 2;
    
    private PUNPlayerTargetManager _punPlayerTargetManager;
    private PlayerTarget _currentPlayerTarget;
    private PlayerTarget _targetHitByLast;
    private Dictionary<PlayerTarget, ThreatLevel> _playerThreats = new Dictionary<PlayerTarget, ThreatLevel>();
    private Dictionary<int, PlayerTarget> _playerTargets = new Dictionary<int, PlayerTarget>();


    private class ThreatLevel
    {
        public float distance;
        public bool isAhead;
        public float recentDamageDealt;
        
        private float _aheadMultiplier;
        private float _distanceMultiplier;
        private float _damageMultiplier;
        private float _hitEnemyLastMultiplier;

        public ThreatLevel(EnemyTargetAcquisition enemyTargetAcquisition)
        {
            recentDamageDealt = enemyTargetAcquisition.damageMultiplier;
            _aheadMultiplier = enemyTargetAcquisition.aheadMultiplier;
            _distanceMultiplier = enemyTargetAcquisition.distanceMultiplier;
            _damageMultiplier = enemyTargetAcquisition.damageMultiplier;
            _hitEnemyLastMultiplier = enemyTargetAcquisition.hitEnemyLastMultiplier;
        }

        public float CalculateThreatLevel(bool didTargetHitLast)
        {
            var threatLevel = 0f;
            if (isAhead) threatLevel += _aheadMultiplier;
            if (didTargetHitLast) threatLevel += _hitEnemyLastMultiplier;
            threatLevel -= distance * _distanceMultiplier;
            threatLevel += recentDamageDealt * _damageMultiplier;
            return threatLevel;
        }

        public void ResetRecentDamage()
        {
            recentDamageDealt = 0;
        }
    }
    
    
    private void Awake()
    {
        if (DebugLogger.IsNullInfo(punPuppetCollisionHandler, this, "Must be set in editor.")) return;

        punPuppetCollisionHandler.WasHit += RegisterHit;
    }

    private void Start()
    {
        _punPlayerTargetManager.WasAdded += AddPlayer;
        _punPlayerTargetManager.WasRemoved += RemovePlayer;
        
        if (!_punPlayerTargetManager)
        {
            SetPunPlayerTargetManager(null);
            if (DebugLogger.IsNullError(_punPlayerTargetManager, this)) return;
        }
    }

    private void OnDestroy()
    {
        if (punPuppetCollisionHandler)
        {
            punPuppetCollisionHandler.WasHit -= RegisterHit;
        }

        if (_punPlayerTargetManager)
        {
            _punPlayerTargetManager.WasAdded -= AddPlayer;
            _punPlayerTargetManager.WasRemoved -= RemovePlayer;
        }
    }
    
    public void Setup()
    {
        if (!_punPlayerTargetManager)
        {
            SetPunPlayerTargetManager(null);
            if (DebugLogger.IsNullError(_punPlayerTargetManager, this)) return;
        }
        
        InitializeThreats();        //TODO optimize, might only need to be called once
        SetPlayerTarget(GetCurrentTarget());
        
        CoroutineCaller.Instance.StartCoroutine(AcquireTargetCoroutine());
    }
    
    public void SetPunPlayerTargetManager(PUNPlayerTargetManager punPlayerTargetManager)
    {
        if (_punPlayerTargetManager) return;

        if (!punPlayerTargetManager) punPlayerTargetManager = FindObjectOfType<PUNPlayerTargetManager>();
        
        if (DebugLogger.IsNullError(punPlayerTargetManager, this)) return;
        
        _punPlayerTargetManager = punPlayerTargetManager;
    }
    
    private void InitializeThreats()
    {
        if (!_punPlayerTargetManager)
        {
            SetPunPlayerTargetManager(null);
            if (DebugLogger.IsNullError(_punPlayerTargetManager, this)) return;
        }

        _playerTargets = new Dictionary<int, PlayerTarget>();
        _playerThreats = new Dictionary<PlayerTarget, ThreatLevel>();
        foreach (var playerTarget in _punPlayerTargetManager.GetPlayerTargets())
        {
            AddPlayer(playerTarget);
        }
    }
    
    public PlayerTarget GetCurrentTarget()
    {
        if (!_currentPlayerTarget)
        {
            if (!_punPlayerTargetManager)
            {
                SetPunPlayerTargetManager(null);
                if (DebugLogger.IsNullError(_punPlayerTargetManager, this)) return null;
            }
            
            var randomPlayerTarget = _punPlayerTargetManager.GetRandomPlayerTarget();
            SetPlayerTarget(randomPlayerTarget);
        }
        
        return _currentPlayerTarget;
    }
    
    private void SetPlayerTarget(PlayerTarget playerTarget)
    {
        if (DebugLogger.IsNullWarning(playerTarget, this)) return;
        if (DebugLogger.IsNullError(prefabPhotonView, this, "Must be set in editor.")) return;

        prefabPhotonView.RPC(nameof(RPCSetPlayerTarget), RpcTarget.AllBuffered, playerTarget.GetPhotonViewId());
    }
    
    [PunRPC]
    private void RPCSetPlayerTarget(int photonViewId)
    {
        var playerPhotonView = PhotonNetwork.GetPhotonView(photonViewId);
        if (DebugLogger.IsNullError(playerPhotonView, this)) return;

        var ownerActorNumber = playerPhotonView.OwnerActorNr;
        if (ownerActorNumber == 0)
        {
            playerPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
        
        SetPlayerTarget(playerPhotonView.OwnerActorNr);
    }
    
    private void SetPlayerTarget(int actorNumber)
    {
        if (DebugLogger.IsNullError(enemyScriptReferences, this, "Must be set in editor.")) return;

        if (!_punPlayerTargetManager)
        {
            SetPunPlayerTargetManager(null);
            if (DebugLogger.IsNullError(_punPlayerTargetManager, this)) return;
        }

        if (!_playerTargets.ContainsKey(actorNumber))
        {
            InitializeThreats();
            if (!_playerTargets.ContainsKey(actorNumber))
            {
                DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(_playerTargets)} does not contain key {nameof(actorNumber)}={actorNumber}.", this);
                return;
            }
        }
        
        var playerTarget = _playerTargets[actorNumber];
        if (DebugLogger.IsNullError(playerTarget, this)) return;

        _currentPlayerTarget = playerTarget;
        enemyScriptReferences.SetPlayerTarget(_currentPlayerTarget);
    }

    private IEnumerator AcquireTargetCoroutine()
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) yield break;
        
        while (health.isAlive)
        {
            AcquireTarget();
            yield return new WaitForSeconds(reassessTargetInterval);
        }
    }
    
    private void AcquireTarget()
    {
        SetTargetDistances();
        SetIsAhead();
        
        if (DebugLogger.IsNullOrEmptyWarning(_playerTargets, this)) return;
        
        PlayerTarget newTarget = null;
        var currentThreat = Single.MinValue;
        foreach (var playerThreat in _playerThreats)
        {
            var threat = playerThreat.Value.CalculateThreatLevel(playerThreat.Key.Equals(_targetHitByLast));
            if (threat > currentThreat)
            {
                currentThreat = threat;
                newTarget = playerThreat.Key;
            }
        }
        
        if (newTarget != null && !newTarget.Equals(_currentPlayerTarget))
        {
            SetPlayerTarget(newTarget);
        }
    }

    private void AddPlayer(PlayerTarget playerTarget)
    {
        if (DebugLogger.IsNullError(playerTarget, this)) return;
        
        if (_playerThreats == null) _playerThreats = new Dictionary<PlayerTarget, ThreatLevel>();

        if (_playerThreats.ContainsKey(playerTarget))
        {
            DebugLogger.Warning(MethodBase.GetCurrentMethod().Name, $"{nameof(_playerThreats)} already contains {nameof(playerTarget)}={playerTarget}", this);
            return;
        }
        _playerThreats.Add(playerTarget, new ThreatLevel(this));
        _playerTargets.Add(playerTarget.GetActorNumber(), playerTarget);
    }

    private void RemovePlayer(PlayerTarget playerTarget)
    {
        if (DebugLogger.IsNullError(playerTarget, this)) return;
        
        if (_playerThreats == null) _playerThreats = new Dictionary<PlayerTarget, ThreatLevel>();
        
        if (!_playerThreats.ContainsKey(playerTarget))
        {
            DebugLogger.Warning(nameof(RemovePlayer), $"{nameof(_playerThreats)} does not contain {nameof(playerTarget)}={playerTarget}", this);
            return;
        }
        _playerThreats.Remove(playerTarget);
        _playerTargets.Remove(playerTarget.GetActorNumber());
    }

    private void SetTargetDistances()
    {
        if (!_punPlayerTargetManager)
        {
            SetPunPlayerTargetManager(null);
            if (!_punPlayerTargetManager)
            {
                DebugLogger.Error(nameof(SetTargetDistances), $"{nameof(_punPlayerTargetManager)} is null.", this);
                return;
            }
        }
    
        foreach (var playerTarget in _playerThreats.Keys)
        {
            if (playerTarget == null)
            {
                DebugLogger.Warning(nameof(SetTargetDistances), $"{nameof(playerTarget)} is null.", this);
                continue;
            }

            _playerThreats[playerTarget].distance =
                Vector3.Distance(playerTarget.GetModelHipsPosition(),
                    enemyScriptReferences.GetModelHipsPosition());
        }
    }

    private void SetIsAhead()
    {
        if (!_punPlayerTargetManager)
        {
            SetPunPlayerTargetManager(null);
            if (!_punPlayerTargetManager)
            {
                DebugLogger.Error(nameof(SetIsAhead), $"{nameof(_punPlayerTargetManager)} is null.", this);
                return;
            }
        }
        
        foreach (var playerTarget in _playerThreats.Keys)
        {
            if (playerTarget == null)
            {
                DebugLogger.Warning(nameof(SetIsAhead), $"{nameof(playerTarget)} is null.", this);
                continue;
            }
            
            _playerThreats[playerTarget].isAhead = Math.Abs(VectorMath.IsTargetAhead(enemyScriptReferences.modelHead.forward,
                playerTarget.GetModelHips()) - 1) < Mathf.Epsilon;
        }
    }

    private void RegisterHit(PlayerTarget playerTarget, float damage)
    {
        if (!playerTarget)
        {
            DebugLogger.Error(nameof(RegisterHit), $"{nameof(playerTarget)} is null.", this);
            return;
        }
        
        if (_playerThreats == null || _playerThreats.Count == 0)
        {
            DebugLogger.Error(nameof(RegisterHit), $"{nameof(_playerThreats)} is null or empty.", this);
            return;
        }
        
        _targetHitByLast = playerTarget;

        if (!_playerThreats.ContainsKey(playerTarget))
        {
            DebugLogger.Error(nameof(RegisterHit), $"{nameof(playerTarget)}={playerTarget} is not in the {nameof(_playerThreats)} dictionary.", this);
            return;
        }
        _playerThreats[playerTarget].recentDamageDealt += damage;
    }

    public void AimAtNearestPlayerTransform()
    {
        enemyScriptReferences.SetSimpleAiPlayerTarget(_currentPlayerTarget.GetNearestPlayerTarget(aimReference));
    }

    public void ResetPlayerTarget()
    {
        enemyScriptReferences.SetSimpleAiPlayerTarget(_currentPlayerTarget.GetPuppetHead());
    }

    private void ResetRecentDamage()
    {
        foreach (var threatLevel in _playerThreats.Values)
        {
            threatLevel.ResetRecentDamage();
        }
    }
}
