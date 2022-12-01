using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TextRendererParticleSystem textRendererParticleSystem;
    [SerializeField] private Transform thisTransform;
    [SerializeField] private float textStartSize = 5;
    


    private void Awake()
    {
        if (!thisTransform)
        {
            DebugLogger.Warning(nameof(Awake), $"{nameof(thisTransform)} is null. Should be set in editor.", this);
            thisTransform = transform;
        }
        
        ResetObject();
    }

    private void OnEnable()
    {
        ResetObject();
    }

    [Button]
    private void Display(float damage)
    {
        if (!textRendererParticleSystem)
        {
            DebugLogger.Error(nameof(Display), $"{nameof(textRendererParticleSystem)} is null. Must be set in editor.", this);
            return;
        }
        
        textRendererParticleSystem.SpawnParticle(thisTransform.position, -damage, Color.red, textStartSize);
    }

    private void ResetObject()
    {
        if (!health)
        {
            DebugLogger.Error(nameof(ResetObject), $"{nameof(health)} is null. Must be set in editor.", this);
            return;
        }

        if (!healthBar)
        {
            DebugLogger.Error(nameof(ResetObject), $"{nameof(healthBar)} is null. Must be set in editor.", this);
            return;
        }
        
        health.ResetObject();
        healthBar.ResetObject();
        
        health.WasHit += Display;
        health.WasKilled += ResetObject;
    }
}
