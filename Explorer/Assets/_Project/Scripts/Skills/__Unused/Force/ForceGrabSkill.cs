using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGrabSkill : Skill
{
    [SerializeField] private ForceGrab forceGrab;
    

    public override void StartUse()
    {
        if (forceGrab)
            forceGrab.StartUse();
        
        base.StartUse();
    }

    public override void EndUse()
    {
        if (forceGrab)
            forceGrab.EndUse();
    }

    public override void Equip()
    {
        return;
    }
}
