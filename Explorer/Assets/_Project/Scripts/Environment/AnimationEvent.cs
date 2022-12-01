using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

//TODO work in progress
public class AnimationEvent : ArenaEvent
{
    [SerializeField] private Animator animator;
    [SerializeField] private string parameterName = "isOpen";
    [Tooltip("If parameter is bool, value is toggled.")]
    [SerializeField] private ParameterType parameterType = ParameterType.Bool;

    private int _parameterId;
    private bool _boolState;

    private enum ParameterType
    {
        Trigger,
        Bool
    }


    protected override void Awake()
    {
        _parameterId = Animator.StringToHash(parameterName);
        
        base.Awake();
    }

    public override void Begin()
    {
        base.Begin();
        
        if (PhotonNetwork.IsMasterClient)
        {
            if (DebugLogger.IsNullError(animator, this, "Must be set in editor.")) return;

            animator.enabled = true;
            
            switch (parameterType)
            {
                case ParameterType.Bool:
                    _boolState = animator.GetBool(_parameterId);
                    animator.SetBool(_parameterId, !_boolState);
                    break;
                case ParameterType.Trigger:
                    animator.SetTrigger(_parameterId);
                    break;
            }
        }
    }
}
