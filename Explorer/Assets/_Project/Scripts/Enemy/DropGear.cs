using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class DropGear : PooledRigidbody
{
    [SerializeField] private Collider[] gearColliders;
    
    public DissolveEffect DissolveEffect;

    private const float TRIGGER_BUFFER = .05f;


    protected override void OnEnable()
    {
        base.OnEnable();
        
        Drop();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        ResetGear();
    }

    [Button]
    public override void PopulateParameters()
    {
        base.PopulateParameters();

        var colliders = GetComponentsInChildren<Collider>();
        gearColliders = colliders.ToArray();

        if (!DissolveEffect)
        {
            DissolveEffect = GetComponent<DissolveEffect>();
            if (!DissolveEffect)
            {
                DissolveEffect = ThisGameObject.AddComponent<DissolveEffect>();
                
            }
        }

        var launchRigidbody = GetComponent<LaunchRigidbody>();
        if (!launchRigidbody)
        {
            launchRigidbody = ThisGameObject.AddComponent<LaunchRigidbody>();
            launchRigidbody.PopulateParameters();
        }

        var rotateRigidbody = GetComponent<RotateRigidbody>();
        if (!rotateRigidbody)
        {
            rotateRigidbody = ThisGameObject.AddComponent<RotateRigidbody>();
            rotateRigidbody.PopulateParameters();
        }
    }
    
    private void ResetGear()
    {
        DissolveEffect.ResetObject();
        
        foreach (var gearCollider in gearColliders)
        {
            gearCollider.isTrigger = true;
        }
    }
    
    private void Drop()
    {
        CoroutineCaller.Instance.StartCoroutine(DropCoroutine());
    }

    private IEnumerator DropCoroutine()
    {
        yield return new WaitForSeconds(TRIGGER_BUFFER);

        foreach (var gearCollider in gearColliders)
        {
            gearCollider.isTrigger = false;
        }
    }
}
