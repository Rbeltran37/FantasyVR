using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using RootMotion.Demos;
using RootMotion.FinalIK;
using UnityEngine;

public class EnemyScriptReferences : MonoBehaviour
{
    [SerializeField] private SimpleAI simpleAi;
    [SerializeField] private CustomUserControlAI customUserControlAi;
    [SerializeField] private BehaviorTree behaviorTree;
    [SerializeField] private AiNavigation aiNavigation;
    [SerializeField] private EnemyTargetAcquisition enemyTargetAcquisition;
    
    [SerializeField] public Transform modelHips;
    [SerializeField] public Transform modelHead;
    
    private const string NAVIGATION_TARGET = "NavTarget";


    public Vector3 GetModelHipsPosition()
    {
        if (DebugLogger.IsNullError(modelHips, this, "Must be set in editor.")) return Vector3.zero;

        return modelHips.position;
    }

    public void SetPlayerTarget(PlayerTarget playerTarget)
    {
        if (DebugLogger.IsNullError(playerTarget, this)) return;
        if (DebugLogger.IsNullError(aiNavigation, this, "Must be set in editor.")) return;

        SetBehaviorTreeTarget(playerTarget.GetPlayerController());
        aiNavigation.SetNavTarget(playerTarget.GetPlayerController().transform);
        SetUserControlAiNavTarget(aiNavigation.GetCornerTransform());
        SetUserControlAiAimTarget(playerTarget.GetModelHips());
        SetSimpleAiPlayerTarget(playerTarget.GetPuppetHead());
    }

    public PlayerTarget GetCurrentTarget()
    {
        if (DebugLogger.IsNullError(enemyTargetAcquisition, this, "Must be set in editor.")) return null;

        return enemyTargetAcquisition.GetCurrentTarget();
    }

    public void SetSimpleAiPlayerTarget(Transform target)
    {
        if (DebugLogger.IsNullError(simpleAi, this, "Must be set in editor.")) return;

        simpleAi.SetPlayerTarget(target);
    }

    private void SetUserControlAiNavTarget(Transform cornerTransform)
    {
        if (DebugLogger.IsNullError(customUserControlAi, this, "Must be set in editor.")) return;

        customUserControlAi.navTarget = cornerTransform;
    }
    
    private void SetUserControlAiAimTarget(Transform playerModelHipsTransform)
    {
        if (DebugLogger.IsNullError(customUserControlAi, this, "Must be set in editor.")) return;

        customUserControlAi.aimTarget = playerModelHipsTransform;
    }

    private void SetBehaviorTreeTarget(GameObject playerControllerGameObject)
    {
        if (DebugLogger.IsNullError(behaviorTree, this, "Must be set in editor.")) return;

        behaviorTree.SetVariable(NAVIGATION_TARGET, (SharedGameObject)playerControllerGameObject);
    }

    public void SetPunPlayerTargetManager(PUNPlayerTargetManager punPlayerTargetManager)
    {
        if (DebugLogger.IsNullError(enemyTargetAcquisition, this, "Must be set in editor.")) return;

        enemyTargetAcquisition.SetPunPlayerTargetManager(punPlayerTargetManager);
    }
}
