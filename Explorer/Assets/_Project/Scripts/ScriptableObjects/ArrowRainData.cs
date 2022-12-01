using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Skills/ArrowRainData", order = 0)]
public class ArrowRainData : ScriptableObject
{
    [Range(0, 1.0f)] public float arrowPullValue = 1;
    public float spawnHeightOffset = 20f;
    [Range(.1f, 3)] public float positionOffsetValue = 1;
    [MinMaxFloatRange(0, .25f)] public RangedFloat arrowSpawnInterval;
    public Quaternion targetRotation = Quaternion.Euler(90, 0, 0);
}