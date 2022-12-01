using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class AiNavigation : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform navTargetTransform;
    [SerializeField] private Transform cornerTransform;
    [SerializeField] private Transform nextPositionTransform;
    [SerializeField] private Transform characterControllerTransform;
    [SerializeField] private float warpBuffer = .2f;

    private bool _reset;


    private void OnEnable()
    {
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;
    }

    private void Update()
    {
        SetDestination();
        GetCorner();
    }

    public void SetNavTarget(Transform target)
    {
        if (DebugLogger.IsNullError(target, this)) return;
        
        navTargetTransform = target;
    }
    
    public Transform GetCornerTransform()
    {
        return cornerTransform;
    }

    private void SetDestination()
    {
        if (DebugLogger.IsNullError(navMeshAgent, this, "Should be set in the editor.")) return;
        if (DebugLogger.IsNullError(navTargetTransform, this, "Should be set in the editor.")) return;
        if (DebugLogger.IsNullError(characterControllerTransform, this, "Should be set in the editor.")) return;
        
        if (Vector3.Distance(navMeshAgent.nextPosition, characterControllerTransform.position) > warpBuffer)
        {
            navMeshAgent.Warp(characterControllerTransform.position);
        }

        navMeshAgent.nextPosition = characterControllerTransform.position;

        if (navMeshAgent.isOnNavMesh) navMeshAgent.SetDestination(navTargetTransform.position);
    }

    private void GetCorner()
    {
        if (DebugLogger.IsNullError(navMeshAgent, this, "Should be set in the editor.")) return;
        if (DebugLogger.IsNullError(cornerTransform, this, "Should be set in the editor.")) return;
        if (DebugLogger.IsNullError(nextPositionTransform, this, "Should be set in the editor.")) return;

        var results = new Vector3[3];
        navMeshAgent.path.GetCornersNonAlloc(results);
        if (results[1] != Vector3.zero)
        {
            cornerTransform.position = results[1];
        }
        
        nextPositionTransform.position = navMeshAgent.nextPosition;
    }
}
