using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class LightningBallInstance : Projectile
{
    [SerializeField] private ShockPuppetSO shockPuppetSo;
    

    protected override void OnTriggerEnter(Collider other) 
    {
        var broadcaster = other.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster)
        {
            ShockMuscles(broadcaster);
            ShockFx(broadcaster);
        }
        
        base.OnTriggerEnter(other);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        var broadcaster = other.collider.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster) 
        {
            ShockMuscles(broadcaster);
            ShockFx(broadcaster);
        }
        
        base.OnCollisionEnter(other);
    }

    private void ShockMuscles(MuscleCollisionBroadcaster muscleCollisionBroadcaster)
    {
        if (DebugLogger.IsNullError(muscleCollisionBroadcaster, this)) return;
        if (DebugLogger.IsNullError(shockPuppetSo, this, "Must be set in editor.")) return;

        shockPuppetSo.ApplyShock(muscleCollisionBroadcaster);
    }

    private void ShockFx(MuscleCollisionBroadcaster muscleCollisionBroadcaster)
    {
        if (DebugLogger.IsNullError(muscleCollisionBroadcaster, this)) return;
        if (DebugLogger.IsNullError(shockPuppetSo, this, "Must be set in editor.")) return;

        var puppetMaster = muscleCollisionBroadcaster.puppetMaster;
        if (DebugLogger.IsNullError(puppetMaster, this)) return;

        var shockEffect = puppetMaster.GetComponent<ShockEffect>();
        if (DebugLogger.IsNullDebug(shockEffect, this)) return;

        shockEffect.ApplyShockEffect(shockPuppetSo);
    }
}
