using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class ThrownWeapon : PooledRigidbody
{
    [SerializeField] protected Rigidbody[] WeaponRigidbodies;
    [SerializeField] protected Collider[] WeaponColliders;
    [SerializeField] protected SimpleAudioEvent ThrowAudioEvent;
    [SerializeField] protected SimpleHapticEvent ThrowHapticEvent;
    [SerializeField] protected SimpleHapticEvent ReturnHapticEvent;
    [SerializeField] protected AudioSource AudioSource;
    [SerializeField] protected LayerMask StickLayers;

    [SerializeField] private SkillModifierContainer skillModifierContainer;

    protected bool IsThrown;
    protected bool IsReturning;
    protected bool IsStuck;
    protected float ThrowSpeed;
    protected Transform ReturnTarget;
    protected PlayerHand PlayerHand;
    
    public Action HasReturned;
    
    private const float HAS_REACHED_TARGET = .1f;
    private const int IS_NEAR_TARGET = 2;
    
    [System.Serializable]
    public class ActivationEvent : UnityEvent<Vector3> { }
    
    public ActivationEvent HasStuck;
    public ActivationEvent HasHit;


    protected override void OnEnable()
    {
        base.OnEnable();

        if (!ThisPhotonView.IsMine)
        {
            enabled = false;
            return;
        }
        
        Throw();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        ResetWeapon();
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        HasHit?.Invoke(other.GetContact(0).point);
    }

    protected void Throw()
    {
        IsThrown = true;

        SetTriggers(false);
        SetKinematic(false);
        PlayThrowFx();
    }

    protected virtual void ResetWeapon()
    {
        IsReturning = false;
        IsThrown = false;
        IsStuck = false;

        ReturnTarget = null;
        
        StopRigidbody();
    }
    
    public virtual void StartReturn()
    {
        IsReturning = true;
        IsStuck = false;
        
        StopRigidbody();
        SetKinematic(false);
        PlayThrowFx();
    }

    protected void PlayThrowFx()
    {
        ThrowAudioEvent.Play(AudioSource);
        if (PlayerHand) ThrowHapticEvent.Play(PlayerHand.GetControllerHaptics());
    }
    
    protected void StopRigidbody()
    {
        ThisRigidbody.velocity = Vector3.zero;
        ThisRigidbody.angularVelocity = Vector3.zero;
    }
    
    
    protected bool IsNearTarget(float distance)
    {
        return distance < IS_NEAR_TARGET;
    }

    protected bool HasReachedTarget(float distance) 
    {
        return distance < HAS_REACHED_TARGET;
    }

    public void FinishReturn()
    {
        if (IsThrown) HasReturned?.Invoke();
        
        PlayFinishReturnFx();
        Despawn();
    }
    
    private void PlayFinishReturnFx()
    {
        ReturnHapticEvent.Play(PlayerHand.GetControllerHaptics());
    }
    
    public void SetPlayerHand(Skill skill)
    {
        PlayerHand = skill.GetPlayerHand();
    }
    
    public void SetReturnTarget(Transform target)
    {
        ReturnTarget = target;
    }
    
    public void ApplyModifiers(ModifierData modifierData)
    {
        skillModifierContainer.ApplyModifiers(modifierData);
    }
    
    protected bool IsInLayerMask(GameObject gameObjectToCheck, LayerMask layerMask)
    {
        return (layerMask.value & (1 << gameObjectToCheck.layer)) > 0;
    }

    public virtual void SetThrowSpeed(float speed)
    {
        ThrowSpeed = speed;
    }
    
    protected void Stick()
    {
        if (IsReturning) return;
        
        if (!IsStuck) HasStuck?.Invoke(ThisTransform.position);

        IsStuck = true;
        
        StopRigidbody();
        SetKinematic(true);
        
        AudioSource.Stop();
    }

    private void SendStick()
    {
        if (!ThisPhotonView.IsMine) return;
        
        ThisPhotonView.RPC(nameof(RPCStick), RpcTarget.Others);
    }

    [PunRPC]
    protected void RPCStick()
    {
        Stick();
    }

    private void SetKinematic(bool state)
    {
        ThisRigidbody.isKinematic = state;

        foreach (var weaponRigidbody in WeaponRigidbodies)
        {
            weaponRigidbody.isKinematic = state;
        }
    }
    
    protected void SetTriggers(bool state)
    {
        foreach (var weaponCollider in WeaponColliders)
        {
            weaponCollider.isTrigger = state;
        }
    }

    public void SetScale(Vector3 scale)
    {
        var adjustedScale = new Vector3(-scale.x, scale.y, scale.z);
        ThisTransform.localScale = adjustedScale;
    }
}
