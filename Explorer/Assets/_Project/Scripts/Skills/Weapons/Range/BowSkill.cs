using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSkill : Weapon
{
    private BowInstance _bowInstance;
    private Transform _pullTarget;


    public override void Setup(PlayerHand playerHand, ManaPool manaPool)
    {
        base.Setup(playerHand, manaPool);

        _pullTarget = playerHand.GetPullTarget();
    }

    public override void StartUse()
    {
        if (DebugLogger.IsNullError(_bowInstance, this)) return;
        if (DebugLogger.IsNullError(_pullTarget, this)) return;
        
        _bowInstance.Pull(_pullTarget);
    }

    public override void EndUse()
    {
        if (!_bowInstance) return;

        _bowInstance.Release();
    }

    public override void Cast()
    {
        LaunchRing();
        PayCastingCost();
    }

    protected override void LaunchRing()
    {
        if (DebugLogger.IsNullError(_bowInstance, this)) return;
        
        var forwardVectorBeforeRelease = _bowInstance.GetTransform().forward;
        LaunchRing(forwardVectorBeforeRelease);
    }

    public override void Equip()
    {
        base.Equip();

        _bowInstance = (BowInstance) WeaponInstance;
        if (DebugLogger.IsNullError(_bowInstance, this)) return;
        
        _bowInstance.SetSkill(this);
        _bowInstance.Equip(PlayerHand, Count);
    }
}
