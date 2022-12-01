using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class IdleLocomotionBehavior : StateMachineBehaviour
{
    private VRAnimationController _vrAnimationController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (!_vrAnimationController)
        {
            _vrAnimationController = animator.GetComponent<VRAnimationController>();
        }

        if (!_vrAnimationController)
        {
            Debug.Log("VR Animation Controller is missing from animator");
            return;
        }

        //_vrAnimationController.SetIdleValues();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_vrAnimationController)
        {
            _vrAnimationController = animator.GetComponent<VRAnimationController>();
        }

        if (!_vrAnimationController)
        {
            Debug.Log("VR Animation Controller is missing from animator");
            return;
        }

        //_vrAnimationController.SetVRIKActiveValues();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
