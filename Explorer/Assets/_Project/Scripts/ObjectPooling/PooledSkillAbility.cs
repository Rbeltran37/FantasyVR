using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledSkillAbility : PooledObject
{
    protected ElementFxSO ElementFxSo;
    protected float Value = NOT_APPLIED;
    
    protected const int NOT_APPLIED = 0;
    
    
    public virtual void SetValue(float value)
    {
        if (value <= NOT_APPLIED) return;
        
        Value = value;
    }

    public virtual void SetElementFxSO(ElementFxSO elementFxSo)
    {
        ElementFxSo = elementFxSo;
    }
}
