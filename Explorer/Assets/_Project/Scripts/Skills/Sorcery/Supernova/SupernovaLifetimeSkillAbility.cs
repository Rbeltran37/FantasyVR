using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupernovaLifetimeSkillAbility : SkillAbility
{
    [SerializeField] private SupernovaInstance supernovaInstance;


    private void OnEnable()
    {
        Activate();
    }

    protected override void Activate()
    {
        supernovaInstance.SetDamageLifetime(Value);
    }
}
