using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemy/PuppetCollisionHandlerData", order = 1)]
public class PuppetCollisionHandlerData : ScriptableObject
{
    public ElementFxSO elementFxSo;
    public float minCollisionImpulse = 5;
}
