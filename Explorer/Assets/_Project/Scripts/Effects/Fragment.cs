using System;
using System.Collections;
using System.Collections.Generic;
using RayFire;
using Sirenix.OdinInspector;
using UnityEngine;

public class Fragment : MonoBehaviour
{
    [SerializeField] private DissolveEffect dissolveEffect;

    private RayfireRigid _rayfireRigid;
    

    [Button]
    public void Initialize()
    {
        if (!dissolveEffect)
        {
            dissolveEffect = GetComponent<DissolveEffect>();
            if (!dissolveEffect)
            {
                dissolveEffect = gameObject.AddComponent<DissolveEffect>();
            }
        }
    }

    private void Start()
    {
        _rayfireRigid = GetComponent<RayfireRigid>();
        if (!_rayfireRigid)
        {
            DebugLogger.Error(nameof(Start), $"{nameof(_rayfireRigid)} is null. Was not found on {name}.", this);
            return;
        }

        _rayfireRigid.activationEvent.LocalEvent += DissolveFragment;

        if (!dissolveEffect)
        {
            
        }

        dissolveEffect.FinishedDissolving += Disable;
    }

    private void DissolveFragment(RayfireRigid rayfireRigid)
    {
        if (!rayfireRigid)
        {
            DebugLogger.Error(nameof(DissolveFragment), $"{nameof(rayfireRigid)} is null.", this);
            return;
        }

        if (!_rayfireRigid)
        {
            DebugLogger.Error(nameof(DissolveFragment), $"{nameof(_rayfireRigid)} is null.", this);
            return;
        }

        if (!rayfireRigid.Equals(_rayfireRigid))
        {
            DebugLogger.Error(nameof(DissolveFragment), $"{nameof(rayfireRigid)}={rayfireRigid} is not equal to {nameof(_rayfireRigid)}={_rayfireRigid}.", this);
            return;
        }
        
        dissolveEffect.Dissolve();
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
