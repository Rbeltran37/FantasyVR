using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using UnityEngine;

public class PUNClientSetup : MonoBehaviour
{
    [SerializeField] private PhotonView thisPhotonView;
    [SerializeField] private Transform playerParent;
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private VRIK vrik;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform leftTarget;
    [SerializeField] private Transform rightTarget;
    [SerializeField] private GrabAction leftGrabAction;
    [SerializeField] private GrabAction rightGrabAction;


    private void Awake()
    {
        if (DebugLogger.IsNullError(thisPhotonView, "Must be set in editor.", this)) return;
        
        if (thisPhotonView && !thisPhotonView.IsMine)
        {
            SetupVrik();
            SetupGrabAction();
            puppetMaster.mode = PuppetMaster.Mode.Kinematic;
            
            if (PhotonNetwork.IsMasterClient)
            {
                var punEnemyManager = FindObjectOfType<PUNEnemyManager>();
                if (punEnemyManager) punEnemyManager.RespawnAllEnemies();
            }

            Destroy(gameObject);
        }
    }

    private void SetupVrik()
    {
        if (DebugLogger.IsNullError(vrik, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(headTarget, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(leftTarget, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(rightTarget, this, "Must be set in editor.")) return;

        vrik.solver.spine.headTarget = headTarget;
        vrik.solver.leftArm.target = leftTarget;
        vrik.solver.rightArm.target = rightTarget;

        headTarget.parent = playerParent;
        leftTarget.parent = playerParent;
        rightTarget.parent = playerParent;
    }

    private void SetupGrabAction()
    {
        if (DebugLogger.IsNullError(leftGrabAction, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(rightGrabAction, this, "Must be set in editor.")) return;

        Destroy(leftGrabAction);
        Destroy(rightGrabAction);
    }
}
