using System;
using System.Collections.Generic;
using System.Reflection;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;

public class DamageReceived : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Transform hitReferenceTransform;
    [SerializeField] private Vector3 referenceAdjustment;

    public DamageReceivedData damageReceivedData;
    public PuppetCollisionHandler puppetCollisionHandler;
    public PuppetDeathHandler puppetDeathHandler;
    public MuscleCollisionBroadcaster muscleCollisionBroadcaster;
    
    private int _muscleIndex;
    private Dictionary<Element.Effectiveness, float> _multiplierDictionary;

    private const float INEFFECTIVE = .5F;
    private const float NORMAL = 1;
    private const float EFFECTIVE = 1.5F;


    private void Awake()
    {
        if (DebugLogger.IsNullWarning(hitReferenceTransform, this, "Should be set in editor.")) hitReferenceTransform = transform;
        if (puppetCollisionHandler &&
            DebugLogger.IsNullWarning(puppetDeathHandler, this, "Should be set in editor. Attempting to find..."))
        {
            var firstParent = hitReferenceTransform.parent;
            if (firstParent)
            {
                var secondParent = firstParent.parent;
                if (secondParent) puppetDeathHandler = secondParent.GetComponent<PuppetDeathHandler>();
            }

            if (DebugLogger.IsNullError(puppetDeathHandler, this, "Should be set in editor. Unable to find.")) return;
        }
        
        if (puppetDeathHandler && DebugLogger.IsNullError(puppetCollisionHandler, this, "Must be set in editor.")) return;
        
        _multiplierDictionary = new Dictionary<Element.Effectiveness, float>
        {
            { Element.Effectiveness.Ineffective, INEFFECTIVE },
            { Element.Effectiveness.Normal, NORMAL },
            { Element.Effectiveness.Effective, EFFECTIVE },
        };
    }

    private void Start()
    {
        muscleCollisionBroadcaster = GetComponent<MuscleCollisionBroadcaster>();
        if (muscleCollisionBroadcaster)
        {
            _muscleIndex = muscleCollisionBroadcaster.muscleIndex;
        }
    }

    public void TakeDamage(float amount, Element attackElement, bool isOwner)
    {
        if (!health || !damageReceivedData) return;

        var multiplier = isOwner ? 1 : damageReceivedData.damageMultiplier;
        if (damageReceivedData.element && attackElement)
        {
            multiplier *= CalculateElementMultiplier(attackElement);
        }
        
        health.Subtract(amount * multiplier);
    }

    public float CalculateElementMultiplier(Element attackElement)
    {
        if (DebugLogger.IsNullError(attackElement, this)) return NORMAL;
        if (DebugLogger.IsNullError(damageReceivedData, this)) return NORMAL;
        if (DebugLogger.IsNullError(damageReceivedData.element, this)) return NORMAL;

        var defenseElement = damageReceivedData.element;
        var effectiveness = attackElement.GetEffectiveness(defenseElement);
        return _multiplierDictionary[effectiveness];
    }

    public void DealNonCollisionDamage(float damage, Vector3 hitPoint, Vector3 hitForward)
    {
        if (DebugLogger.IsNullError(puppetCollisionHandler, this, "Must be set in editor.")) return;
        
        var referencePoint = GetReferencePoint();
        var referenceForward = GetReferenceForward();
        puppetCollisionHandler.DealNonCollisionDamage(_muscleIndex, damage, referenceForward, referencePoint, hitPoint, hitForward);
    }

    public Health GetOwner()
    {
        return health;
    }

    public Vector3 GetReferencePoint()
    {
        var adjustmentZ = hitReferenceTransform.forward * referenceAdjustment.z;
        return hitReferenceTransform.position + adjustmentZ;
    }

    public Vector3 GetReferenceForward()
    {
        var forward = hitReferenceTransform.forward;
        var adjustmentZ = forward * referenceAdjustment.z;
        return forward + adjustmentZ;
    }

    public bool GetHasBeenInstantKilled()
    {
        if (DebugLogger.IsNullError(puppetDeathHandler, this, "Must be set in editor.")) return true;
        
        return puppetDeathHandler.GetHasBeenInstantKilled();
    }
}