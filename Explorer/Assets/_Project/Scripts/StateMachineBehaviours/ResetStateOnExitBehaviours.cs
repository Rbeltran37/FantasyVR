using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStateOnExitBehaviours : StateMachineBehaviour
{
    private AnimationReferenceHelper _animationReferenceHelper;
    

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_animationReferenceHelper)
        {
            _animationReferenceHelper = animator.GetComponent<AnimationReferenceHelper>();
        }
        
        if (!_animationReferenceHelper) return;
        
        if (_animationReferenceHelper.animationEventHandler) _animationReferenceHelper.animationEventHandler.DisableBothHitColliders();
        if (_animationReferenceHelper.simpleAi)
        {
            _animationReferenceHelper.simpleAi.EndDodge();
        }

        if (_animationReferenceHelper.animationEventHandler)
        {
            _animationReferenceHelper.animationEventHandler.EndAim();
        }
    }
}
