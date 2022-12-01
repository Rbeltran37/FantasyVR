using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SkillInstanceData : SkillData
{
    private int _power;
    private int _speed;
    private float _powerIncrement;
    private float _speedIncrement;
    private float _powerMin;
    private float _speedMin;
    

    public SkillInstanceData(Skill skill) : base(skill)
    {
        Skill = skill;
        /*var power = Skill.SkillSo.Power;
        var speed = Skill.SkillSo.Speed;
        
        _power = power.BaseMultiplier;
        _speed = speed.BaseMultiplier;

        _powerIncrement = power.Increment;
        _powerMin = power.MinValue;

        _speedIncrement = speed.Increment;
        _speedMin = speed.MinValue;*/
    }

    public void ModifyPower(int modValue)
    {
        _power += modValue;
        _power = Mathf.Clamp(_power, MIN_VALUE, MAX_VALUE);
    }
    
    public void ModifySpeed(int modValue)
    {
        _speed += modValue;
        _speed = Mathf.Clamp(_speed, MIN_VALUE, MAX_VALUE);
    }

    public int GetPower()
    {
        return Mathf.RoundToInt(_powerIncrement * _power + _powerMin);
    }

    public int GetSpeed()
    {
        return Mathf.RoundToInt(_speedIncrement * _speed + _speedMin);
    }
    
    public float GetCastingSpeed()
    {
        return (MAX_VALUE - _speed) * _speedIncrement + _speedMin;
    }
}
