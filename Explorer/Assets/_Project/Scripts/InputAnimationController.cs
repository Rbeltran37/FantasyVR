using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VRTK.Prefabs.CameraRig.UnityXRCameraRig.Input;

public class InputAnimationController : MonoBehaviour
{
    public enum Hand
    {
        Left,
        Right
    };

    public Hand hand;
    public Animator rightAnimator;
    public Animator leftAnimator;

    public void SetGripButtonAnimation(float gripValue)
    {
        if (!rightAnimator || !leftAnimator)
            return;
        
        if (hand == Hand.Right)
        {
            rightAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            leftAnimator.SetFloat("Grip", gripValue);
        }
    }
    
    public void SetTriggerAnimation(float triggerValue)
    {
        if (!rightAnimator || !leftAnimator)
            return;
        
        if (hand == Hand.Right)
        {
            rightAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            leftAnimator.SetFloat("Trigger", triggerValue);
        }
    }
    
    public void SetJoystickAnimation(Vector2 axisValues)
    {
        if (!rightAnimator || !leftAnimator)
            return;
        
        if (hand == Hand.Right)
        {
            rightAnimator.SetFloat("Joy X", axisValues.x);
            rightAnimator.SetFloat("Joy Y", axisValues.y);
        }
        else
        {
            leftAnimator.SetFloat("Joy X", axisValues.x);
            leftAnimator.SetFloat("Joy Y", axisValues.y);
        }
    }
    
    public void ButtonOneAnimationActivated()
    {
        if (!rightAnimator || !leftAnimator)
            return;
        
        if (hand == Hand.Right)
        {
            rightAnimator.SetFloat("Button 1", 1);
        }
        else
        {
            leftAnimator.SetFloat("Button 1", 1);
        }
    }
    
    public void ButtonOneAnimationDeactivated()
    {
        if (!rightAnimator || !leftAnimator)
            return;
        
        if (hand == Hand.Right)
        {
            rightAnimator.SetFloat("Button 1", 0);
        }
        else
        {
            leftAnimator.SetFloat("Button 1", 0);
        }
    }
    
    public void ButtonTwoAnimationActivated()
    {
        if (!rightAnimator || !leftAnimator)
            return;
        
        if (hand == Hand.Right)
        {
            rightAnimator.SetFloat("Button 2", 1);
        }
        else
        {
            leftAnimator.SetFloat("Button 2", 1);
        }
    }
    
    public void ButtonTwoAnimationDeactivated()
    {
        if (!rightAnimator || !leftAnimator)
            return;
        
        if (hand == Hand.Right)
        {
            rightAnimator.SetFloat("Button 2", 0);
        }
        else
        {
            leftAnimator.SetFloat("Button 2", 0);
        }
    }
}