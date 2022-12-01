using System;
using System.Collections;
using UnityEngine;

public class FirewallSkill : Localized
{
    private SkillModifierContainer _targetingSkillModifierContainer;
    
    public override void Setup(PlayerHand playerHand, ManaPool manaPool)
    {
        base.Setup(playerHand, manaPool);

        _targetingSkillModifierContainer = LocalizedTarget.GetComponentInChildren<SkillModifierContainer>();
        if (DebugLogger.IsNullError(_targetingSkillModifierContainer, this)) return;
    }

    public override void StartUse()
    {
        _targetingSkillModifierContainer.ApplyModifiers(ModifierData);

        base.StartUse();
    }
}