using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class BalancePuppet : MonoBehaviour
{
    [SerializeField] private BehaviourPuppet _behaviourPuppet;
    [SerializeField] private Transform _puppetHead;
    [SerializeField] private Transform _root;

    [SerializeField] private bool _isMaintainingBalance = false;
    [SerializeField] private float _regainBalanceSpeed = .01f;
    [SerializeField] private float _minBalanceDistance = .1f;
    
    
    private bool _isHeadActive = true;
    

    private void FixedUpdate()
    {
        if (_isMaintainingBalance)
        {
            MaintainBalance();
        }
    }

    private void MaintainBalance()
    {
        var puppetHeadPosition = _puppetHead.position;
        var headFloorVector = new Vector3(puppetHeadPosition.x, _root.position.y, puppetHeadPosition.z);
        if (Vector3.Distance(headFloorVector, _root.position) > _minBalanceDistance)
        {
            _root.position += (headFloorVector - _root.position) * _regainBalanceSpeed;
        }
    }

    public BehaviourPuppet GetBehaviourPuppet()
    {
        return _behaviourPuppet;
    }

    public void SetIsMaintainingBalance(bool state)
    {
        _isMaintainingBalance = state;
    }

    public void SetIsHeadActive(bool state)
    {
        _isHeadActive = state;
    }
}
