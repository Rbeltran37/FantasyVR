using UnityEngine;

[CreateAssetMenu(fileName = "ForceGrabData", menuName = "ScriptableObjects/Skills/ForceGrabData", order = 1)]
public class ForceGrabData : ScriptableObject
{
    public float startUseHapticAmplitude = .4f;
    public float startUseHapticDuration = .1f;
    public float unpin = 1000;
    public float unpinVectorLength = 5;
    public float maxGrabTime = 3;
    [Range(0, 3)] public float liftHeight = 1;
    public float handLaunchDistance = .5f;
    public float launchForce = 600;
    public LayerMask groundLayers;
}