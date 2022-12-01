using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

//TODO rename
public class HolsterModifierContainer : MonoBehaviour
{
    [SerializeField] private HolsterCooldown holsterCooldown;
    [SerializeField] private int modifierSlots = INFINITE;
    [SerializeField] private int typeModLimit = INFINITE;

    private ModifierData _modifierData;
    private Dictionary<ModifierTypeSO, int> _modifierTypeLevelMap = new Dictionary<ModifierTypeSO, int>();
    private int _numModifiers;

    private const int TYPE_UPGRADE_LIMIT = 5;
    private const int MODIFIER_SLOTS = 10;
    private const int INFINITE = -1;


    private void Awake()
    {
        InitializationCheck();
        InitializeUpgradeDictionary();
    }
    
    public void SetModifierData(Skill leftSkill, Skill rightSkill)
    {
        if (DebugLogger.IsNullError(leftSkill, this)) return;
        if (DebugLogger.IsNullError(rightSkill, this)) return;

        _modifierData = new ModifierData(leftSkill);    // Initialize if null
        leftSkill.SetModifierData(_modifierData);
        rightSkill.SetModifierData(_modifierData);
        
        InitializeUpgradeDictionary();
    }

    //TODO rewrite, check mod value
    //TODO may be obsolete
    public bool CanUpgrade(ModifierTypeSO modifierTypeSo)
    {
        if (DebugLogger.IsNullError(modifierTypeSo, nameof(CanUpgrade))) return false;

        if (modifierSlots != INFINITE && _numModifiers >= modifierSlots)        // All slots filled
        {
            DebugLogger.Info(nameof(CanUpgrade), $"Unable to Upgrade. All modifier slots are filled. {_numModifiers} >= {modifierSlots}", this);
            return false;
        }
        
        if (!_modifierTypeLevelMap.ContainsKey(modifierTypeSo)) return true;        // Hasn't been upgraded yet
        if (!_modifierData.CanUpgrade(modifierTypeSo)) return false;    // Skill Modifier data does not contain modifier type

        return typeModLimit == INFINITE || _modifierTypeLevelMap[modifierTypeSo] < typeModLimit;
    }

    [Button]
    public void AddModifier(ModifierTypeSO modifierTypeSo, int level)
    {
        if (DebugLogger.IsNullError(_modifierData, this)) return;

        _modifierData.ModifyLevel(modifierTypeSo, level);
    }
    
    public List<ModifierTypeSO> GetPossibleUpgrades()
    {
        var tempList = new List<ModifierTypeSO>();
        foreach (var upgrade in _modifierTypeLevelMap)
        {
            if (upgrade.Value < typeModLimit) tempList.Add(upgrade.Key);
        }
        
        return tempList;
    }

    private void InitializationCheck()
    {
        if (DebugLogger.IsNullInfo(holsterCooldown, this, "Should be set in editor. Attempting to find."))
        {
            holsterCooldown = GetComponent<HolsterCooldown>();
            if (DebugLogger.IsNullError(holsterCooldown, this, "Should be set in editor. Unable to find.")) return;
        }
    }

    private void InitializeUpgradeDictionary()
    {
        _modifierTypeLevelMap = new Dictionary<ModifierTypeSO, int>();

        _numModifiers = 0;
    }

    public void AddModifierTypeToDictionary(ModifierTypeSO modifierTypeSo)
    {
        if (DebugLogger.IsNullError(modifierTypeSo, this)) return;

        if (_modifierTypeLevelMap.ContainsKey(modifierTypeSo))
        {
            _modifierTypeLevelMap[modifierTypeSo]++;
        }
        else
        {
            _modifierTypeLevelMap.Add(modifierTypeSo, 1);
        }

        _numModifiers++;
    }

    private void ApplyUpgrade(ModifierTypeSO modifierTypeSo, int level)
    {
        _modifierData.ModifyLevel(modifierTypeSo, level);
    }

    public int GetCurrentLevel(ModifierTypeSO modifierTypeSo)
    {
        return _modifierData.GetCurrentLevel(modifierTypeSo);
    }

    public int GetModifiedLevel(ModifierTypeSO modifierTypeSo, int modifierLevel)
    {
        if (_modifierData.GetIsReversed(modifierTypeSo)) modifierLevel *= -1;
        
        return _modifierData.GetCurrentLevel(modifierTypeSo) + modifierLevel;
    }

    public float GetCurrentValue(ModifierTypeSO modifierTypeSo)
    {
        return _modifierData.GetCurrentValue(modifierTypeSo);
    }
    
    public int GetCurrentValueInt(ModifierTypeSO modifierTypeSo)
    {
        return _modifierData.GetCurrentValueInt(modifierTypeSo);
    }

    public float GetValue(ModifierTypeSO modifierTypeSo, int level)
    {
        return _modifierData.GetValue(modifierTypeSo, level);
    }
}
