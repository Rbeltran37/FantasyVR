using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using RootMotion.FinalIK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.XR.XRSettings;

public class NetworkedPlayerSetup : MonoBehaviourPun
{
    #region Preset Variables

    [Header("Preset Variables")] [SerializeField]
    private AppEntitlementCheck _appEntitlementCheck;
    [SerializeField] private NameTag nameTag;
    [SerializeField] private GameObject head;
    [SerializeField] private VRIK vrik;
    [SerializeField] private HolsterPositioner holsterPositioner;
    [SerializeField] private RigidbodyStickLocomotion rbStickLocomotion;
    [SerializeField] private SmoothTurning smoothTurning;
    [SerializeField] private PlayerColliderHandler playerColliderHandler;
    [SerializeField] private Jump jump;
    [SerializeField] private GameObject remoteClientInput;
    [SerializeField] private GameObject remoteClientlocomotion;
    [SerializeField] private GameObject remoteClientJump;
    [SerializeField] private TeleportDodge teleportDodge;
    [SerializeField] private CombatDash combatDash;
    [SerializeField] private SnapTurning snapTurning;

    #endregion

    #region Variables Found on Awake

    [Header("Need to be found on awake")] 
    [SerializeField] private PlayerReferenceManager playerReferenceManager;
    [SerializeField] private Transform headAnchor;
    [SerializeField] private Transform leftAnchor;
    [SerializeField] private Transform rightAnchor;
    [SerializeField] public Transform headTarget;
    [SerializeField] public Transform leftTarget;
    [SerializeField] public Transform rightTarget;
    [SerializeField] public Transform unityXRCameraRig;
    [SerializeField] public GameObject uiMenu;
    [SerializeField] public XRRayInteractor leftRayInteractor;
    [SerializeField] public XRRayInteractor rightRayInteractor;
    [SerializeField] public MenuController menuController;
    [SerializeField] private VRAnimationController vrAnimationController;
    
    [SerializeField] public Save save;

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        if (loadedDeviceName.Equals("Oculus"))
        {
            _appEntitlementCheck.InitializeOculusCheck();
        }
        else
        {
            Debug.Log("Oculus Device is NOT present. Initiate player onboarding.");
            //ENTER USERNAME MANUALLY AND REPORT TO DATABASE.
        }

        //Check if the player is mine/local
        if (photonView.IsMine)
        {
            playerReferenceManager = GameObject.Find("[PlayerReferenceManager]").GetComponent<PlayerReferenceManager>();

            if (playerReferenceManager)
            {
                SetupRig();
                HolsterSetup();
                RBStickLocomotionSetup();
                SnapTurningSetup();
                SmoothTurningSetup();
                TeleportSetup();
                CombatDashSetup();
                HeightAdjustSetup();
                JumpSetup();
                VRIKSetup();
                //UIMenuSetup();
                XRRayInteractorSetup();
                SaveSetup();
                SetupVRLocomotion();
            }
        }
        else
        {
            remoteClientInput.SetActive(false);
            remoteClientlocomotion.SetActive(false);
            remoteClientJump.SetActive(false);
            headTarget = head.transform;
            //holsterPositioner.headSet = head.transform;       //TODO made private
        }

        photonView.Owner.TagObject = gameObject;

        if (photonView)
        {
            photonView.RPC("SyncPlayers", RpcTarget.All, photonView.ViewID);
        }
    }

    [PunRPC]
    public void SyncPlayers(int playerID)
    {
        PhotonView playerView = PhotonView.Find(playerID);
        if (playerView)
        {
            var controller = GameObject.Find("[NetworkController]").GetComponent<NetworkControllerTESTINGONLY>();
            controller.players.Add(playerView.gameObject);
            Debug.Log("added player");
        }
    }

    public void SetNameTag(string name)
    {
        nameTag.SetID(name);
    }

    private void SetupRig()
    {
        unityXRCameraRig = playerReferenceManager.unityXRCameraRig;
        headTarget = playerReferenceManager.headTarget;
        rightTarget = playerReferenceManager.rightTarget;
        leftTarget = playerReferenceManager.leftTarget;
        headAnchor = playerReferenceManager.headAnchor;
        leftAnchor = playerReferenceManager.leftAnchor;
        rightAnchor = playerReferenceManager.rightAnchor;
    }

    private void HolsterSetup()
    {
        //holsterPositioner.headSet = headAnchor;       //TODO made private
    }

    private void RBStickLocomotionSetup()
    {
        //TODO set rbsticklocomotion variables back to private
        /*rbStickLocomotion.playArea = unityXRCameraRig;
        rbStickLocomotion.headset = headAnchor;
        rbStickLocomotion.leftController = leftAnchor;
        rbStickLocomotion.rightController = rightAnchor;*/
    }

    private void SnapTurningSetup()
    {
        snapTurning.headset = headAnchor;
        snapTurning.playArea = unityXRCameraRig;
    }

    private void SmoothTurningSetup()
    {
        //TODO set rbsticklocomotion variables back to private
        /*smoothTurning.playArea = unityXRCameraRig;
        smoothTurning.headset = headAnchor;*/
    }

    private void TeleportSetup()
    {
        teleportDodge._headset = headAnchor;
    }

    private void CombatDashSetup()
    {
        combatDash.playArea = unityXRCameraRig;
        combatDash.headset = headAnchor;
    }

    private void HeightAdjustSetup()
    {
        //heightAdjust.playArea = unityXRCameraRig;
        //heightAdjust.headset = headAnchor;
    }

    private void JumpSetup()
    {
        //jump.playAreaTransform = unityXRCameraRig;
    }

    private void VRIKSetup()
    {
        vrik.solver.spine.headTarget = headTarget;
        vrik.solver.rightArm.target = rightTarget;
        vrik.solver.leftArm.target = leftTarget;
    }

    private void UIMenuSetup()
    {
        uiMenu = playerReferenceManager.uiMenu;
        menuController = playerReferenceManager.menuController;
        menuController.smoothTurning = smoothTurning;
        menuController.snapTurning = snapTurning;
        menuController.rigidbodyStickLocomotion = rbStickLocomotion;
    }

    private void XRRayInteractorSetup()
    {
        leftRayInteractor = playerReferenceManager.leftRayInteractor;
        rightRayInteractor = playerReferenceManager.rightRayInteractor;
    }

    private void SaveSetup()
    {
        save = playerReferenceManager.save;
    }

    private void SetupVRLocomotion()
    {
        //vrAnimationController.xrRig = playerReferenceManager.unityXRCameraRig;        //TODO Kevin: changed to private
        //vrAnimationController.headTarget = playerReferenceManager.headTarget;        //TODO Kevin: changed to private
        //vrAnimationController.walkingHeadTarget = playerReferenceManager.walkingHeadTarget;        //TODO Kevin: changed to private
    }
}
