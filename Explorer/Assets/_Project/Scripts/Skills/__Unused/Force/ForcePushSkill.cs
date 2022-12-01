using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePushSkill : Skill
{
    [SerializeField] private ForcePushData forcePushData;
    [SerializeField] private ForcePush forcePush;
    
    
    public override void Setup(PlayerHand playerHand, ManaPool manaPool)
    {
        base.Setup(playerHand, manaPool);
        
        if (forcePush)
            forcePush.Setup(forcePushData);
    }

    public override void StartUse()
    {
        if (forcePush)
            forcePush.Use();
        
        base.StartUse();
    }
}
