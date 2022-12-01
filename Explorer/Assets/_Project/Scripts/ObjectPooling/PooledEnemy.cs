using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;

public class PooledEnemy : PooledObject
{
    [SerializeField] private EnemySetup enemySetup;
    [SerializeField] private EnemySetupSOIndex enemySetupSoIndex;
    [SerializeField] private EnemyScriptReferences enemyScriptReferences;
    [SerializeField] private Health health;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private PuppetCollisionHandler puppetCollisionHandler;
    [SerializeField] private PUNPuppetBalanceHandler punPuppetBalanceHandler;
    [SerializeField] private PuppetDeathHandler puppetDeathHandler;
    [SerializeField] private PuppetLauncher puppetLauncher;
    [SerializeField] private EnemyTargetAcquisition enemyTargetAcquisition;
    [SerializeField] private DissolveEffect dissolveEffect;
    [SerializeField] private AccessoryHandler accessoryHandler;
    [SerializeField] private EnemyScaleHandler enemyScaleHandler;
    [SerializeField] private Transform characterControllerTransform;
    [SerializeField] private SimpleAI simpleAi;


    protected override void OnEnable()
    {
        base.OnEnable();
        
        Setup();
    }

    private void Setup()
    {
        if (DebugLogger.IsNullError(enemyTargetAcquisition, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(puppetDeathHandler, this, "Must be set in editor.")) return;

        enemyTargetAcquisition.Setup();
        health.Revive();
        puppetDeathHandler.RevivePuppet();
    }

    protected override void ResetObject()
    {
        base.ResetObject();

        if (DebugLogger.IsNullError(enemySetup, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(healthBar, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(puppetCollisionHandler, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(punPuppetBalanceHandler, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(puppetDeathHandler, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(dissolveEffect, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(characterControllerTransform, this, "Must be set in editor.")) return;

        if (DebugLogger.IsNullInfo(accessoryHandler, this, "Must be set in editor.")) return;

        DebugLogger.IsNullDebug(puppetLauncher, this, "Must be set in editor.");
        
        enemySetup.ResetObject();
        health.ResetObject();
        healthBar.ResetObject();
        puppetCollisionHandler.ResetObject();
        punPuppetBalanceHandler.ResetObject();
        puppetDeathHandler.ResetObject();
        dissolveEffect.ResetObject();
        accessoryHandler.ResetObject();
        
        characterControllerTransform.localPosition = Vector3.zero;
        characterControllerTransform.localRotation = Quaternion.identity;
        
        if (puppetLauncher) puppetLauncher.ResetObject();
    }

    public void SubscribeToWasKilled(Action callback)
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        
        health.WasKilled += callback;
    }
    
    public void EnemySetup(EnemySetupSO enemySetupSo)
    {
        if (DebugLogger.IsNullError(enemySetupSo, this)) return;
        if (DebugLogger.IsNullError(enemySetup, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(enemySetupSoIndex, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(ThisPhotonView, this, "Must be set in editor.")) return;

        enemySetup.Setup(enemySetupSo);
        SetupAccessories();
        SetupScale();
        SubscribeToDissolve();
        SubscribeToPuppetDeathHandler();

        var id = enemySetupSoIndex.GetId(enemySetupSo);
        ThisPhotonView.RPC(nameof(RPCSetup), RpcTarget.OthersBuffered, id);
    }

    public void AttemptActivate()
    {
        Activate();
        SendActivate();
    }

    private void SendActivate()
    {
        if (PhotonNetwork.OfflineMode) return;
        
        if (DebugLogger.IsNullError(ThisPhotonView, this, "Must be set in editor.")) return;

        ThisPhotonView.RPC(nameof(RPCActivate), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPCActivate()
    {
        Activate();
    }

    private void Activate()
    {
        if (DebugLogger.IsNullError(simpleAi, this, "Must be set in editor.")) return;

        simpleAi.EndWait();
    }

    private void AttemptDeactivate()
    {
        Deactivate();
        SendDeactivate();
    }

    private void SendDeactivate()
    {
        if (PhotonNetwork.OfflineMode) return;
        
        if (DebugLogger.IsNullError(ThisPhotonView, this, "Must be set in editor.")) return;

        ThisPhotonView.RPC(nameof(RPCDeactivate), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void RPCDeactivate()
    {
        Deactivate();
    }

    private void Deactivate()
    {
        if (DebugLogger.IsNullError(simpleAi, this, "Must be set in editor.")) return;

        simpleAi.Wait();
    }

    [PunRPC]
    protected void RPCSetup(int enemySetupId)
    {
        if (DebugLogger.IsNullError(enemySetupSoIndex, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(enemySetup, this, "Must be set in editor.")) return;

        var enemySetupSo = enemySetupSoIndex.GetEnemySetupSo(enemySetupId);
        if (DebugLogger.IsNullError(enemySetupSo, this)) return;
        
        SubscribeToPuppetDeathHandler();
        enemySetup.Setup(enemySetupSo);
    }

    public void SetPUNPlayerTargetManager(PUNPlayerTargetManager punPlayerTargetManager)
    {
        if (DebugLogger.IsNullError(enemyScriptReferences, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(ThisPhotonView, this, "Must be set in editor.")) return;

        enemyScriptReferences.SetPunPlayerTargetManager(punPlayerTargetManager);

        ThisPhotonView.RPC(nameof(RPCPUNPlayerTargetManager), RpcTarget.OthersBuffered);
    }
    
    [PunRPC]
    protected void RPCPUNPlayerTargetManager()
    {
        if (DebugLogger.IsNullError(enemyScriptReferences, this, "Must be set in editor.")) return;

        enemyScriptReferences.SetPunPlayerTargetManager(null);
    }
    
    private void SetupAccessories()
    {
        if (DebugLogger.IsNullError(accessoryHandler, this, "Must be set in editor.")) return;

        accessoryHandler.Setup();
    }

    private void SetupScale()
    {
        if (DebugLogger.IsNullError(enemyScaleHandler, this, "Must be set in editor.")) return;

        enemyScaleHandler.Setup();
    }

    private void SubscribeToDissolve()
    {
        if (DebugLogger.IsNullError(dissolveEffect, this, "Must be set in editor.")) return;

        dissolveEffect.FinishedDissolving += Despawn;
    }

    private void SubscribeToPuppetDeathHandler()
    {
        if (DebugLogger.IsNullError(puppetDeathHandler, this, "Must be set in editor.")) return;

        puppetDeathHandler.WasTurnedOff += dissolveEffect.Dissolve;
    }
}
