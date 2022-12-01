using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ModifierTypeUI : ModifierRewardUI
{
    [SerializeField] private Transform uiTransform;
    [SerializeField] private Transform uiParent;

    private Vector3 _defaultLocalPosition;
    
    private const string PLUS_SIGN = "+";
    private const string NEGATIVE_SIGN = "-";
    

    protected override void Awake()
    {
        base.Awake();

        if (DebugLogger.IsNullError(uiTransform, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(uiParent, this, "Must be set in editor.")) return;

        _defaultLocalPosition = uiTransform.localPosition;
    }

    public override void ActivateUI()
    {
        base.ActivateUI();
        
        for (int i = 0; i < NumMods; i++)
        {
            ModParents[i].SetActive(true);
        }

        uiTransform.localPosition = _defaultLocalPosition;
        uiTransform.SetParent(null);
    }

    public override void DeactivateUI()
    {
        base.DeactivateUI();

        uiTransform.SetParent(uiParent);
    }

    public override void SetUI(Dictionary<ModifierTypeSO, int> modifiers, HolsterModifierContainer holsterModifierContainer)
    {
        base.SetUI(modifiers, holsterModifierContainer);

        if (DebugLogger.IsNullError(textMeshPros, this, "Must be set in editor.")) return;

        var modifierTypes = modifiers.Keys.ToList();
        var numModifiers = modifierTypes.Count;
        for (int i = 0; i < numModifiers; i++)
        {
            var modifierType = modifierTypes[i];
            var sign = i == 0 ? PLUS_SIGN : "";
            textMeshPros[i].text = $"{modifierType.Name} {sign}{modifiers[modifierType]}";
        }
    }
}
