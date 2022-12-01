using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifierEffectUI : ModifierRewardUI
{
    [SerializeField] private Image mod1FillImage;
    [SerializeField] private Image mod2FillImage;
    [SerializeField] private Image mod3FillImage;

    private const float MAX_LEVEL = 12f;


    public override void SetUI(Dictionary<ModifierTypeSO, int> modifiers, HolsterModifierContainer holsterModifierContainer)
    {
        base.SetUI(modifiers, holsterModifierContainer);

        if (DebugLogger.IsNullError(textMeshPros, this, "Must be set in editor.")) return;

        var modifierTypeSos = modifiers.Keys.ToList();
        var numModifiers = modifiers.Count;
        for (int i = 0; i < numModifiers; i++)
        {
            var modifierTypeSo = modifierTypeSos[i];
            var currentLevel = holsterModifierContainer.GetCurrentLevel(modifierTypeSo);
            var modifiedLevel = holsterModifierContainer.GetModifiedLevel(modifierTypeSo, modifiers[modifierTypeSo]);
            var currentValue = holsterModifierContainer.GetCurrentValue(modifierTypeSo);
            var modifiedValue = holsterModifierContainer.GetValue(modifierTypeSo, modifiedLevel);
            var currentValueString = currentValue.ToString("F1");
            var modifiedValueString = modifiedValue.ToString("F1");
            var tempText = $"{modifierTypeSo.Name} {currentValueString} -> {modifiedValueString}";
            textMeshPros[i].text = tempText;

            Image image = null;
            switch (i)
            {
                case 0: image = mod1FillImage;
                    break;
                case 1: image = mod2FillImage;
                    break;
                case 2: image = mod3FillImage;
                    break;
                default: DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(i)}={i} has exceeded max value of 2");
                    break;
            }

            if (image)
            {
                image.fillAmount = modifiedLevel / MAX_LEVEL;
                image.color = modifierTypeSo.Color;
            }
        }
    }
}
