using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class HandAnimationHandler : MonoBehaviour
{
    [SerializeField] private HandAnimationHandlerData handAnimationHandlerData;
    [SerializeField] private Animator animator;

    private float _currentLeftCallId;
    private float _currentRightCallId;

    private void Start()
    {
        RestHand(true);
        RestHand(false);
    }

    [Button]
    public void OpenHand(bool isLeftHand)
    {
        StartCoroutine(OpenHandCoroutine(isLeftHand));
    }
    
    public void HoldOpen(bool isLeftHand)
    {
        StartCoroutine(HoldOpenCoroutine(isLeftHand));
    }

    [Button]
    public void CloseHand(bool isLeftHand)
    {
        StartCoroutine(CloseHandCoroutine(isLeftHand));
    }

    public void CloseThenRest(bool isLeftHand)
    {
        StartCoroutine(CloseHandThenRestCoroutine(isLeftHand));
    }

    public void Grab(bool isLeftHand)
    {
        StartCoroutine(GrabHandCoroutine(isLeftHand));
    }

    public void RestHand(bool isLeftHand)
    {
        StartCoroutine(RestHandCoroutine(isLeftHand));
    }

    private IEnumerator OpenHandCoroutine(bool isLeftHand)
    {
        var currentCallId = GetCurrentCallId(isLeftHand);
        var handId = GetHandId(isLeftHand);
        
        var speed = handAnimationHandlerData.openHandSpeed;
        var interval = handAnimationHandlerData.openHandInterval;
        var randomValue = Random.Range(handAnimationHandlerData.openHand.minValue, 
            handAnimationHandlerData.openHand.maxValue);
        var currentValue = animator.GetFloat(handId);
        while (currentValue < randomValue)
        {
            currentValue += speed;
            if (currentValue > randomValue) currentValue = randomValue;
            animator.SetFloat(handId, currentValue);
            yield return new WaitForSeconds(interval);
            if (isLeftHand && Math.Abs(currentCallId - _currentLeftCallId) > Mathf.Epsilon || 
                !isLeftHand && Math.Abs(currentCallId - _currentRightCallId) > Mathf.Epsilon)
            {
                yield break;
            }
        }

        StartCoroutine(RestHandCoroutine(isLeftHand));
    }
    
    private IEnumerator HoldOpenCoroutine(bool isLeftHand)
    {
        var currentCallId = GetCurrentCallId(isLeftHand);
        var handId = GetHandId(isLeftHand);
        
        var speed = handAnimationHandlerData.openHandSpeed;
        var interval = handAnimationHandlerData.openHandInterval;
        var randomValue = Random.Range(handAnimationHandlerData.openHand.minValue, 
            handAnimationHandlerData.openHand.maxValue);
        var currentValue = animator.GetFloat(handId);
        while (currentValue < randomValue)
        {
            currentValue += speed;
            if (currentValue > randomValue) currentValue = randomValue;
            animator.SetFloat(handId, currentValue);
            yield return new WaitForSeconds(interval);
            if (isLeftHand && Math.Abs(currentCallId - _currentLeftCallId) > Mathf.Epsilon || 
                !isLeftHand && Math.Abs(currentCallId - _currentRightCallId) > Mathf.Epsilon)
            {
                yield break;
            }
        }
    }

    private IEnumerator CloseHandCoroutine(bool isLeftHand)
    {
        var currentCallId = GetCurrentCallId(isLeftHand);
        var handId = GetHandId(isLeftHand);
        
        var speed = handAnimationHandlerData.closeHandSpeed;
        var interval = handAnimationHandlerData.closeHandInterval;
        var randomValue = Random.Range(handAnimationHandlerData.closeHand.minValue, 
            handAnimationHandlerData.closeHand.maxValue);
        var currentValue = animator.GetFloat(handId);
        while (currentValue > randomValue)
        {
            currentValue -= speed;
            if (currentValue < randomValue) currentValue = randomValue;
            animator.SetFloat(handId, currentValue);
            yield return new WaitForSeconds(interval);
            if (isLeftHand && Math.Abs(currentCallId - _currentLeftCallId) > Mathf.Epsilon || 
                !isLeftHand && Math.Abs(currentCallId - _currentRightCallId) > Mathf.Epsilon)
            {
                yield break;
            }
        }
    }
    
    private IEnumerator CloseHandThenRestCoroutine(bool isLeftHand)
    {
        var currentCallId = GetCurrentCallId(isLeftHand);
        var handId = GetHandId(isLeftHand);
        
        var speed = handAnimationHandlerData.closeHandSpeed;
        var interval = handAnimationHandlerData.closeHandInterval;
        var randomValue = Random.Range(handAnimationHandlerData.closeHand.minValue, 
            handAnimationHandlerData.closeHand.maxValue);
        var currentValue = animator.GetFloat(handId);
        while (currentValue > randomValue)
        {
            currentValue -= speed;
            if (currentValue < randomValue) currentValue = randomValue;
            animator.SetFloat(handId, currentValue);
            yield return new WaitForSeconds(interval);
            if (isLeftHand && Math.Abs(currentCallId - _currentLeftCallId) > Mathf.Epsilon || 
                !isLeftHand && Math.Abs(currentCallId - _currentRightCallId) > Mathf.Epsilon)
            {
                yield break;
            }
        }
        
        StartCoroutine(RestHandCoroutine(isLeftHand));
    }
    
    private IEnumerator RestHandCoroutine(bool isLeftHand)
    {
        var currentCallId = GetCurrentCallId(isLeftHand);
        var handId = GetHandId(isLeftHand);

        var speed = handAnimationHandlerData.restHandSpeed;
        var interval = handAnimationHandlerData.restHandInterval;
        var randomValue = Random.Range(handAnimationHandlerData.restHand.minValue, 
            handAnimationHandlerData.restHand.maxValue);
        var currentValue = animator.GetFloat(handId);
        if (currentValue > randomValue)
        {
            while (currentValue > randomValue)
            {
                currentValue -= speed;
                if (currentValue < randomValue) currentValue = randomValue;
                animator.SetFloat(handId, currentValue);
                yield return new WaitForSeconds(interval);
                if (isLeftHand && Math.Abs(currentCallId - _currentLeftCallId) > Mathf.Epsilon || 
                    !isLeftHand && Math.Abs(currentCallId - _currentRightCallId) > Mathf.Epsilon)
                {
                    break;
                }
            }
        }
        else
        {
            while (currentValue < randomValue)
            {
                currentValue += handAnimationHandlerData.restHandSpeed;
                if (currentValue > randomValue) currentValue = randomValue;
                animator.SetFloat(handId, currentValue);
                yield return new WaitForSeconds(handAnimationHandlerData.restHandInterval);
                if (isLeftHand && Math.Abs(currentCallId - _currentLeftCallId) > Mathf.Epsilon || 
                    !isLeftHand && Math.Abs(currentCallId - _currentRightCallId) > Mathf.Epsilon)
                {
                    yield break;
                }
            }
        }
    }
    
    private IEnumerator GrabHandCoroutine(bool isLeftHand)
    {
        var currentCallId = GetCurrentCallId(isLeftHand);
        var handId = GetHandId(isLeftHand);

        var speed = handAnimationHandlerData.grabHandSpeed;
        var interval = handAnimationHandlerData.grabHandInterval;
        var randomValue = Random.Range(handAnimationHandlerData.grabHand.minValue, 
            handAnimationHandlerData.grabHand.maxValue);
        var currentValue = animator.GetFloat(handId);
        if (currentValue > randomValue)
        {
            while (currentValue > randomValue)
            {
                currentValue -= speed;
                if (currentValue < randomValue) currentValue = randomValue;
                animator.SetFloat(handId, currentValue);
                yield return new WaitForSeconds(interval);
                if (isLeftHand && Math.Abs(currentCallId - _currentLeftCallId) > Mathf.Epsilon || 
                    !isLeftHand && Math.Abs(currentCallId - _currentRightCallId) > Mathf.Epsilon)
                {
                    break;
                }
            }
        }
        else
        {
            while (currentValue < randomValue)
            {
                currentValue += handAnimationHandlerData.grabHandSpeed;
                if (currentValue > randomValue) currentValue = randomValue;
                animator.SetFloat(handId, currentValue);
                yield return new WaitForSeconds(handAnimationHandlerData.grabHandInterval);
                if (isLeftHand && Math.Abs(currentCallId - _currentLeftCallId) > Mathf.Epsilon || 
                    !isLeftHand && Math.Abs(currentCallId - _currentRightCallId) > Mathf.Epsilon)
                {
                    yield break;
                }
            }
        }
    }

    private float GetCurrentCallId(bool isLeftHand)
    {
        if (isLeftHand)
        {
            _currentLeftCallId = Time.time;
        }
        else
        {
            _currentRightCallId = Time.time;
        }

        var currentHandId = isLeftHand ? _currentLeftCallId : _currentRightCallId;
        return currentHandId;
    }

    private int GetHandId(bool isLeftHand)
    {
        var hand = isLeftHand ? handAnimationHandlerData.leftHandString : handAnimationHandlerData.rightHandString;
        var handId = Animator.StringToHash(hand);
        return handId;
    }
}
