using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/ForcePushData", order = 0)]
public class ForcePushData : ScriptableObject
{
    public float startUseHapticAmplitude = .5f;
    public float startUseHapticDuration = .2f;
    public float range = 10;
    public float radius = .5f;
    public float force = 200;
    public float hover = 20;
    public float unpin = 50f;
}