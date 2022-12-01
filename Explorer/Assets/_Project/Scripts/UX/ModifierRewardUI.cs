using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public abstract class ModifierRewardUI : MonoBehaviour
{
    [SerializeField] protected TMP_Text[] textMeshPros;
    [SerializeField] protected GameObject[] ModParents;

    protected int NumMods;

    

    protected virtual void Awake()
    {
        DeactivateUI();
    }

    public virtual void ActivateUI()
    {
        gameObject.SetActive(true);
    }
    
    public virtual void DeactivateUI()
    {
        gameObject.SetActive(false);
    }
    
    public virtual void Clear()
    {
        foreach (var textMeshPro in textMeshPros)
        {
            textMeshPro.text = String.Empty;
        }
        
        foreach (var mod in ModParents)
        {
            mod.SetActive(false);
        }

        NumMods = 0;
    }

    public virtual void SetUI(Dictionary<ModifierTypeSO, int> modifiers, HolsterModifierContainer holsterModifierContainer)
    {
        Clear();

        var modifierTypes = modifiers.Keys.ToList();
        var numModifiers = modifierTypes.Count;
        for (int i = 0; i < numModifiers; i++)
        {
            ModParents[i].SetActive(true);
        }

        NumMods = numModifiers;
    }
}
