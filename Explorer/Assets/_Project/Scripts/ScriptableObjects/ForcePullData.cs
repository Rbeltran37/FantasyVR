using UnityEngine;

[CreateAssetMenu(fileName = "ForcePullData", menuName = "ScriptableObjects/Skills/ForcePullData", order = 0)]
public class ForcePullData : ScriptableObject
{
    public float startUseHapticAmplitude = .4f;
    public float startUseHapticDuration = .1f;
    public float pullSpeed = 12;
    public float unpin = 1000;
    public float maxPullTime = 3;
    public float unpinVectorLength = 5;
    public float stabilizingDistance = .5f;
    [Range(0, 3)] public float liftHeight = 1.1f;
    public float targetDistance = .5f;
}