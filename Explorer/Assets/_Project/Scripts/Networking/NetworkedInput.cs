using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkedInput : MonoBehaviourPunCallbacks
{
    public NetworkedPlayerSetup playerSetup;
    public SkillUseHandler skillUseHandler;
    public GrabAction leftGrabAction;
    public GrabAction rightGrabAction;
    public Jump jump;
    public CombatDash combatDash;
    public int actorNumber;
    public string userID;

    private void Awake()
    {
        actorNumber = photonView.Owner.ActorNumber;
        //Debug.Log("Actor Number: " + actorNumber);

        userID = photonView.Owner.UserId;
        //Debug.Log("User ID: " + userID);
    }

    [PunRPC]
    private void NetworkedLeftStartSkillHandler(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            skillUseHandler.AttemptStartUseLeftSkill();
        }
        
    }
    
    public void AttemptStartLeftSkill()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NetworkedLeftStartSkillHandler", RpcTarget.Others, actorNumber);
        }
    }
    
    [PunRPC]
    private void NetworkedLeftEndUseSkillHandler(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            skillUseHandler.AttemptEndUseLeftSkill();
            
        }
    }

    public void AttemptEndUseLeftSkill()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NetworkedLeftEndUseSkillHandler", RpcTarget.Others, actorNumber);
        }
    }
    
    [PunRPC]
    private void NetworkedLeftAttemptGrab(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            leftGrabAction.AttemptGrab();
        }
    }

    public void AttemptLeftGrab()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NetworkedLeftAttemptGrab", RpcTarget.Others, actorNumber);
        }
    }
    
    [PunRPC]
    private void NetworkedLeftAttemptRelease(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            leftGrabAction.AttemptRelease();
        }
    }

    public void AttemptLeftRelease()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NetworkedLeftAttemptRelease", RpcTarget.Others, actorNumber);
        }
    }
    
    [PunRPC]
    private void NetworkedCombatDash(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            combatDash.Dash();
        }
        
    }

    public void Dash()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NetworkedCombatDash", RpcTarget.Others, actorNumber);
        }
    }
    
    [PunRPC]
    private void NetworkedRightStartSkillHandler(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            skillUseHandler.AttemptStartUseRightSkill();
        }
    }

    public void AttemptStartRightSkill()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NetworkedRightStartSkillHandler", RpcTarget.Others, actorNumber);
        }
    }
    
    [PunRPC]
    private void NetworkedRightEndUseSkillHandler(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            skillUseHandler.AttemptEndUseRightSkill();
        }
    }

    public void AttemptEndUseRightSkill()
    {
        if(photonView.IsMine)
        {
            photonView.RPC("NetworkedRightEndUseSkillHandler", RpcTarget.Others, actorNumber);
        }
    }
    
    [PunRPC]
    private void NetworkedRightAttemptGrab(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            rightGrabAction.AttemptGrab();
        }
    }

    public void AttemptRightGrab()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NetworkedRightAttemptGrab", RpcTarget.Others, actorNumber);
        }
    }
    
    [PunRPC]
    private void NetworkedRightAttemptRelease(int actorNumber)
    {
        if (this.actorNumber == actorNumber)
        {
            rightGrabAction.AttemptRelease();
        }
    }

    public void AttemptRightRelease()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("NetworkedRightAttemptRelease", RpcTarget.Others, actorNumber);
        }
    }

    public void UIActivation()
    {
        if (photonView.IsMine)
        {
            if (playerSetup.uiMenu.activeSelf)
            {
                playerSetup.uiMenu.SetActive(false);
                playerSetup.rightRayInteractor.enabled = false;
                playerSetup.leftRayInteractor.enabled = false;
            }
            else
            {
                playerSetup.uiMenu.SetActive(true);
                playerSetup.rightRayInteractor.enabled = true;
                playerSetup.leftRayInteractor.enabled = true;
            }
        }
    }

}
