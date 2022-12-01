using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Localized : Skill
{
    [SerializeField] private TargetAcquisition targetAcquisition;
    [SerializeField] private ObjectPool targetingObjectPool;

    protected LocalizedTarget LocalizedTarget;

    private bool _isTargeting;
    private Vector3 _targetPosition = Vector3.zero;
    
    private const float GROUND_OFFSET = .01f;

    
    public override void Setup(PlayerHand playerHand, ManaPool manaPool)
    {
        base.Setup(playerHand, manaPool);

        if (DebugLogger.IsNullError(targetAcquisition, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(targetingObjectPool, this, "Must be set in editor.")) return;

        targetingObjectPool.InitializePool();
            
        if (!LocalizedTarget)
        {
            var pooledObject = targetingObjectPool.GetPooledObject();
            if (DebugLogger.IsNullError(pooledObject, this)) return;

            LocalizedTarget = (LocalizedTarget) pooledObject;
            if (DebugLogger.IsNullError(LocalizedTarget, this)) return;
        }
    }

    public override void StartUse()
    {
        base.StartUse();
        
        StartCoroutine(TargetingCoroutine());
    }

    public override void EndUse()
    {
        _isTargeting = false;
        CanCastPhased = false;
        
        LocalizedTarget.Despawn();
        
        base.EndUse();
    }

    public override void Cast()
    {
        if (DebugLogger.IsNullError(LocalizedTarget, this, "Must be set in editor.")) return;
        
        LocalizedTarget.Despawn();
        
        if (_targetPosition == Vector3.zero) return;
        
        base.Cast();
    }

    protected override void SpawnSkillInstance(SkillInstance skillInstance)
    {
        var rotation = LocalizedTarget.GetTransform().rotation;
        var position = LocalizedTarget.GetTransform().position;
        
        skillInstance.Spawn(rotation, position, true);
    }

    private IEnumerator TargetingCoroutine()
    {
        _isTargeting = true;
        _targetPosition = Vector3.zero;

        if (DebugLogger.IsNullError(LocalizedTarget, this)) yield break;

        LocalizedTarget.Spawn();

        while (_isTargeting)
        {
            if (CanCastPhased) LocalizedTarget.CanCast();
            
            _targetPosition = targetAcquisition.AcquireHitPoint(ThisTransform);
            if (_targetPosition != Vector3.zero)
            {
                LocalizedTarget.GetTransform().position = _targetPosition + (Vector3.up * GROUND_OFFSET);
                LocalizedTarget.GetTransform().rotation = ThisTransform.rotation;
                LocalizedTarget.GetTransform().localEulerAngles = new Vector3(0, LocalizedTarget.GetTransform().localEulerAngles.y, 0);
                
                if (!LocalizedTarget.isActiveAndEnabled) LocalizedTarget.Spawn();   //TODO isActiveAndEnabled may not be what's needed to be checked
            }
            else
            {
                LocalizedTarget.Despawn();
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
}
