using System;
using System.Collections;
using UnityEngine;
using RootMotion.Dynamics;

public class PuppetCollisionHandler : MonoBehaviour
{
    [SerializeField] private PuppetCollisionHandlerData puppetCollisionHandlerData;
    [SerializeField] private AnimationEventHandler animationEventHandler;
    [SerializeField] private AnimatorHitReactionHandler animatorHitReactionHandler;
    [SerializeField] private Health health;
    [SerializeField] private BehaviourPuppet behaviourPuppet;
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private PuppetLauncher puppetLauncher;
    [SerializeField] private Transform hitTransform;

    private const string SHIELD = "Shield";
    
    
    private void Start ()
    {
        SubscribeToEvents();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    protected virtual void OnCollisionImpulse(MuscleCollision m, float impulse) {

        if (m.collision.contacts.Length == 0) return;
        if (!HasMetPuppetCollisionHandlerThreshold(impulse)) return;
        if (!HasMetElementFxThreshold(impulse)) return;
        if (HasCollidedWithShield(m)) return;

        CheckForManaRecover(m);

        var currentMuscleIndex = m.muscleIndex;
        var muscle = behaviourPuppet.puppetMaster.muscles[currentMuscleIndex];

        // Do not emit blood from prop contacts with static objects
        if (WasInvalidColliderHit(m, muscle)) return;
        
        var hitPoint = GetHitPoint(m);
        var hitForward = m.collision.contacts[0].normal;
        PositionHitTransform(hitPoint, hitForward);
        
        var damageReceived = muscle.transform.GetComponent<DamageReceived>();
        
        var damage = CalculateDamage(m, impulse, muscle, damageReceived);
        //var damage = 0;     //Deal no damage from collisions
        
        var referenceForward = damageReceived.GetReferenceForward();
        var referencePoint = damageReceived.GetReferencePoint();
        DealDamage(currentMuscleIndex, damage, referenceForward, referencePoint, hitPoint);
    }
    
    public void DealNonCollisionDamage(int muscleIndex, float damage, Vector3 referenceForward, Vector3 referencePoint, Vector3 hitPoint, Vector3 hitForward)
    {
        if (puppetMaster && puppetMaster.state == PuppetMaster.State.Dead || 
            (puppetLauncher && puppetLauncher.HasBeenLaunched)) 
            return;
        
        PositionHitTransform(hitPoint, hitForward);
        DealDamage(muscleIndex, damage, referenceForward, referencePoint, hitPoint);
    }
    
    private bool GetIsPuppetUnpinned()
    {
        return behaviourPuppet.state == BehaviourPuppet.State.Unpinned;
    }

    private void PositionHitTransform(Vector3 hitPoint, Vector3 hitForward)
    {
        if (DebugLogger.IsNullError(hitTransform, this, "Must be set in editor.")) return;

        hitTransform.position = hitPoint;
        hitTransform.rotation = Quaternion.LookRotation(hitForward);
    }
    
    private void DealDamage(int muscleIndex, float damage, Vector3 referenceForward, Vector3 referencePoint, Vector3 hitPoint)
    {
        if (puppetLauncher && puppetLauncher.HasBeenLaunched) return;
        
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        
        health.Subtract(damage);    //TODO possibly remove, don't deal damage based on physics
        if (!health.isAlive)
        {
            DeathBlow(damage);
            return;
        }

        if (GetIsPuppetUnpinned())
        {
            if (DebugLogger.IsNullError(animationEventHandler, this, "Must be set in editor.")) return;

            animationEventHandler.DisableBothHitFrames();
        }

        ActivateHitReaction(muscleIndex, damage, referenceForward, referencePoint, hitPoint);
    }

    private void DeathBlow(float damage)
    {
        if (puppetLauncher) puppetLauncher.Launch(damage);
        if (DebugLogger.IsNullError(animationEventHandler, this, "Must be set in editor.")) return;

        animationEventHandler.DisableBothHitFrames();
    }

    //TODO may need to be decoupled
    private static void CheckForManaRecover(MuscleCollision m)
    {
        var manaRecover = m.collision.collider.GetComponent<ManaRecover>();
        if (manaRecover)
        {
            manaRecover.Recover();
        }
    }

    private void ActivateHitReaction(int currentMuscleIndex, float damage, Vector3 referenceForward, Vector3 referencePoint, Vector3 hitPoint)
    {
        if (DebugLogger.IsNullError(animationEventHandler, this, "Must be set in editor.")) return;
        
        animatorHitReactionHandler.SetHitReaction(currentMuscleIndex, damage, referenceForward, referencePoint, hitPoint);
    }

    private float CalculateDamage(MuscleCollision m, float impulse, Muscle muscle, DamageReceived damageReceived)
    {
        var physicsDamageDealt = m.collision.transform.GetComponent<PhysicsDamageDealt>();
        if (!physicsDamageDealt) return 0;
        
        var damageReceivedMultiplier = damageReceived.damageReceivedData.damageMultiplier;
        var damageDealtElementMultiplier = damageReceived.CalculateElementMultiplier(physicsDamageDealt.GetElement());
        var damage = physicsDamageDealt.GetDamageMultiplier();
        var totalDamageMultiplier = damageReceivedMultiplier * damageDealtElementMultiplier * damage;

        return totalDamageMultiplier;
    }

    private Vector3 GetHitPoint(MuscleCollision m)
    {
        return m.collision.GetContact(0).point;
    }

    private static bool WasInvalidColliderHit(MuscleCollision m, Muscle muscle)
    {
        return muscle.props.@group == Muscle.Group.Prop && 
               (m.collision.collider.attachedRigidbody == null || m.collision.collider.attachedRigidbody.isKinematic);
    }

    private static bool HasCollidedWithShield(MuscleCollision m)
    {
        return m.collision.transform.CompareTag(SHIELD);    //TODO make more generic, rename
    }

    private bool HasMetPuppetCollisionHandlerThreshold(float impulse)
    {
        return impulse >= puppetCollisionHandlerData.minCollisionImpulse;
    }

    private bool HasMetElementFxThreshold(float impulse)
    {
        if (DebugLogger.IsNullError(puppetCollisionHandlerData, this, "Must be set in editor.")) return false;
        if (DebugLogger.IsNullError(puppetCollisionHandlerData.elementFxSo, this, "Must be set in editor.")) return false;

        return impulse >= puppetCollisionHandlerData.elementFxSo.minVelocityThreshold;
    }

    private void SubscribeToEvents()
    {
        if (DebugLogger.IsNullError(behaviourPuppet, this, "Must be set in editor.")) return;

        behaviourPuppet.OnCollisionImpulse += OnCollisionImpulse;
        behaviourPuppet.onLoseBalance.unityEvent.AddListener(TurnOnInternalCollisions);
        behaviourPuppet.onRegainBalance.unityEvent.AddListener(TurnOffInternalCollisions);
    }

    private void UnsubscribeFromEvents()
    {
        if (DebugLogger.IsNullError(behaviourPuppet, this, "Must be set in editor.")) return;

        behaviourPuppet.OnCollisionImpulse -= OnCollisionImpulse;
    }

    private void TurnOffInternalCollisions()
    {
        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) return;

        puppetMaster.internalCollisions = false;
    }
    
    private void TurnOnInternalCollisions()
    {
        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) return;

        puppetMaster.internalCollisions = true;
    }

    public void ResetObject()
    {
        SubscribeToEvents();
    }
}
