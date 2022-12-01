using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFeedback : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSo;
    
    public ControllerHaptics controllerHaptics;
    public Rigidbody weaponRigidbody;
    public bool activateOnCollision = true;

    private bool _wasHit;


    private void OnCollisionEnter(Collision other)
    {
        if (!activateOnCollision || _wasHit || !weaponRigidbody) return;
        
        var otherRigid = other.transform.GetComponent<Rigidbody>();
        if (weaponRigidbody.velocity.magnitude > weaponSo.minHitVelocity || 
            (otherRigid && otherRigid.velocity.magnitude > weaponSo.minHitVelocity))
        {
            StartCoroutine(HitCoroutine());
        }
    }

    public void Hit()
    {
        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        _wasHit = true;
        if (weaponSo)
        {
            weaponSo.PlayHitHaptics(controllerHaptics);
        }
        yield return new WaitForSeconds(weaponSo.hitBufferTime);
        _wasHit = false;
    }
}
