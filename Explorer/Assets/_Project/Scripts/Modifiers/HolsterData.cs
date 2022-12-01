using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolsterData : SkillData
{
    private int _cooldown;
    private int _cost;
    private int _count;
    private float _cooldownIncrement;
    private float _costIncrement;
    private float _countIncrement;
    private float _cooldownMin;
    private float _costMin;
    private float _countMin;

    
    public HolsterData(Skill skill) : base(skill)
    {
        Skill = skill;
        /*var count = Skill.SkillSo.Count;
        var cooldown = Skill.SkillSo.Cooldown;
        var cost = Skill.SkillSo.Cost;
        
        _count = count.BaseMultiplier;
        _cooldown = cooldown.BaseMultiplier;
        _cost = cost.BaseMultiplier;

        _cooldownIncrement = cooldown.Increment;
        _cooldownMin = cooldown.MinValue;

        _costIncrement = cost.Increment;
        _costMin = cost.MinValue;

        _countIncrement = count.Increment;
        _countMin = count.MinValue;*/
    }
    
    public void ModifyCount(int modValue)
    {
        _count += modValue;
        _count = Mathf.Clamp(_count, MIN_VALUE, MAX_VALUE);
    }
    
    public void ModifyCooldown(int modValue)
    {
        _cooldown += modValue;
        _cooldown = Mathf.Clamp(_cooldown, MIN_VALUE, MAX_VALUE);
    }
    
    public void ModifyCost(int modValue)
    {
        _cost += modValue;
        _cost = Mathf.Clamp(_cost, MIN_VALUE, MAX_VALUE);
    }

    public float GetCooldown()
    {
        return _cooldownIncrement * _cooldown + _cooldownMin;
    }

    public int GetCost()
    {
        return Mathf.RoundToInt(_costIncrement * _cost + _costMin);
    }

    public int GetCount()
    {
        return Mathf.RoundToInt(_countIncrement * _count + _countMin);
    }
}
