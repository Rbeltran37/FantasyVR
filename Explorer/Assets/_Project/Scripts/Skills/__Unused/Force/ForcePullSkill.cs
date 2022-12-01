using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePullSkill : Skill
{
    [SerializeField] private ForcePull forcePull;

    
    public override void StartUse()
    {
        if (forcePull)
            forcePull.StartUse();
        
        base.StartUse();
    }

    public override void EndUse()
    {
        if (forcePull)
            forcePull.EndUse();
    }
}
