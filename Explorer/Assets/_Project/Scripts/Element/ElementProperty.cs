using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "_ElementProperty", menuName = "ScriptableObjects/Element/ElementPropertyData", order = 1)]
public class ElementProperty : ScriptableObject
{
    public Element element;
    public ElementFxSO elementFxSo;       //For lookup purposes
    public bool emitsOut;

    public Element.Effectiveness GetEffectiveness(Element otherElement)
    {
        if (DebugLogger.IsNullError(element, this, "Must be set in editor.")) return Element.Effectiveness.Normal;

        return element.GetEffectiveness(otherElement);
    }
}