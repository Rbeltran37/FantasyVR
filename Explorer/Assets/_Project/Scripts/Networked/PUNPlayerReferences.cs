using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using UnityEngine;

public class PUNPlayerReferences : MonoBehaviour
{
    public VRIK vrik;
    public PuppetMaster puppetMaster;
    public HandAnimationHandler handAnimationHandler;
    public Transform playerController;
    public Transform leftEmitter;
    public Transform rightEmitter;
    public Transform leftPullTarget;
    public Transform rightPullTarget;
    public Transform leftPuppetHand;
    public Transform rightPuppetHand;
    public Rigidbody leftPuppetHandRigid;
    public Rigidbody rightPuppetHandRigid;
}
