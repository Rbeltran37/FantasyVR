using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Projectile : SkillInstance
{
    [SerializeField] private Rigidbody rigidbodyToLaunch;
    [SerializeField] private GameObject fxGameObject;
    [SerializeField] private SimpleAudioEvent loopingAudioEvent;
    [SerializeField] private SimpleAudioEvent impactAudioEvent;
    [SerializeField] private GameObject meshGameObject;
    [SerializeField] private float despawnDelay = .5f;
    [SerializeField] private DamageDealt damageDealt;
    [SerializeField] private Collider damageCollider;
    
    [System.Serializable]
    public class ActivationEvent : UnityEvent<Vector3> { }
    
    public ActivationEvent WasImpacted;

    protected virtual void OnTriggerEnter(Collider other)
    {
        Impact();
    }

    protected virtual  void OnCollisionEnter(Collision other)
    {
        Impact();
    }

    protected override void Initialize()
    {
        base.Initialize();

        if (DebugLogger.IsNullWarning(rigidbodyToLaunch, this, "Should be set in editor. Attempting to set."))
        {
            rigidbodyToLaunch = GetComponent<Rigidbody>();
            if (DebugLogger.IsNullError(rigidbodyToLaunch, this, "Should be set in editor.")) return;
        }

        if (DebugLogger.IsNullError(SkillAudioSource, this, "Should be set in editor.")) return;
        
        if (DebugLogger.IsNullWarning(damageDealt, this, "Must be set in editor."))
        {
            damageDealt = GetComponent<DamageDealt>();
            if (DebugLogger.IsNullError(damageDealt, "Must be set in editor.", this)) return;
        }
    }

    protected virtual void Impact()
    {
        damageCollider.enabled = false;
        
        meshGameObject.SetActive(false);
        fxGameObject.SetActive(false);
        
        SkillAudioSource.loop = false;
        impactAudioEvent.Play(SkillAudioSource);
        
        WasImpacted?.Invoke(ThisTransform.position);

        rigidbodyToLaunch.isKinematic = true;
        rigidbodyToLaunch.velocity = Vector3.zero;
        rigidbodyToLaunch.angularVelocity = Vector3.zero;
        
        Despawn(despawnDelay);
    }

    protected override void ResetObject()
    {
        base.ResetObject();
        
        if (DebugLogger.IsNullError(rigidbodyToLaunch, this)) return;
        
        rigidbodyToLaunch.isKinematic = false;
        rigidbodyToLaunch.velocity = Vector3.zero;
        rigidbodyToLaunch.angularVelocity = Vector3.zero;
        
        SkillAudioSource.loop = true;
        loopingAudioEvent.Play(SkillAudioSource);
        
        meshGameObject.SetActive(true);
        fxGameObject.SetActive(true);
        
        damageCollider.enabled = true;
    }
}
