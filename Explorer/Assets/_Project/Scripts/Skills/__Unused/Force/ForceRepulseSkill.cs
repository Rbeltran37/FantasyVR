using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRepulseSkill : Skill
{
    [SerializeField] private ForceRepulseData forceRepulseData;
    [SerializeField] private ForceRepulse forceRepulse;
    
    
    public override void Setup(PlayerHand playerHand, ManaPool manaPool)
    {
       base.Setup(playerHand, manaPool);

        if (forceRepulse)
            forceRepulse.Setup(forceRepulseData);
    }

    public override void StartUse()
    {
        if (forceRepulse)
            forceRepulse.Use();
        
        base.StartUse();
    }
}
