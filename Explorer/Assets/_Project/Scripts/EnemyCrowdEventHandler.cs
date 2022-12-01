using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrowdEventHandler : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private CrowdController crowdController;

    private void Start()
    {
        crowdController = FindObjectOfType<CrowdController>();

        if (!crowdController)
            return;
        
        health.WasKilled += crowdController.SynchronizePositiveCrowdAudio;
    }

    private void OnDisable()
    {
        if (!crowdController)
            return;
        
        health.WasKilled -= crowdController.SynchronizePositiveCrowdAudio;
    }
}
