using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Combat/DamageDealtData", order = 1)]
public class DamageDealtData : ScriptableObject
{
    public DamageTarget damageTarget = DamageTarget.Collider;
    public DamageType damageType = DamageType.SingleHit;
    public float damageInterval = ZERO;
    public bool canBeBlocked = true;
    public float damage;

    private const int ZERO = 0;


    public enum DamageTarget
    {
        None,
        Collider,
        Owner,
    }
    
    public enum DamageType
    {
        NoDamage,
        SingleHit,
        MultipleHits,
        DamageOverTime,
        Melee
    }

    public float GetDamageInterval()
    {
        return damageInterval;
    }
}
