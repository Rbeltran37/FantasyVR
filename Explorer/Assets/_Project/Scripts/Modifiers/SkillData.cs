using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillData
{
    protected Skill Skill;

    protected const int MIN_VALUE = 0;
    protected const int MAX_VALUE = 12;


    protected SkillData(Skill skill)
    {
        Skill = skill;
    }
}
