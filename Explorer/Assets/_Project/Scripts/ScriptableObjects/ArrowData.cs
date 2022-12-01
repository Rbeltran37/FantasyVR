using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/WoodenArrowData", order = 1)]
public class ArrowData : ScriptableObject
{
    public SimpleAudioEvent simpleAudioEvent;
    public LayerMask hittableLayers;
    public float rotationSpeed = 10;
    public float arrowLifetime = 7;
    public int numDeflections = 2;
}
