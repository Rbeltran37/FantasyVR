using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class PuppetHeadHitReaction : MonoBehaviour
{
    [SerializeField] private PuppetHeadHitReactionData puppetHeadHitReactionData;
    [SerializeField] private AnimatorHitReactionHandler animatorHitReactionHandler;
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] private Transform puppetHeadTransform;
    [SerializeField] private Transform characterControllerTransform;

    private bool _isHeadActive = true;

    private void Awake()
    {
        if (animatorHitReactionHandler)
        {
            animatorHitReactionHandler.HeadWasHit += ActivateHeadHitReaction;
        }
    }

    private void OnDestroy()
    {
        if (animatorHitReactionHandler)
        {
            animatorHitReactionHandler.HeadWasHit -= ActivateHeadHitReaction;
        }
    }

    //TODO possibly optimize
    private void FixedUpdate()
    {
        if (_isHeadActive)
        {
            SetHeadRegainPinSpeed();
        }
    }

    private void SetHeadRegainPinSpeed()
    {
        var rootPosition = characterControllerTransform.position;
        var puppetHeadPosition = puppetHeadTransform.position;
        var adjustedHeadPosition = new Vector3(puppetHeadPosition.x, rootPosition.y, puppetHeadPosition.z);
        var headDistanceFromRoot = Vector3.Distance(adjustedHeadPosition, rootPosition);
        if (headDistanceFromRoot > puppetHeadHitReactionData.headDistanceLimit)
        {
            behaviourPuppet.groupOverrides[0].props.regainPinSpeed = puppetHeadHitReactionData.headHitRegain;
        }
        else if (headDistanceFromRoot < puppetHeadHitReactionData.headRegainPinDistance)
        {
            behaviourPuppet.groupOverrides[0].props.regainPinSpeed = puppetHeadHitReactionData.defaultHeadRegain;
            _isHeadActive = false;
        }
    }

    public BehaviourPuppet GetBehaviourPuppet()
    {
        return behaviourPuppet;
    }

    private void ActivateHeadHitReaction()
    {
        StartCoroutine(ActivateHeadHitReactionCoroutine());
    }

    private IEnumerator ActivateHeadHitReactionCoroutine()
    {
        yield return new WaitForSeconds(puppetHeadHitReactionData.activationBufferTime);
        _isHeadActive = true;
    }
}
