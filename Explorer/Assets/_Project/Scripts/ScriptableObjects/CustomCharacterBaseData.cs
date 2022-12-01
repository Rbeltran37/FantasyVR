using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/CustomCharacterBaseData", order = 1)]
public class CustomCharacterBaseData : ScriptableObject
{
    [Tooltip("Multiplies gravity applied to the character even if 'Individual Gravity' is unchecked.")]
    public float gravityMultiplier = 2f; // gravity modifier - often higher than natural gravity feels right for game characters

    public float airborneThreshold = 0.6f; // Height from ground after which the character is considered airborne
    public float slopeStartAngle = 50f; // The start angle of velocity dampering on slopes
    public float slopeEndAngle = 85f; // The end angle of velocity dampering on slopes
    public float spherecastRadius = 0.1f; // The radius of sperecasting
    public LayerMask groundLayers; // The walkable layers
}
