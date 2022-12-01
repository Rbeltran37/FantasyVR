using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/WoodenBowData", order = 1)]
public class BowSO : ScriptableObject
{
    public ObjectPool ArrowObjectPool;
    public AudioClip releaseClip;
    public AudioClip drawBackClip;
    public float grabThreshold = 0.15f;
    public float releaseThreshold = 0.25f;
    public int numHapticPoints = 10;
    public float arrowRespawnDelay = 0.25f;
    [MinMaxFloatRange(0, 3)] public RangedFloat drawStringPitch;
    [Range(0, 1)] public float arrowSpawnLocalZPosition = 0.425f;
    [Range(0, 1)] public float arrowKnockingAmplitude = .05f;
    [Range(0, 1)] public float arrowKnockingDuration = .05f;
    [Range(0, 1)] public float drawStringHapticAmplitude = .05f;
    [Range(0, 1)] public float drawStringHapticDuration = .05f;
    [Range(0, 1)] public float fullDrawHapticAmplitude = .05f;
    [Range(0, 1)] public float fullDrawHapticDuration = .05f;
    [Range(0, 1)] public float arrowReleaseHapticAmplitude = .2f;
    [Range(0, 1)] public float arrowReleaseHapticDuration = .5f;
    public string pullAnimationParameterName = "Blend";
    public int pullAnimationParameterId = 0;


    public void InitializeAnimationIds()
    {
        pullAnimationParameterId = Animator.StringToHash(pullAnimationParameterName);
    }
}
