using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderHandler : MonoBehaviour
{
    [SerializeField] private Transform headset;
    [SerializeField] private CapsuleCollider playAreaCapsuleCollider;
    [SerializeField] private LocomotionManager locomotionManager;
    

    private void FixedUpdate()
    {
        AdjustCapsule();
    }

    private void AdjustCapsule() 
    {
        var headsetLocalPosition = headset.localPosition;
        playAreaCapsuleCollider.height = headsetLocalPosition.y + locomotionManager.heightAdjustHeightOffset;
        playAreaCapsuleCollider.center = new Vector3(headsetLocalPosition.x, 
            playAreaCapsuleCollider.height / 2, headsetLocalPosition.z);
    }
}
