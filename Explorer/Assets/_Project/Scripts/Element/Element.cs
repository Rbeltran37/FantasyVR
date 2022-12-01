using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : ScriptableObject
{
    [SerializeField] private List<Element> effectiveAgainst = new List<Element>();
    [SerializeField] private List<Element> ineffectiveAgainst = new List<Element>();
    [SerializeField] private List<Element> normalAgainst = new List<Element>();

    private Dictionary<Element, Effectiveness> _effectivenessDictionary;
    
    public enum Effectiveness
    {
        Effective,
        Ineffective,
        Normal
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        _effectivenessDictionary = new Dictionary<Element, Effectiveness>();
        
        foreach (var element in effectiveAgainst)
        {
            if (!_effectivenessDictionary.ContainsKey(element))
                _effectivenessDictionary.Add(element, Effectiveness.Effective);
        }

        foreach (var element in ineffectiveAgainst)
        {
            if (!_effectivenessDictionary.ContainsKey(element))
                _effectivenessDictionary.Add(element, Effectiveness.Ineffective);
        }

        foreach (var element in normalAgainst)
        {
            if (!_effectivenessDictionary.ContainsKey(element)) 
                _effectivenessDictionary.Add(element, Effectiveness.Normal);
        }
    }

    public Effectiveness GetEffectiveness(Element otherElement)
    {
        if (!otherElement)
        {
            DebugLogger.Error(nameof(GetEffectiveness), $"{nameof(otherElement)} is null.", this);
            return Effectiveness.Normal;
        }
        
        if (_effectivenessDictionary == null) Initialize();

        if (!_effectivenessDictionary.ContainsKey(otherElement))
        {
            DebugLogger.Error(nameof(GetEffectiveness), $"{name} {nameof(_effectivenessDictionary)} does not contain key={otherElement}.", this);
            return Effectiveness.Normal;
        }
        
        return _effectivenessDictionary[otherElement];
    }
}
