using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using RootMotion.Dynamics;
using UnityEngine;

public class PUNPuppetCollisionHandler : PuppetCollisionHandler
{
    public Action<PlayerTarget, float> WasHit;
    
    
    protected override void OnCollisionImpulse(MuscleCollision m, float impulse)
    {
        var collidingTransform = m.collision.transform;
        
        if (IsOnlineAndNotMine(collidingTransform)) return;

        RegisterHitToPlayerTarget(m, impulse);

        base.OnCollisionImpulse(m, impulse);
    }

    private bool IsOnlineAndNotMine(Transform collidingTransform)
    {
        if (PhotonNetwork.OfflineMode) return false;

        var photonView = collidingTransform.GetComponent<PhotonView>();
        if (photonView) return !photonView.IsMine;
        
        var photonViewReference = collidingTransform.GetComponent<PhotonViewReference>();
        if (DebugLogger.IsNullError(photonViewReference, this)) return true;

        return !photonViewReference.IsMine;
    }
    
    private void RegisterHitToPlayerTarget(MuscleCollision m, float impulse)
    {
        var collidingTransform = m.collision.transform;
        var photonView = collidingTransform.GetComponent<PhotonView>();
        if (photonView)
        {
            var otherPlayerTarget = (PlayerTarget) photonView.Owner.TagObject;
            if (otherPlayerTarget) WasHit?.Invoke(otherPlayerTarget, impulse);
            return;
        }

        var otherPhotonViewReference = collidingTransform.GetComponent<PhotonViewReference>();
        if (otherPhotonViewReference)
        {
            var otherPlayerTarget = (PlayerTarget) otherPhotonViewReference.Owner.TagObject;
            if (otherPlayerTarget) WasHit?.Invoke(otherPlayerTarget, impulse);
            return;
        }
    }
}
