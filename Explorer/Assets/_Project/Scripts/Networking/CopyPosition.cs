using System;
using System.Collections.Generic;
using CurvedUI;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.XR;
using VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input;
using Zinnia.Extension;

[RequireComponent(typeof(PhotonView))]
public class CopyPosition : MonoBehaviourPun
{
    [Header("Preset Variables")]
    [SerializeField] private GameObject head;
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform leftTarget;
    [SerializeField] private Transform rightTarget;
    [SerializeField] private NetworkedPlayerSetup playerSetup;

    private void Start()
    {
        headTarget = playerSetup.headTarget;
        leftTarget = playerSetup.leftTarget;
        rightTarget = playerSetup.rightTarget;
    }
    
    private void Update()
    {
        //Check if the player is mine/local
        if (photonView.IsMine)
        {
            //Set NETWORKED head position and rotation to the LOCAL position and rotation 
            head.transform.position = headTarget.position;
            head.transform.rotation = headTarget.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            //Set NETWORKED hand positions and rotations to the LOCAL positions and rotations 
            leftHand.transform.position = leftTarget.position;
            leftHand.transform.rotation = leftTarget.rotation;

            rightHand.transform.position = rightTarget.position;
            rightHand.transform.rotation = rightTarget.rotation;
        }
    }
}