using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipBehaviours : StateMachineBehaviour
{
    private AnimationReferenceHelper _animationReferenceHelper;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_animationReferenceHelper)
        {
            _animationReferenceHelper = animator.GetComponent<AnimationReferenceHelper>();
        }
        
        if (!_animationReferenceHelper) return;

        if (_animationReferenceHelper.simpleAi)
        {
            //_animationReferenceHelper.simpleAi.IsEquipping(true);
        }
    }
}
