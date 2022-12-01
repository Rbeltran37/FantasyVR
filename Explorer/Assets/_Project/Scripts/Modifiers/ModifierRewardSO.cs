using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "ModifierReward", menuName = "ScriptableObjects/Modifiers/ModifierReward", order = 1)]
public class ModifierRewardSO : ScriptableObject
{
    public ModifierTypeSO ModifierTypeSo;
    [SerializeField] private ModifierLevel level = ModifierLevel.One;
    public int ModifierLevelValue => (int)level;
    
    private enum ModifierLevel
    {
        NegativeTwo = -2,
        NegativeOne = -1,
        One = 1,
        Two = 2,
        Three = 3
    }

    
    public void Apply(ModifierData modifierData)
    {
        if (DebugLogger.IsNullError(modifierData, this)) return;
        
        modifierData.ModifyLevel(ModifierTypeSo, ModifierLevelValue);
    }

    public override string ToString()
    {
        var isPositive = ModifierLevelValue > 0;

        return ToString(isPositive, ModifierLevelValue.ToString());
    }
    
    protected string ToString(bool isPositive, string value)
    {
        var stringBuilder = new StringBuilder();
        
        // Rich text color
        stringBuilder.Append(ModifierTypeSo.Name);
        stringBuilder.Append(": ");
        
        var signString = GetSignString(isPositive);
        stringBuilder.Append(signString);
        stringBuilder.Append(value);
        
        return stringBuilder.ToString();
    }

    private string GetSignString(bool isPositive)
    {
        return isPositive ? "+" : "";
    }
}
