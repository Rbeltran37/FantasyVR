using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private VRIK vrik;


    private void Awake()
    {
        if (health)
        {
            health.WasKilled += Disable;
        }
    }

    private void OnDestroy()
    {
        if (health)
        {
            health.WasKilled -= Disable;
        }
    }

    private void Disable()
    {
        if (vrik) vrik.enabled = false;
    }
}
