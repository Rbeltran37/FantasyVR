using System;
using System.Collections;
using System.Reflection;
using Photon.Pun;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class VRAnimationController : MonoBehaviour
{
    [SerializeField] private PhotonView thisPhotonView;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Transform playArea;
    [SerializeField] private Transform headTarget;
    [SerializeField] private Transform walkingHeadTarget;    
    [SerializeField] private Rigidbody playAreaRigidbody;
    [SerializeField] private Animator animator;
    [SerializeField] private VRIK vrik;
    [SerializeField] private RigidbodyStickLocomotion rigidbodyStickLocomotion;
    [SerializeField] private Jump jump;
    [SerializeField] private PlayerScale playerScale;
    [SerializeField] private Vector3 rootOffset;
    
    private bool _heightHasBeenAdjusted;
    private bool _isMoving;
    private bool _isJumping;
    private int _isJumpingParameterId;
    private int _jumpHeightParameterId;
    private int _isMovingParameterId;
    private int _directionXParameterId;
    private int _directionYParameterId;
    private int _hasJumpedParameterId;
    private int _jumpDirectionParameterId;
    private float _playAreaVelocityX;
    private float _playAreaVelocityY;
    private float _playAreaVelocityZ;
    
    private bool CanSendRPC => !PhotonNetwork.OfflineMode && thisPhotonView.IsMine;

    private const float ZERO = 0f;
    private const float ONE = 1.0f;
    private const string JUMP_HEIGHT = "JumpHeight";
    private const string IS_MOVING_NAME = "IsMoving";
    private const string DIRECTION_X_NAME = "DirectionX";
    private const string DIRECTION_Y_NAME = "DirectionY";
    private const string HAS_JUMPED_NAME = "HasJumped";
    private const string JUMP_DIRECTION = "JumpDirection";
    private const string IS_JUMPING = "IsJumping";
    

    private void Awake()
    {
        if (DebugLogger.IsNullError(rigidbodyStickLocomotion, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(playerScale, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(jump, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(modelRoot, "Must be set in editor.", this)) return;
        if (DebugLogger.IsNullError(thisPhotonView, "Must be set in editor.", this)) return;

        if (!thisPhotonView.IsMine)
        {
            enabled = false;
            return;
        }
        
        rigidbodyStickLocomotion.WasActivated += SetActiveLocomotionValues;
        rigidbodyStickLocomotion.IsNotActive += SetInactiveLocomotionValues;
        jump.HasJumped += Jump;
        jump.HasLanded += Land;

        _isJumpingParameterId = Animator.StringToHash(IS_JUMPING);
        _jumpHeightParameterId = Animator.StringToHash(JUMP_HEIGHT);
        _isMovingParameterId = Animator.StringToHash(IS_MOVING_NAME);
        _directionXParameterId = Animator.StringToHash(DIRECTION_X_NAME);
        _directionYParameterId = Animator.StringToHash(DIRECTION_Y_NAME);
        _hasJumpedParameterId = Animator.StringToHash(HAS_JUMPED_NAME);
        _jumpDirectionParameterId = Animator.StringToHash(JUMP_DIRECTION);
    }

    private void OnDestroy()
    {
        if (rigidbodyStickLocomotion && playerScale && jump)
        {
            rigidbodyStickLocomotion.WasActivated -= SetActiveLocomotionValues;
            rigidbodyStickLocomotion.IsNotActive -= SetInactiveLocomotionValues;
            jump.HasJumped -= Jump;
            jump.HasLanded -= Land;
        }
    }
    
    private void Update()
    {
        if (!playAreaRigidbody) return;
        
        var playAreaVelocity = playAreaRigidbody.velocity;
        _playAreaVelocityX = modelRoot.InverseTransformDirection(playAreaVelocity).x;
        _playAreaVelocityY = modelRoot.InverseTransformDirection(playAreaVelocity).y;
        _playAreaVelocityZ = modelRoot.InverseTransformDirection(playAreaVelocity).z;

        if (_isJumping)
        {
            vrik.solver.spine.headTarget = headTarget;
            animator.SetFloat(_jumpHeightParameterId, _playAreaVelocityY);
            animator.SetFloat(_jumpDirectionParameterId, _playAreaVelocityX);
        }
    }

    private void LateUpdate()
    {
        modelRoot.position = CalculateCenterOfMass();
    }

    private void OnAnimatorMove()
    {
        //This prevents hand/arm jitter.
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
    }

    private void SetActiveLocomotionValues(float x, float y)
    {
        _isMoving = true;

        //If we're jumping then we ignore the active state settings.
        if (_isJumping) return;
        
        SetWalkingValues();

        if (!_heightHasBeenAdjusted)
        {
            vrik.solver.spine.headTarget = walkingHeadTarget; 
            _heightHasBeenAdjusted = true;
        }

        animator.SetBool(_isMovingParameterId, true);
        animator.SetFloat(_directionXParameterId, _playAreaVelocityX);
        animator.SetFloat(_directionYParameterId, _playAreaVelocityZ);
    }

    private void SetInactiveLocomotionValues()
    {
        //If we're jumping then we ignore the inactive state settings.
        if (_isJumping)
        { 
            _isMoving = false;
            return;
        }
        
        animator.SetBool(_isMovingParameterId, false);
        
        SetIdleValues();
        
        if (_heightHasBeenAdjusted)
        {
            vrik.solver.spine.headTarget = headTarget; 
            _heightHasBeenAdjusted = false;
        }
        
        /*If we detect that the player was moving then we need to reset the solvers position
         * so we can change the LOD for IK to take over local movement.
         */
        if (_isMoving)
        {
            ResetVrikSolver();
        }

        _isMoving = false;
        animator.SetFloat(_directionXParameterId, 0);
        animator.SetFloat(_directionYParameterId, 0);
    }
    
    /*Possibly update this center of mass formula to account for chest and feet position vectors*/
    private Vector3 CalculateCenterOfMass()
    {
        var headTargetPosition = vrik.solver.spine.headTarget.position;
        var centerOfMass = new Vector3(headTargetPosition.x, playArea.position.y, headTargetPosition.z);
        
        //This adds the rootOffset adjustments to current animation to a body leaning effect.
        centerOfMass += modelRoot.rotation * rootOffset;        
        return centerOfMass;
    }
    
    private void SetWalkingValues()
    {
        if (CanSendRPC)
        {
            thisPhotonView.RPC(nameof(RPCSetWalkingValues), RpcTarget.OthersBuffered);
        }
        
        LocalSetWalkingValues();
    }
    
    [PunRPC]
    private void RPCSetWalkingValues()
    {
        LocalSetWalkingValues();
    }

    private void LocalSetWalkingValues()
    {
        vrik.solver.LOD = 1;
        vrik.solver.locomotion.weight = ZERO;
    }

    private void SetIdleValues()
    {
        if (CanSendRPC)
        {
            thisPhotonView.RPC(nameof(RPCSetIdleValues), RpcTarget.OthersBuffered);
        }
        
        LocalSetIdleValues();
    }
    
    [PunRPC]
    private void RPCSetIdleValues()
    {
        LocalSetIdleValues();
    }

    private void LocalSetIdleValues()
    {
        vrik.solver.LOD = 0;
        vrik.solver.locomotion.weight = ONE;
    }

    /*
     * Method called on event from Jump.cs to enable animation. Reset LOD and locomotion weight to allow
     * animation to display properly.
     */
    private void Jump()
    {
        _isJumping = true;
        animator.SetBool(_isJumpingParameterId, _isJumping);
        animator.SetTrigger(_hasJumpedParameterId);

        SetJumpValues();
    }

    private void SetJumpValues()
    {
        if (CanSendRPC)
        {
            thisPhotonView.RPC(nameof(RPCSetJumpValues), RpcTarget.Others);
        }
        
        LocalSetJumpValues();
    }
    
    [PunRPC]
    private void RPCSetJumpValues()
    {
        LocalSetJumpValues();
    }

    private void LocalSetJumpValues()
    {
        vrik.solver.LOD = 1;
        vrik.solver.locomotion.weight = ZERO;
    }

    /*
     * Method called on event from Jump.cs to detect landing.
     */
    private void Land()
    {
        _isJumping = false;
        animator.SetBool(_isJumpingParameterId, _isJumping);
        
        ResetVrikSolver();
    }

    [PunRPC]
    private void ResetVrikSolver()
    {
        if (CanSendRPC)
        {
            thisPhotonView.RPC(nameof(RPCResetVrikSolver), RpcTarget.OthersBuffered);
        }

        LocalResetVrikSolver();
    }

    [PunRPC]
    private void RPCResetVrikSolver()
    {
        LocalResetVrikSolver();
    }
    
    private void LocalResetVrikSolver()
    {
        vrik.solver.Reset();
    }
}
