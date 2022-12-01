using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Tracking.Velocity;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private CustomEnums.Hand hand;
    [SerializeField] private ControllerHaptics controllerHaptics;
    [SerializeField] private HandAnimationHandler handAnimationHandler;
    [SerializeField] private AudioSource burstAudioSource;
    [SerializeField] private AudioSource loopAudioSource;
    [SerializeField] private Transform pullTarget;
    [SerializeField] private Transform puppetHandTransform;
    [SerializeField] private Rigidbody puppetHandRigidbody;
    [SerializeField] private Transform puppetForearmTransform;
    [SerializeField] private Rigidbody puppetForearmRigidbody;
    [SerializeField] private AverageVelocityEstimator averageVelocityEstimator;
    [SerializeField] private Interactor interactor;
    
    private bool _isLeft;

    
    private void Awake()
    {
        _isLeft = hand == CustomEnums.Hand.Left;
    }

    public bool IsLeftHand()
    {
        return _isLeft;
    }
    
    public Transform GetPullTarget()
    {
        return pullTarget;
    }

    public Transform GetPuppetHandTransform()
    {
        return puppetHandTransform;
    }
    
    public Transform GetPuppetForearmTransform()
    {
        return puppetForearmTransform;
    }
    
    public Rigidbody GetPuppetHandRigidbody()
    {
        return puppetHandRigidbody;
    }
    
    public Rigidbody GetPuppetForearmRigidbody()
    {
        return puppetForearmRigidbody;
    }

    public Interactor GetInteractor()
    {
        return interactor;
    }

    public ControllerHaptics GetControllerHaptics()
    {
        return controllerHaptics;
    }

    public void Open()
    {
        if (DebugLogger.IsNullError(handAnimationHandler, this)) return;
        
        handAnimationHandler.OpenHand(_isLeft);
    }

    public void Close()
    {
        if (DebugLogger.IsNullError(handAnimationHandler, this)) return;

        handAnimationHandler.CloseHand(_isLeft);
    }

    public void CloseThenRest()
    {
        if (DebugLogger.IsNullError(handAnimationHandler, this)) return;

        handAnimationHandler.CloseThenRest(_isLeft);
    }

    public void GrabObject()
    {
        if (DebugLogger.IsNullError(handAnimationHandler, this)) return;

        handAnimationHandler.Grab(_isLeft);
    }

    public void Rest()
    {
        if (DebugLogger.IsNullError(handAnimationHandler, this)) return;
        
        handAnimationHandler.RestHand(_isLeft);
    }

    public void HoldOpen()
    {
        if (DebugLogger.IsNullError(handAnimationHandler, this)) return;
        
        handAnimationHandler.HoldOpen(_isLeft);
    }

    public AudioSource GetBurstAudioSource()
    {
        return burstAudioSource;
    }
    
    public AudioSource GetLoopAudioSource()
    {
        return loopAudioSource;
    }

    public AverageVelocityEstimator GetAverageVelocityEstimator()
    {
        return averageVelocityEstimator;
    }
}
