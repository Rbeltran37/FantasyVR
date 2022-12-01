using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Element/ElementFxData", order = 1)]
public class ElementFxSO : ScriptableObject
{
    [MinMaxFloatRange(1, 100)] public RangedFloat velocityRange;
    [MinMaxFloatRange(.01f, 10)] public RangedFloat sizeRange;
    [MinMaxFloatRange(1, 100)] public RangedFloat forceRange;
    public float debrisSizeMaxMultiplier = 2;
    public float debrisSpeedMaxMultiplier = 10;
    public float debrisCountMaxMultiplier = 5;
    public float dustSizeMaxMultiplier = 2;
    public float dustSpeedMaxMultiplier = 10;
    public float dustCountMaxMultiplier = 5;

    public bool debrisUseScaleAsSize;
    public bool dustUseScaleAsSize;
    public float minVelocityThreshold = 2;
}