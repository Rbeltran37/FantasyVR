using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DamageDealt : MonoBehaviour
{
    [SerializeField] protected DamageDealtData damageDealtData;
    [SerializeField] protected DamageDealtUnpinData damageDealtUnpinData;
    
    [SerializeField] private SimpleAI simpleAi;
    [SerializeField] private Element element;

    public bool canDealDamage = true;
    
    private Dictionary<object, bool> _targets = new Dictionary<object, bool>();
    private float _damage = -1;
    private bool _resetCanDealDamage;

    public Action<Vector3> WasActivatedOnce;
    

    private void Awake()
    {
        _resetCanDealDamage = canDealDamage;
        if (damageDealtData) _damage = damageDealtData.damage;
    }

    private void OnEnable()
    {
        canDealDamage = _resetCanDealDamage;
        _targets = new Dictionary<object, bool>();
    }
    
    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage) return;
        
        var damageReceived = other.GetComponent<DamageReceived>();
        if (!damageReceived) return;
        
        if (!CanTargetBeDamaged(other, out var target, damageReceived)) return;
        
        DealDamage(damageReceived);
        RegisterDamageOnTarget(target, damageReceived);
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (!canDealDamage) return;

        var targetCollider = other.collider;
        var damageReceived = targetCollider.GetComponent<DamageReceived>();
        if (!damageReceived) return;
        
        if (!CanTargetBeDamaged(targetCollider, out var target, damageReceived)) return;
        
        if (CanBeBlocked() && CanObjectParry(other))    //TODO don't hardcode CanObjectParry
        {
            Parried();
            return;
        }

        var point = other.contacts[0].point;
        WasActivatedOnce?.Invoke(point);
        WasActivatedOnce = null;
        DealDamage(damageReceived);
        RegisterDamageOnTarget(target, damageReceived);
    }

    public void DealDamage(DamageReceived damageReceived)
    {
        if (!canDealDamage || !damageReceived) return;
        
        if (!damageReceived.damageReceivedData.isPuppet)
        {
            var isOwner = damageDealtData.damageTarget == DamageDealtData.DamageTarget.Owner;
            damageReceived.TakeDamage(_damage, element, isOwner);
            return;
        }

        var thisTransform = transform;
        var hitPoint = thisTransform.position;
        var hitForward = thisTransform.forward;
        
        ApplyUnpin(damageReceived, hitPoint, hitForward);
        ApplyDamage(damageReceived, hitPoint, hitForward);
    }

    private bool CanTargetBeDamaged(Collider targetCollider, out object target, DamageReceived damageReceived)
    {
        target = null;
        switch (damageDealtData.damageTarget)
        {
            case DamageDealtData.DamageTarget.Collider:
                target = targetCollider;
                break;
            case DamageDealtData.DamageTarget.Owner:
            {
                target = damageReceived.GetOwner();
                break;
            }
            case DamageDealtData.DamageTarget.None:
                return false;
            default:
                target = targetCollider;
                break;
        }

        if (DebugLogger.IsNullError(target, this)) return false;
        
        if (!_targets.ContainsKey(target)) _targets.Add(target, true);
        
        return _targets[target];
    }

    private void RegisterDamageOnTarget(object target, DamageReceived damageReceived)
    {
        if (damageDealtData.damageType == DamageDealtData.DamageType.SingleHit)
        {
            canDealDamage = false;
        } 
        else if (damageDealtData.damageType == DamageDealtData.DamageType.MultipleHits)
        {
            _targets[target] = true;
        }
        else if (damageDealtData.damageType == DamageDealtData.DamageType.DamageOverTime)
        {
            StartCoroutine(DamageOverTimeCoroutine(target, damageReceived));
        }
        else if (damageDealtData.damageType == DamageDealtData.DamageType.Melee)
        {
            _targets[target] = true;
            if (damageDealtData.damageInterval > Mathf.Epsilon)
            {
                StartCoroutine(DamageIntervalCoroutine());
            }
        }
    }

    private IEnumerator DamageOverTimeCoroutine(object target, DamageReceived damageReceived)
    {
        while (gameObject.activeSelf)
        {
            _targets[target] = false;
            yield return new WaitForSeconds(damageDealtData.GetDamageInterval());
            _targets[target] = true;
            DealDamage(damageReceived);
        }
    }

    private IEnumerator DamageIntervalCoroutine()
    {
        canDealDamage = false;
        yield return new WaitForSeconds(damageDealtData.GetDamageInterval());
        canDealDamage = true;
    }

    private void ApplyUnpin(DamageReceived damageReceived, Vector3 hitPoint, Vector3 hitForward)
    {
        if (DebugLogger.IsNullError(damageReceived, this)) return;
        if (DebugLogger.IsNullError(damageReceived.muscleCollisionBroadcaster, this)) return;
        if (DebugLogger.IsNullError(damageDealtUnpinData, this, "Must be set in editor.")) return;

        damageReceived.muscleCollisionBroadcaster.Hit(
            damageDealtUnpinData.GetUnpin(),
            hitForward * damageDealtUnpinData.GetForce(),
            hitPoint);
    }
    
    private void ApplyDamage(DamageReceived damageReceived, Vector3 hitPoint, Vector3 hitForward)
    {
        if (DebugLogger.IsNullError(damageReceived, this)) return;
        if (DebugLogger.IsNullError(damageReceived.puppetCollisionHandler, this)) return;
        if (DebugLogger.IsNullError(damageReceived.damageReceivedData, this)) return;

        var multiplier = damageDealtData.damageTarget == DamageDealtData.DamageTarget.Owner ? 1 : damageReceived.damageReceivedData.damageMultiplier;
        multiplier *= damageReceived.CalculateElementMultiplier(element);
        var adjustedDamage = _damage * multiplier;
        
        damageReceived.DealNonCollisionDamage(adjustedDamage, hitPoint, hitForward);
    }

    //TODO possibly refactor block and parry logic
    public void Blocked()
    {
        if (!CanBeBlocked()) return;
        
        canDealDamage = false;
    }

    public bool CanBeBlocked()
    {
        return damageDealtData.canBeBlocked;
    }

    private void Parried()
    {
        Blocked();
        if (simpleAi) simpleAi.Parried();
    }

    public Element GetElement()
    {
        return element;
    }

    private bool CanObjectParry(Collision other)
    {
        return other.transform.CompareTag("Weapon");        //TODO don't hardcode
    }
}
