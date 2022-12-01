using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private DamageDealt damageDealt;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private SimpleAudioEvent simpleAudioEvent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float damageColliderLifetime = .5f;
    [SerializeField] private float baseDamage = 10;
    [SerializeField] private ElementFxSO elementFxSo;       //For lookup purposes

    private GameObject _thisGameObject;


    private void Awake()
    {
        _thisGameObject = gameObject;
    }

    private void OnEnable()
    {
        StartCoroutine(ExplosionCoroutine());
    }

    private IEnumerator ExplosionCoroutine() 
    {
        if (DebugLogger.IsNullError(sphereCollider, "Must be set in editor.", this)) yield break;
        if (DebugLogger.IsNullError(simpleAudioEvent, "Must be set in editor.", this)) yield break;
        if (DebugLogger.IsNullError(audioSource, "Must be set in editor.", this)) yield break;
        
        simpleAudioEvent.Play(audioSource);

        sphereCollider.enabled = true;
        yield return new WaitForSeconds(damageColliderLifetime);
        sphereCollider.enabled = false;
    }
    
    public void SetValue(float value)
    {
        if (DebugLogger.IsNullError(damageDealt, "Must be set in editor.", this)) return;

        damageDealt.SetDamage(value * baseDamage);
    }

    public ElementFxSO GetElementFxSo()
    {
        return elementFxSo;
    }

    public GameObject GetGameObject()
    {
        if (!_thisGameObject) _thisGameObject = gameObject;
        
        return _thisGameObject;
    }
}
