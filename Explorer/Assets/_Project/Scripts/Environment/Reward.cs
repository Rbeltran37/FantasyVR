using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reward : PooledObject
{
    [SerializeField] private ModifierTypeUI modifierTypeUI;
    [SerializeField] private ModifierEffectUI modifierEffectUI;

    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private bool _canBeUsed;

    public Action RewardWasUsed;
    

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        _defaultPosition = ThisTransform.position;
        _defaultRotation = ThisTransform.rotation;
    }

    protected override void OnEnable()
    {
        _canBeUsed = true;
        _defaultPosition = ThisTransform.position;
        _defaultRotation = ThisTransform.rotation;
        
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        _canBeUsed = false;
        
        base.OnDisable();
    }

    protected void ActivateTypeUI()
    {
        modifierTypeUI.ActivateUI();
    }
    
    protected void DeactivateTypeUI()
    {
        modifierTypeUI.DeactivateUI();
    }
    
    protected void SetAndActivateEffectUI(Dictionary<ModifierTypeSO, int> modifiers, HolsterModifierContainer holsterModifierContainer)
    {
        modifierEffectUI.SetUI(modifiers, holsterModifierContainer);
        modifierEffectUI.ActivateUI();
    }
    
    protected void DeactivateEffectUI()
    {
        modifierEffectUI.DeactivateUI();
    }
    
    protected void ResetPositionAndRotation()
    {
        var thisTransform = transform;
        thisTransform.position = _defaultPosition;
        thisTransform.rotation = _defaultRotation;
    }

    public virtual void Clear()
    {
        Despawn();
    }

    protected bool CanBeUsed()
    {
        return _canBeUsed;
    }
    
    protected virtual void Use()
    {
        RewardWasUsed?.Invoke();
        
        ResetPositionAndRotation();
    }

    protected void SetModifierTypeUI(Dictionary<ModifierTypeSO, int> modifiers)
    {
        if (DebugLogger.IsNullError(modifierTypeUI, this, "Must be set in editor.")) return;

        modifierTypeUI.SetUI(modifiers, null);
    }
}
