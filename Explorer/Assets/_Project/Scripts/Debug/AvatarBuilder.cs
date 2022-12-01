using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

public class AvatarBuilder : MonoBehaviour
{
    public static readonly string HEAD_ANCHOR = "HeadAnchor";
    public static readonly string LEFT_ANCHOR = "LeftAnchor";
    public static readonly string RIGHT_ANCHOR = "RightAnchor";
    public static readonly string PLAYER_ROOT = "[Player]";
    public static readonly string HEAD_TARGET = "HeadTarget";
    public static readonly string LEFT_HAND_TARGET = "LeftTarget";
    public static readonly string RIGHT_HAND_TARGET = "RightTarget";
    public static readonly string Target_Place_Holder = "TargetPlaceholder";

    [SerializeField] private Transform _playerManager;
    [SerializeField] private Transform _cameraRigPlayArea;
    [SerializeField] private RuntimeAnimatorController _vrikAnimatedLocomotionController;
    [SerializeField] private GameObject _puppetBehaviorPrefab;
    [SerializeField] private string _characterControllerLayer = "CharacterController";
    [SerializeField] private string _ragdollLayer = "Ragdoll";
    [SerializeField] private string _handLayer = "Fist";
    
    
    private const float LimbAngleAdjustment = 2;
    private const int SpineIndex = 7;
    private const int HeadIndex = 11;
    private const int LeftHandIndex = 10;
    private const int RightHandIndex = 14;
    private PuppetMaster _puppetMaster;
    
    [Button]
    public void AddRagdollAndCollider()
    {
        //add ragdoll creator
        gameObject.AddComponent<BipedRagdollCreator>();
        gameObject.AddComponent<CapsuleCollider>();
    }

    [Button]
    public void CreateRagdoll()
    {
        //add ragdoll creator
        var bipedRagdollCreator = gameObject.GetComponent<BipedRagdollCreator>();
        if (!bipedRagdollCreator)
            return;

        //rotate arms and legs slightly
        var localEulerAngles = bipedRagdollCreator.references.leftLowerArm.localEulerAngles;
        localEulerAngles =
            new Vector3(localEulerAngles.x, localEulerAngles.y + LimbAngleAdjustment, localEulerAngles.z);
        bipedRagdollCreator.references.leftLowerArm.localEulerAngles = localEulerAngles;

        localEulerAngles = bipedRagdollCreator.references.rightLowerArm.localEulerAngles;
        localEulerAngles =
            new Vector3(localEulerAngles.x, localEulerAngles.y - LimbAngleAdjustment, localEulerAngles.z);
        bipedRagdollCreator.references.rightLowerArm.localEulerAngles = localEulerAngles;

        localEulerAngles = bipedRagdollCreator.references.leftLowerLeg.localEulerAngles;
        localEulerAngles =
            new Vector3(localEulerAngles.x + LimbAngleAdjustment, localEulerAngles.y, localEulerAngles.z);
        bipedRagdollCreator.references.leftLowerLeg.localEulerAngles = localEulerAngles;

        localEulerAngles = bipedRagdollCreator.references.rightLowerLeg.localEulerAngles;
        localEulerAngles =
            new Vector3(localEulerAngles.x + LimbAngleAdjustment, localEulerAngles.y, localEulerAngles.z);
        bipedRagdollCreator.references.rightLowerLeg.localEulerAngles = localEulerAngles;

        // Find bones (Humanoids)
        BipedRagdollReferences r = BipedRagdollReferences.FromAvatar(gameObject.GetComponent<Animator>());
        
        // How would you like your ragdoll?
        BipedRagdollCreator.Options options = BipedRagdollCreator.AutodetectOptions(r);

        // Create the ragdoll
        BipedRagdollCreator.Create(r, options);
        Debug.Log("A ragdoll was successfully created.");
    }
    
    [Button]
    public void ConvertRagdollToPuppet()
    {
        _puppetMaster = gameObject.AddComponent<PuppetMaster>();
        
        // This will duplicate the "ragdoll" instance, remove the ragdoll components from the original and use it as the animated target, setting the duplicate up as a puppet.
        _puppetMaster.SetUpTo(transform, LayerMask.NameToLayer(_characterControllerLayer), LayerMask.NameToLayer(_ragdollLayer));
        Debug.Log("A ragdoll was successfully converted to a Puppet.");
    }

    [Button]
    public void Build()
    {
        if (gameObject.GetComponent<BipedRagdollCreator>())
            DestroyImmediate(gameObject.GetComponent<BipedRagdollCreator>());

        //set animator parameters: controller, update mode, culling mode
        if (SetAnimatorParameters(out var animator)) return;
        
        var headTarget = _cameraRigPlayArea.Find(HEAD_ANCHOR);
        if (headTarget)
            headTarget = headTarget.Find(HEAD_TARGET);
        var leftHandTarget = _cameraRigPlayArea.Find(LEFT_ANCHOR);
        if (leftHandTarget)
            leftHandTarget = leftHandTarget.Find(LEFT_HAND_TARGET);
        var rightHandTarget = _cameraRigPlayArea.Find(RIGHT_ANCHOR);
        if (rightHandTarget)
            rightHandTarget = rightHandTarget.Find(RIGHT_HAND_TARGET);

        var vrik = SetupVrik(headTarget, leftHandTarget, rightHandTarget);

        //create puppet
        if (!_puppetMaster)
            _puppetMaster = gameObject.transform.parent.GetComponentInChildren<PuppetMaster>();

        //add behavior prefab
        var playerRoot = _puppetMaster.transform.parent;
        if (!playerRoot || playerRoot.childCount != 3)
            return;
        var behaviorInstance = Instantiate(_puppetBehaviorPrefab, playerRoot.GetChild(0));

        var spineAudio = AddHeadSpineAndHandsAudio(out var headAudio, out var leftHandAudio, out var rightHandAudio);

        //change hand layers
        var leftHand = ChangeHandLayers(leftHandAudio, rightHandAudio, out var rightHand);

        AddHandScripts(leftHand,
            rightHand,
            animator,
            leftHandAudio,
            rightHandAudio,
            leftHandTarget,
            rightHandTarget);

        //set holster hands
        //var holsterMaster = _playerManager.GetComponentInChildren<HolsterEquipHandler>();     //TODO Obsolete, deleted class
        //holsterMaster.leftPuppetHand = leftHand.transform;    //TODO made private, obsolete
        //holsterMaster.rightPuppetHand = rightHand.transform;    //TODO made private, obsolete
        
        //TODO
        //fix powers parameters (firebombCollider and FistHitHandler)
        //re-set firefist hitHandlers

        AddTwistRelaxers(leftHand, rightHand, vrik);

        //add VRIK Animated locomotion
        if (!GetComponent<VRIKAnimatedLocomotion>())
            gameObject.AddComponent<VRIKAnimatedLocomotion>();

        var behaviorPuppet = behaviorInstance.GetComponent<BehaviourPuppet>();

        //check for other player avatar
        var player = GameObject.Find(PLAYER_ROOT);
        if (!player) 
            Debug.Log("No previous player found");
        
        DestroyImmediate(player);

        //rename new Player Root
        playerRoot.name = PLAYER_ROOT;
    }
    

    private static void AddHandScripts(GameObject leftHand, GameObject rightHand, Animator animator,
        AudioSource leftHandAudio, AudioSource rightHandAudio, Transform leftHandTarget, Transform rightHandTarget)
    {
        //add hand scripts: handPhysics, weaponhaptics, weaponhithandler
        //leftControllerHaptics = leftHand.AddComponent<ControllerHaptics>();
        //leftControllerHaptics = null;
        //leftWeaponHitHandler = leftHand.AddComponent<WeaponHitHandler>();

        //rightControllerHaptics = rightHand.AddComponent<ControllerHaptics>();
        //rightControllerHaptics = null;
        //rightWeaponHitHandler = rightHand.AddComponent<WeaponHitHandler>();

        /*leftControllerHaptics.isLeftHand = true;
        rightControllerHaptics.isLeftHand = false;*/

        //set weaponhithandler parameters
        var animatorTransform = animator.transform;
        //leftWeaponHitHandler.userRoot = animatorTransform;
        //leftWeaponHitHandler.weaponAudio = leftHandAudio;
        //rightWeaponHitHandler.userRoot = animatorTransform;
        //rightWeaponHitHandler.weaponAudio = rightHandAudio;
    }

    private GameObject ChangeHandLayers(AudioSource leftHandAudio, AudioSource rightHandAudio, out GameObject rightHand)
    {
        var leftHand = leftHandAudio.transform.gameObject;
        rightHand = rightHandAudio.transform.gameObject;
        leftHand.layer = LayerMask.NameToLayer(_handLayer);
        rightHand.layer = LayerMask.NameToLayer(_handLayer);
        return leftHand;
    }

    private AudioSource AddHeadSpineAndHandsAudio(out AudioSource headAudio, out AudioSource leftHandAudio,
        out AudioSource rightHandAudio)
    {
        //add audiosource to spine
        var spineAudio = _puppetMaster.muscles[SpineIndex].joint.gameObject.AddComponent<AudioSource>();
        //add audiosource to head
        headAudio = _puppetMaster.muscles[HeadIndex].joint.gameObject.AddComponent<AudioSource>();
        //add audiosource to hands
        leftHandAudio = _puppetMaster.muscles[LeftHandIndex].joint.gameObject.AddComponent<AudioSource>();
        rightHandAudio = _puppetMaster.muscles[RightHandIndex].joint.gameObject.AddComponent<AudioSource>();
        return spineAudio;
    }

    private VRIK SetupVrik(Transform headTarget, Transform leftHandTarget, Transform rightHandTarget)
    {
        var vrik = gameObject.AddComponent<VRIK>();
        vrik.solver.spine.headTarget = headTarget;
        vrik.solver.leftArm.target = leftHandTarget;
        vrik.solver.rightArm.target = rightHandTarget;
        vrik.solver.plantFeet = false;
        return vrik;
    }

    private bool SetAnimatorParameters(out Animator animator)
    {
        animator = gameObject.GetComponent<Animator>();
        if (!animator) return true;
        if (_vrikAnimatedLocomotionController)
            animator.runtimeAnimatorController = _vrikAnimatedLocomotionController;

        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        return false;
    }

    private void AddTwistRelaxers(GameObject leftHand, GameObject rightHand, VRIK vrik)
    {
        //add twist relaxers to hands and forearms
        var leftForearm = leftHand.transform.parent;
        var leftForearmTwistRelaxer = leftForearm.GetComponent<TwistRelaxer>();
        if (!leftForearmTwistRelaxer)
            leftForearmTwistRelaxer = leftForearm.gameObject.AddComponent<TwistRelaxer>();
        leftForearmTwistRelaxer.child = leftHand.transform;
        leftForearmTwistRelaxer.parent = leftForearm.parent;

        var rightForearm = rightHand.transform.parent;
        var rightTwistRelaxer = rightForearm.GetComponent<TwistRelaxer>();
        if (!rightTwistRelaxer)
            rightTwistRelaxer = rightForearm.gameObject.AddComponent<TwistRelaxer>();
        rightTwistRelaxer.child = rightHand.transform;
        rightTwistRelaxer.parent = rightForearm.parent;
        rightTwistRelaxer.ik = vrik;
    }
}
