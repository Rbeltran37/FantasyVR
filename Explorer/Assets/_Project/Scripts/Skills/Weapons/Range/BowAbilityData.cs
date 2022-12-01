using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAbilityData : WeaponAbilityData
{
    public BowInstance BowInstance;
    
    public Arrow Arrow => BowInstance.GetCurrentArrow();
    public int ArrowCount => BowInstance.GetArrowCount();

    public BowAbilityData(BowInstance bowInstance) : base(bowInstance)
    {
        BowInstance = bowInstance;
    }
}
