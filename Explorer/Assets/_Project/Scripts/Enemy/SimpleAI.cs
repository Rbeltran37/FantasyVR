using System;
using System.Collections;
using System.Reflection;
using BehaviorDesigner.Runtime;
using Photon.Pun;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleAI : MonoBehaviourPun
{
    [SerializeField] private SimpleAiData simpleAiData;
    [SerializeField] private Animator animator;
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] private Health health;
    [SerializeField] private CustomCharacterAnimationThirdPerson customCharacterAnimationThirdPerson;
    [SerializeField] private Behavior behavior;
    [SerializeField] private LookAtIK lookAtIk;
    [SerializeField] private AimIK leftAimIk;
    [SerializeField] private AimIK rightAimIk;

    public Action AimStarted;

    private bool _isBlocking;
    private bool _isDodging;
    private bool _canAttack;
    private bool _isWaiting;
    private Transform _currentTarget;
    private Transform _lastPlayerTarget;

    private const int INACTIVE_INDEX = -1;
    private const float DELAY = .5f;


    private void Start()
    {
        SubscribeToEvents();

        _canAttack = true;

        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) behavior.enabled = false;        //TODO may be obsolete
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    private void SubscribeToEvents()
    {
        if (DebugLogger.IsNullError(simpleAiData, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(behaviourPuppet, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(behavior, this, "Must be set in editor.")) return;

        behaviourPuppet.onLoseBalance.unityEvent.AddListener(ResetTriggers);
        behaviourPuppet.onLoseBalance.unityEvent.AddListener(DisableLookAndAimIk);
        behaviourPuppet.onRegainBalance.unityEvent.AddListener(EnableLookAndAimIk);

        health.WasKilled += DisableBehaviorTree;
        health.WasKilled += DisableLookAndAimIk;
    }

    private void UnsubscribeFromEvents()
    {
        if (behaviourPuppet)
        {
            behaviourPuppet.onLoseBalance.unityEvent.RemoveListener(ResetTriggers);
            behaviourPuppet.onLoseBalance.unityEvent.RemoveListener(DisableLookAndAimIk);
            behaviourPuppet.onRegainBalance.unityEvent.RemoveListener(EnableLookAndAimIk);
        }

        if (health)
        {
            if (behavior)
            {
                health.WasKilled -= DisableBehaviorTree;
            }

            health.WasKilled -= DisableLookAndAimIk;
        }
    }

    public void SetSimpleAiData(SimpleAiData simpleAiData)
    {
        this.simpleAiData = simpleAiData;
    }

    public void SetPlayerTarget(Transform playerTarget)
    {
        _lastPlayerTarget = !playerTarget ? _lastPlayerTarget : playerTarget;
        SetTarget(_lastPlayerTarget, true, true);
    }

    public void SetTarget(Transform target, bool setLeftHand, bool setRightHand)
    {
        SetLookAtIkTarget(target);
        SetAimIkTarget(target, setLeftHand, setRightHand);
    }
    
    public void RandomAttack()
    {
        var randomIndex = Random.Range(0, simpleAiData.numAttacks);
        StartCoroutine(AttackCoroutine(randomIndex));
    }

    public void ShortRangeAttack()
    {
        var randomIndex = Random.Range(simpleAiData.shortAttackMin, simpleAiData.shortAttackMax);
        StartCoroutine(AttackCoroutine(randomIndex));
    }

    public void MediumRangeAttack()
    {
        var randomIndex = Random.Range(simpleAiData.mediumAttackMin, simpleAiData.mediumAttackMax);
        StartCoroutine(AttackCoroutine(randomIndex));
    }

    public void LongRangeAttack()
    {
        var randomIndex = Random.Range(simpleAiData.longAttackMin, simpleAiData.longAttackMax);
        StartCoroutine(AttackCoroutine(randomIndex));
    }

    public void ProjectileAttack()
    {
        var randomIndex = Random.Range(simpleAiData.projectileAttackMin, simpleAiData.projectileAttackMax);
        StartCoroutine(AttackCoroutine(randomIndex));
    }

    public void Stun(float stunTime) 
    {
        var isStunned = animator.GetBool(simpleAiData.isStunnedBoolId);
        if (isStunned)
            return;

        StartCoroutine(HoldStunCoroutine(stunTime));
    }
    
    public bool CanTakeAction()
    {
        if (_isWaiting) return false;
        
        var isStunned = animator.GetBool(simpleAiData.isStunnedBoolId);
        if (isStunned) return false;
        
        var isPinned = behaviourPuppet.state == BehaviourPuppet.State.Puppet;
        if (!isPinned) return false;
        
        var isInGroundedState = customCharacterAnimationThirdPerson.animationGrounded;
        return isInGroundedState || _isBlocking;
    }

    [Button]
    public void Wait()
    {
        _isWaiting = true;
        DisableBehaviorTree();
    }

    [Button]
    public void EndWait()
    {
        _isWaiting = false;
        EnableBehaviourTree();
    }

    private void EnableBehaviourTree()
    {
        behavior.EnableBehavior();
    }

    public bool CanAttack()
    {
        return _canAttack;
    }

    public void BackUp()
    {
        var randomIndex = Random.Range(0, simpleAiData.numBackUpClips);
        StartCoroutine(BackUpCoroutine(randomIndex));
    }

    public bool GetIsBlocking()
    {
        return _isBlocking;
    }
    
    public void Block(GameObject blockTarget)
    {
        if (_isDodging || !simpleAiData) return;

        //Start Block animation
        animator.SetBool(simpleAiData.blockBoolId, true);
        
        //TODO Check if able to block with both hands
        
        //Check if object is on the left side or right side
        if (!blockTarget) return;
        var targetTransform = blockTarget.transform;
        var targetHeading = VectorMath.GetTargetHeading(transform, targetTransform);
        bool targetOnRight = Math.Abs(VectorMath.IsTargetToTheLeftOrRight(targetHeading, transform) - 1) < Mathf.Epsilon;
        
        //Choose which arm to block with
        if (rightAimIk && (simpleAiData.blockHandedness == AnimationReferenceHelper.Handedness.Right || targetOnRight))
        {
            SetTarget(targetTransform, false, true);
            SetBlockIkValues(false);

            //Check if left hand was attempting to block target
            if (leftAimIk && leftAimIk.solver.target == targetTransform)
            {
                SetDefaultIkValues(true, false);
            }
        }
        else if (leftAimIk && (simpleAiData.blockHandedness == AnimationReferenceHelper.Handedness.Left || !targetOnRight))
        {
            SetBlockIkValues(true);
            SetTarget(targetTransform, true, false);
            
            //Check if right hand was attempting to block target
            if (rightAimIk && rightAimIk.solver.target == targetTransform)
            {
                SetDefaultIkValues(false, true);
            }
        }

        if (puppetMaster) puppetMaster.angularPinning = true;
        _isBlocking = true;
    }

    private void SetBlockIkValues(bool isLeft)
    {
        if (isLeft && leftAimIk)
        {
            leftAimIk.enabled = true;
            leftAimIk.solver.axis = CustomEnums.Instance.GetAxis(simpleAiData.blockAxis);
            leftAimIk.solver.clampWeight = simpleAiData.blockClampWeight;
            var bones = leftAimIk.solver.bones;
            for (int i = 0; i < bones.Length && i < simpleAiData.blockWeights.Length; i++)
            {
                bones[i].weight = simpleAiData.blockWeights[i];
            }
        }
        else if (!isLeft && rightAimIk)
        {
            rightAimIk.enabled = true;
            rightAimIk.solver.axis = CustomEnums.Instance.GetAxis(simpleAiData.blockAxis);
            rightAimIk.solver.clampWeight = simpleAiData.blockClampWeight;
            var bones = rightAimIk.solver.bones;
            for (int i = 0; i < bones.Length && i < simpleAiData.blockWeights.Length; i++)
            {
                bones[i].weight = simpleAiData.blockWeights[i];
            }
        }
    }

    public void EndBlock()
    {
        if (!_isBlocking) return;
        
        SetPlayerTarget(_lastPlayerTarget);
        SetDefaultIkValues(true, true);
        
        animator.SetBool(simpleAiData.blockBoolId, false);
        _isBlocking = false;
    }

    public void SetAttackIkValues(bool setLeft, bool setRight)
    {
        if (leftAimIk && setLeft)
        {
            leftAimIk.enabled = true;
            leftAimIk.solver.axis = CustomEnums.Instance.GetAxis(simpleAiData.attackAxis);
            leftAimIk.solver.clampWeight = simpleAiData.attackClampWeight;
            var bones = leftAimIk.solver.bones;
            for (int i = 0; i < bones.Length && i < simpleAiData.attackWeights.Length; i++)
            {
                bones[i].weight = simpleAiData.attackWeights[i];
            }
            
            SetAimAccuracy(leftAimIk);
        }

        if (rightAimIk && setRight)
        {
            rightAimIk.enabled = true;
            rightAimIk.solver.axis = CustomEnums.Instance.GetAxis(simpleAiData.attackAxis);
            rightAimIk.solver.clampWeight = simpleAiData.attackClampWeight;
            var bones = rightAimIk.solver.bones;
            for (int i = 0; i < bones.Length && i < simpleAiData.attackWeights.Length; i++)
            {
                bones[i].weight = simpleAiData.attackWeights[i];
            }
            
            SetAimAccuracy(rightAimIk);
        }
        
        if (puppetMaster) puppetMaster.angularPinning = true;
        
        AimStarted?.Invoke();
    }

    private void SetAimAccuracy(AimIK aimIK)
    {
        var randomAccuracy = Random.Range(-simpleAiData.accuracy, simpleAiData.accuracy);
        aimIK.solver.axis = CustomEnums.Instance.GetAxis(simpleAiData.attackAxis) + Vector3.one * randomAccuracy;
    }

    public void SetDefaultIkValues(bool setLeft, bool setRight)
    {
        if (leftAimIk && setLeft)
        {
            leftAimIk.enabled = false;
        }

        if (rightAimIk && setRight)
        {
            rightAimIk.enabled = false;
        }
        
        if (puppetMaster) puppetMaster.angularPinning = false;
    }

    public void AttackCooldown(float waitTime)
    {
        StartCoroutine(AttackCooldownCoroutine(waitTime));
    }

    public void EndDodge()
    {
        _isDodging = false;
    }

    public void ResetValues()
    {
        ResetTriggers();
        animator.SetInteger(simpleAiData.actionIndexId, INACTIVE_INDEX);
        animator.SetInteger(simpleAiData.backUpIndexId, INACTIVE_INDEX);
    }

    public void Parried()
    {
        animator.SetTrigger(simpleAiData.isParriedId);
    }

    public void DodgeForward()
    {
        if (_isBlocking || _isDodging) return;
        
        animator.SetTrigger(simpleAiData.dodgeForwardTriggerId);
        _isDodging = true;
    }
    
    public void DodgeLeft()
    {
        if (_isBlocking || _isDodging) return;
        
        animator.SetTrigger(simpleAiData.dodgeLeftTriggerId);
        _isDodging = true;
    }
    
    public void DodgeRight()
    {
        if (_isBlocking || _isDodging) return;
        
        animator.SetTrigger(simpleAiData.dodgeRightTriggerId);
        _isDodging = true;
    }

    public void ChangeWeapons(bool primaryState, bool secondaryState)
    {
        if (!animator || !simpleAiData) return;
        
        animator.SetBool(simpleAiData.isPrimaryEquippedBoolId, primaryState);
        animator.SetBool(simpleAiData.isSecondaryEquippedBoolId, secondaryState);
        ResetTriggers();
    }

    public void EquipPrimary()
    {
        if (!animator || !simpleAiData) return;
        
        animator.SetTrigger(simpleAiData.equipPrimaryTriggerId);
    }

    public void EquipSecondary()
    {
        if (!animator || !simpleAiData) return;

        animator.SetTrigger(simpleAiData.equipSecondaryTriggerId);
    }
    
    public void ResetTriggers()
    {
        if (!animator || !simpleAiData) return;
        
        animator.ResetTrigger(simpleAiData.dodgeForwardTriggerId);
        animator.ResetTrigger(simpleAiData.equipPrimaryTriggerId);
        animator.ResetTrigger(simpleAiData.equipSecondaryTriggerId);
    }

    private IEnumerator HoldStunCoroutine(float stunTime) 
    {
        animator.SetBool(simpleAiData.isStunnedBoolId, true);
        yield return new WaitForSeconds(stunTime);
        animator.SetBool(simpleAiData.isStunnedBoolId, false);
    }

    private void DisableBehaviorTree()
    {
        behavior.DisableBehavior(false);
    }

    private IEnumerator AttackCoroutine(int attackIndex)
    {
        animator.SetInteger(simpleAiData.actionIndexId, attackIndex);
        yield return new WaitForSeconds(DELAY);
        animator.SetInteger(simpleAiData.actionIndexId, INACTIVE_INDEX);
    }

    private IEnumerator BackUpCoroutine(int backUpIndex)
    {
        animator.SetInteger(simpleAiData.backUpIndexId, backUpIndex);
        yield return new WaitForSeconds(DELAY);
        animator.SetInteger(simpleAiData.backUpIndexId, INACTIVE_INDEX);
    }

    private IEnumerator AttackCooldownCoroutine(float waitTime)
    {
        _canAttack = false;
        yield return new WaitForSeconds(waitTime);
        _canAttack = true;
    }
    
    private void SetLookAtIkTarget(Transform target)
    {
        if (!lookAtIk) return; lookAtIk.solver.target = target;
    }

    private void SetAimIkTarget(Transform target, bool setLeftHand, bool setRightHand)
    {
        if (setLeftHand && leftAimIk) leftAimIk.solver.target = target;
        if (setRightHand && rightAimIk) rightAimIk.solver.target = target;
    }

    private void DisableLookAndAimIk()
    {
        if (lookAtIk) lookAtIk.enabled = false;
        if (rightAimIk) rightAimIk.enabled = false;
        if (leftAimIk) leftAimIk.enabled = false;
    }
    
    private void EnableLookAndAimIk()
    {
        if (lookAtIk) lookAtIk.enabled = true;
        if (rightAimIk) rightAimIk.enabled = true;
        if (leftAimIk) leftAimIk.enabled = true;
    }
}
