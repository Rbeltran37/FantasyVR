using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class LevelKnockout : MonoBehaviour
{
    [SerializeField] private GameObject knockoutFX;


    private void OnCollisionEnter(Collision other)
    {
        //TODO possibly check tag
        var damageReceived = other.gameObject.GetComponent<DamageReceived>();
        if (damageReceived && !damageReceived.GetHasBeenInstantKilled())
        {
            var puppetDeathHandler = damageReceived.puppetDeathHandler;
            if (puppetDeathHandler)
            {
                knockoutFX.transform.position = puppetDeathHandler.GetHipsPosition();
                knockoutFX.SetActive(false);
                knockoutFX.SetActive(true);
                
                puppetDeathHandler.InstantKill();
            }
        }
    }
}
