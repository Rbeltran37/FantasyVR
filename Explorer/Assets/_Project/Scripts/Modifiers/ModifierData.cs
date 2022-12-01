using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierData
{
    private Dictionary<ModifierTypeSO, int> _currentLevels;
    private Dictionary<ModifierTypeSO, float> _currentValues;
    private Dictionary<ModifierTypeSO, Dictionary<int, float>> _possibleValues;
    private Dictionary<ModifierTypeSO, bool> _isReversed;

    private const int MIN_LEVEL = 0;
    private const int MAX_LEVEL = 12;
    private const int NULL_VALUE = -1;


    public ModifierData(Skill skill)
    {
        _currentLevels = new Dictionary<ModifierTypeSO, int>();
        _currentValues = new Dictionary<ModifierTypeSO, float>();
        _possibleValues = new Dictionary<ModifierTypeSO, Dictionary<int, float>>();
        _isReversed = new Dictionary<ModifierTypeSO, bool>();
        foreach (var modifiableSo in skill.SkillSo.Modifiables)
        {
            _currentLevels.Add(modifiableSo.ModifierTypeSo, modifiableSo.BaseLevel);
            _currentValues.Add(modifiableSo.ModifierTypeSo, modifiableSo.Increment * modifiableSo.BaseLevel + modifiableSo.MinValue);
            _possibleValues.Add(modifiableSo.ModifierTypeSo, new Dictionary<int, float>());
            for (var level = 0; level <= MAX_LEVEL; level++)
            {
                _possibleValues[modifiableSo.ModifierTypeSo].Add(level, modifiableSo.Increment * level + modifiableSo.MinValue);
            }

            _isReversed[modifiableSo.ModifierTypeSo] = modifiableSo.ReverseModifyLevel;
        }
    }

    public bool Contains(ModifierTypeSO modifierTypeSo)
    {
        if (DebugLogger.IsNullOrEmptyError(_currentLevels, "")) return false;
        
        return _currentLevels.ContainsKey(modifierTypeSo);
    }

    public bool CanUpgrade(ModifierTypeSO modifierTypeSo)
    {
        if (DebugLogger.IsNullOrEmptyError(_currentLevels, "")) return false;
        if (!_currentLevels.ContainsKey(modifierTypeSo)) return false;

        var level = _currentLevels[modifierTypeSo];
        
        return level > MIN_LEVEL && level < MAX_LEVEL;
    }

    public void ModifyLevel(ModifierTypeSO modifierTypeSo, int levelModifier)
    {
        if (DebugLogger.IsNullOrEmptyError(_currentLevels, "")) return;

        var currentLevel = _currentLevels[modifierTypeSo];
        var isReversed = _isReversed[modifierTypeSo];
        if (isReversed) levelModifier *= -1;        // Used for modifiers where lower values=better, ie. Casting, Cost, etc.
        currentLevel += levelModifier;
        currentLevel = Mathf.Clamp(currentLevel, MIN_LEVEL, MAX_LEVEL);
        
        _currentLevels[modifierTypeSo] = currentLevel;
        _currentValues[modifierTypeSo] = _possibleValues[modifierTypeSo][currentLevel];
    }

    public int GetCurrentLevel(ModifierTypeSO modifierTypeSo)
    {
        if (!Contains(modifierTypeSo)) return NULL_VALUE;

        return _currentLevels[modifierTypeSo];
    }

    public float GetCurrentValue(ModifierTypeSO modifierTypeSo)
    {
        if (!Contains(modifierTypeSo)) return NULL_VALUE;

        return _currentValues[modifierTypeSo];
    }

    public int GetCurrentValueInt(ModifierTypeSO modifierTypeSo)
    {
        if (!Contains(modifierTypeSo)) return NULL_VALUE;

        return Mathf.RoundToInt(_currentValues[modifierTypeSo]);
    }
    
    public float GetValue(ModifierTypeSO modifierTypeSo, int level)
    {
        if (!Contains(modifierTypeSo)) return NULL_VALUE;

        level = Mathf.Clamp(level, MIN_LEVEL, MAX_LEVEL);
        return _possibleValues[modifierTypeSo][level];
    }
    
    public int GetValueInt(ModifierTypeSO modifierTypeSo, int level)
    {
        if (!Contains(modifierTypeSo)) return NULL_VALUE;

        level = Mathf.Clamp(level, MIN_LEVEL, MAX_LEVEL);
        return Mathf.RoundToInt(_possibleValues[modifierTypeSo][level]);
    }

    public bool GetIsReversed(ModifierTypeSO modifierTypeSo)
    {
        if (!Contains(modifierTypeSo)) return false;
        
        return _isReversed[modifierTypeSo];
    }
}
