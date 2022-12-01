using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private WeaponFeedback weaponFeedback;
    
    
    private void OnCollisionEnter(Collision other)
    {
        var damageDealt = other.gameObject.GetComponent<DamageDealt>();
        if (damageDealt && damageDealt.CanBeBlocked())
        {
            damageDealt.Blocked();
            
            if (weaponFeedback)
            {
                weaponFeedback.Hit();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var damageDealt = other.gameObject.GetComponent<DamageDealt>();
        if (damageDealt && damageDealt.CanBeBlocked())
        {
            damageDealt.Blocked();
            
            if (weaponFeedback)
            {
                weaponFeedback.Hit();
            }
        }
    }
}
