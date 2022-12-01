using UnityEngine;

[CreateAssetMenu(fileName = "RockLaunchData", menuName = "ScriptableObjects/Skills/RockLaunchData", order = 1)]
public class RockLaunchData : ScriptableObject
{
    public float startUseHapticAmplitude = .6f;
    public float startUseHapticDuration = .2f;
    public float afterExplodeLifetime = 1;
    public float explodeLifetime = 2;
}