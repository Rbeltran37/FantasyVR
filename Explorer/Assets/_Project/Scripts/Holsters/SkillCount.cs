using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillCount : MonoBehaviour
{
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private GameObject ringFX;
    [SerializeField] private Transform emitter;
    [SerializeField] private AudioSource handAudioSource;

    private Color _color;
    private int _baseCount;
    private int _currentCount = 0;
    private Stack<PooledRigidbody> _rings = new Stack<PooledRigidbody>();


    private void Awake()
    {
        objectPool.InitializePool();
    }

    [Button]
    public bool IsCountValid()
    {
        return _currentCount > 0;
    }
    
    public bool IsCountValid(int adjustment)
    {
        return _currentCount + adjustment > 0;
    }

    public void LaunchRing(Vector3 launchVector)
    {
        if (DebugLogger.IsNullOrEmptyDebug(_rings, this)) return;

        var pooledObjectRigidbody = _rings.Pop();
        if (DebugLogger.IsNullError(pooledObjectRigidbody, this)) return;

        var currentRingRigid = pooledObjectRigidbody.GetRigidbody();
        if (DebugLogger.IsNullError(currentRingRigid, this)) return;
        currentRingRigid.isKinematic = false;

        if (launchVector.Equals(Vector3.zero)) launchVector = emitter.forward;
        
        currentRingRigid.AddForce(launchVector * holsterManager.ringLaunchForce, ForceMode.Impulse);
        pooledObjectRigidbody.Despawn(holsterManager.ringLife);

        _currentCount = _rings.Count;
        if (_currentCount == 0)
        {
            PlayEmptyClipFx();
        }
    }

    private void PlayEmptyClipFx()
    {
        if (handAudioSource == null) //Not properly detected by debuglogger.isnull
        {
            DebugLogger.Error(MethodBase.GetCurrentMethod().Name,
                $"{nameof(handAudioSource)} is null. Must be set in editor.", this);
        }

        if (DebugLogger.IsNullError(holsterManager, this, "Must be set in editor.")) return;

        handAudioSource.PlayOneShot(holsterManager.emptyClip);
    }

    public void SetBaseCountAndColor(Skill skill, Color color)
    {
        if (DebugLogger.IsNullError(skill, this)) return;

        SetBaseCount(skill);
        _color = color;
    }

    public void ResetCount()
    {
        _currentCount = _baseCount;
        
        ClearRings();

        var ringFxTransform = ringFX.transform;
        for (int i = 0; i < _currentCount; i++)
        {
            var pooledObject = objectPool.GetPooledObject();
            if (DebugLogger.IsNullError(pooledObject, this)) continue;

            var pooledObjectRigidbody = (PooledRigidbody) pooledObject;
            if (DebugLogger.IsNullError(pooledObjectRigidbody, this)) return;
            
            pooledObjectRigidbody.Spawn(ringFxTransform, ringFxTransform.position, ringFxTransform.rotation, true);
            
            //TODO add setup to event
            var ringParticleSystem = pooledObject.GetComponent<ParticleSystem>().main;
            ringParticleSystem.startSize = holsterManager.startSize + (i * holsterManager.sizeIncrement);
            ringParticleSystem.startDelay = i * holsterManager.timeIncrement;
            ringParticleSystem.startColor = _color;
            _rings.Push(pooledObjectRigidbody);
        }
    }

    public void ZeroOutCount()
    {
        _baseCount = 0;
        
        ResetCount();
    }

    public void ClearRings()
    {
        if (_rings == null) return;
        
        foreach (var oldRing in _rings)
        {
            oldRing.Despawn();
        }
        _rings.Clear();
    }

    public void SetBaseCount(Skill skill)
    {
        _baseCount = skill.Count;
    }
}
