using UnityEngine;

[CreateAssetMenu(fileName = "DropGearData", menuName = "ScriptableObjects/SharedData/DropGearData", order = 0)]
public class DropGearSO : ScriptableObject
{
        [MinMaxFloatRange(0,20)] public RangedFloat upwardDropForce;
}