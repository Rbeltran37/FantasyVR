using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/ForceRepulseData", order = 0)]
public class ForceRepulseData : ScriptableObject
{
    public float startUseHapticAmplitude = .6f;
    public float startUseHapticDuration = .2f;
    public float radius = 2.5f;
    public float force = 100;
    public float cap = 200;
    public float hover = 10;
    public float unpin = 100f;
}