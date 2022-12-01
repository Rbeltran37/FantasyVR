using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/DamageReceivedData", order = 1)]
public class DamageReceivedData : ScriptableObject
{
    public Element element;
    public float damageMultiplier = 1;
    public bool isPuppet = true;
}
