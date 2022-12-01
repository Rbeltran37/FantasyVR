using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;

public class CrowdInteractor : MonoBehaviour
{
    [SerializeField] private Transform headAnchor;
    [SerializeField] private Transform leftAnchor;
    [SerializeField] private Transform rightAnchor;
    [SerializeField] private bool isCheckingForCrowdInteraction;
    private CrowdController _crowdController;
    private CrowdEngagementTracker _crowdEngagementTracker;

    public delegate void CrowdInteractionHandler(bool isInteracting);
    public event CrowdInteractionHandler handsAreRaised;
    

    // Start is called before the first frame update
    void Start()
    {
        if (!headAnchor)
        {
            DebugLogger.Error("Start", "Head Anchor is missing.", this);
            return;
        }
        if (!leftAnchor)
        {
            DebugLogger.Error("Start", "Left Anchor is missing.", this);
            return;
        }
        if (!rightAnchor)
        {
            DebugLogger.Error("Start", "Right Anchor is missing.", this);
        }
        
        _crowdController = FindObjectOfType<CrowdController>();
        _crowdEngagementTracker = FindObjectOfType<CrowdEngagementTracker>();

        if (!_crowdController || !_crowdEngagementTracker)
        {
            isCheckingForCrowdInteraction = false;
            return;
        }
        
        _crowdController.SetPlayerPosition(headAnchor);
    }

    // Update is called once per frame
    void Update()
    {
        //This will be either enabled or disabled for player in between rounds or 
        //whenever we allow the player the interact with the crowd.
        
        if (isCheckingForCrowdInteraction)
        {
            //Currently set to check for average hand height above head but can be set to check for any of the methods below.
            //Only thing required is the check to make sure its not playing to ensure it doesnt repeat on itself. 
            
            if (IsAverageHandHeightOverHead())
            //if (BothHandsOverHead() && !crowdController.reactionCrowdSource.isPlaying)
            //if (IsLeftHandOverHead() && !crowdController.reactionCrowdSource.isPlaying)
            //if (IsRightHandOverHead() && !crowdController.reactionCrowdSource.isPlaying)
            {
                if (!_crowdController.IsAudioPlaying())
                {
                    if (_crowdController)
                    {
                        _crowdEngagementTracker.InitiateCounter(true);
                        _crowdController.SynchronizePositiveCrowdAudio();
                    }
                }
            }
            else
            {
                //Not engaging crowd.
                _crowdEngagementTracker.InitiateCounter(false);
            }
        }
    }

    private Vector3 GetLeftHandPosition()
    {
        return leftAnchor.localPosition;
    }
    
    private Vector3 GetRightHandPosition()
    {
        return rightAnchor.localPosition;
    }

    private bool IsAverageHandHeightOverHead()
    {
        var averageVector = (GetLeftHandPosition() + GetRightHandPosition()) / 2;
        return averageVector.y > headAnchor.position.y;
    }

    private bool BothHandsOverHead()
    {
        return IsLeftHandOverHead() && IsRightHandOverHead();
    }

    private bool IsLeftHandOverHead()
    {
        return GetLeftHandPosition().y > headAnchor.position.y;
    }
    
    private bool IsRightHandOverHead()
    {
        return GetRightHandPosition().y > headAnchor.position.y;
    }
}
