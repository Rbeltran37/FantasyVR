using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterPositioner : MonoBehaviour
{
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private Transform leftWaistHolster;
    [SerializeField] private Transform rightWaistHolster;
    [SerializeField] private Transform leftBackHolster;
    [SerializeField] private Transform rightBackHolster;
    [SerializeField] private Transform headSet;
    [SerializeField] private Transform waistHolsterParent;
    [SerializeField] private Transform backHolsterParent;

    
    private void Awake()
    {
        SetHolsterDistance();
    }

    void LateUpdate()
    {
        var headSetPosition = headSet.position;
        //waistHolsterParent.forward = headSet.forward;
        
        waistHolsterParent.position = new Vector3(headSetPosition.x, 
            headSetPosition.y + -holsterManager.waistDistanceFromHeadset, headSetPosition.z);
        waistHolsterParent.localEulerAngles = new Vector3(0, waistHolsterParent.localEulerAngles.y, 0);

        //backHolsterParent.forward = headSet.forward;
        backHolsterParent.position = new Vector3(headSetPosition.x, 
            headSetPosition.y + holsterManager.backDistanceFromHeadset, headSetPosition.z);
        backHolsterParent.localEulerAngles = new Vector3(0, backHolsterParent.localEulerAngles.y, 0);
    }

    private void SetHolsterDistance() 
    {
        leftWaistHolster.localPosition = new Vector3(-holsterManager.waistDistanceFromCenter, 0, leftWaistHolster.localPosition.z);
        rightWaistHolster.localPosition = new Vector3(holsterManager.waistDistanceFromCenter, 0, rightWaistHolster.localPosition.z);
        leftBackHolster.localPosition = new Vector3(-holsterManager.backDistanceFromCenter, 0, leftBackHolster.localPosition.z);
        rightBackHolster.localPosition = new Vector3(holsterManager.backDistanceFromCenter, 0, rightBackHolster.localPosition.z);
    }
}
