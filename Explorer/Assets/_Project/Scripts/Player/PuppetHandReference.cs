using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PuppetHandReference : MonoBehaviour
{
    [SerializeField] private Rigidbody leftHandRigidbody;
    [SerializeField] private Rigidbody rightHandRigidbody;
    [SerializeField] private Rigidbody leftForearmRigidbody;
    [SerializeField] private Rigidbody rightForearmRigidbody;
    

    public Rigidbody GetHandRigidbody(bool isLeftHand)
    {
        return isLeftHand ? leftHandRigidbody : rightHandRigidbody;
    }
    
    public Rigidbody GetForearmRigidbody(bool isLeftHand)
    {
        return isLeftHand ? leftForearmRigidbody : rightForearmRigidbody;
    }
}
