using System;
using System.Collections;
using RootMotion.Dynamics;
using UnityEngine;

public class IceBallInstance : Projectile
{
    [SerializeField] private float slowAmount = .5f;


    protected override  void OnTriggerEnter(Collider other) {

        var broadcaster = other.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster)
        {
            var pointOfContact = other.ClosestPoint(GetTransformPosition());
            Freeze(broadcaster, pointOfContact);
        }

        base.OnTriggerEnter(other);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        var broadcaster = other.collider.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster)
        {
            var pointOfContact = other.GetContact(0).point;
            Freeze(broadcaster, pointOfContact);
        }

        base.OnCollisionEnter(other);
    }

    private Vector3 GetTransformPosition()
    {
        if (DebugLogger.IsNullWarning(ThisTransform, this, "Should be set in editor.")) ThisTransform = transform;

        return ThisTransform.position;
    }

    private void Freeze(MuscleCollisionBroadcaster muscleCollisionBroadcaster, Vector3 pointOfContact)
    {
        if (DebugLogger.IsNullError(muscleCollisionBroadcaster, this)) return;

        var puppetMaster = muscleCollisionBroadcaster.puppetMaster;
        if (DebugLogger.IsNullError(puppetMaster, this)) return;

        var freezeEffect = puppetMaster.GetComponent<FreezeEffect>();
        if (DebugLogger.IsNullDebug(freezeEffect, this)) return;
        if (freezeEffect == null) return;
        
        freezeEffect.ApplyFreezeEffect(slowAmount, pointOfContact);
    }
}
