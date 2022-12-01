using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RootMotion.Dynamics;
using UnityEngine;

public class PuppetDeathHandler : MonoBehaviour
{
    [SerializeField] private PuppetDeathHandlerData puppetDeathHandlerData;
    [SerializeField] private Health health;
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private Collider characterCollider;
    [SerializeField] private Rigidbody characterControllerRigid;
    [SerializeField] private Rigidbody hipsRigidbody;
    [SerializeField] private HealthBar healthBar;
    

    private GameObject _healthBarGameObject;
    private GameObject _puppetMasterGameObject;
    private Transform _characterControllerTransform;
    private bool _hasBeenKilled;
    private bool _hasBeenTurnedOff;
    private bool _hasBeenInstantKilled;

    public Action WasTurnedOff;
    
    private const int HIPS = 0;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(healthBar, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(characterCollider, this, "Must be set in editor.")) return;

        health.WasKilled += StartPostDeathSequence;

        _healthBarGameObject = healthBar.gameObject;
        _puppetMasterGameObject = puppetMaster.gameObject;
        _characterControllerTransform = characterCollider.transform;
    }

    public void InstantKill()
    {
        if (_hasBeenInstantKilled) return;
        
        _hasBeenInstantKilled = true;

        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;

        KillPuppet();
        
        health.Kill();
        TurnPuppetOff();
    }
    
    private void KillPuppet()
    {
        if (_hasBeenKilled) return;

        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(characterCollider, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(characterControllerRigid, this, "Must be set in editor.")) return;

        puppetMaster.Kill();

        //TODO create way for collider to not get in way of player locomotion, possibly change layer
        characterControllerRigid.isKinematic = true;
        characterCollider.isTrigger = true;       
        _hasBeenKilled = true;
    }

    // Turn off unnecessary scripts and objects post-death for performance reasons
    private void TurnPuppetOff() 
    {
        if (_hasBeenTurnedOff) return;
        
        _hasBeenTurnedOff = true;

        if (DebugLogger.IsNullError(healthBar, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) return;

        _healthBarGameObject.SetActive(false);

        puppetMaster.Kill();
        puppetMaster.mode = PuppetMaster.Mode.Kinematic;
        _puppetMasterGameObject.SetActive(false);

        WasTurnedOff?.Invoke();
        WasTurnedOff = null;
    }

    private void StartPostDeathSequence()
    {
        if (_hasBeenKilled) return;

        KillPuppet();
        StartCoroutine(CheckBodyMovementCoroutine());
    }

    private IEnumerator CheckBodyMovementCoroutine()
    {
        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) yield break;
        if (DebugLogger.IsNullError(hipsRigidbody, this)) yield break;

        yield return new WaitForSeconds(puppetDeathHandlerData.turnOffTime.minValue);
        var timeElapsed = puppetDeathHandlerData.turnOffTime.minValue;
        while (enabled && timeElapsed < puppetDeathHandlerData.turnOffTime.maxValue)
        {
            if (puppetMaster.state == PuppetMaster.State.Dead &&
                hipsRigidbody.velocity.magnitude < puppetDeathHandlerData.deathVelocity) 
            {
                break;
            }
            
            yield return new WaitForSeconds(puppetDeathHandlerData.checkForDeathInterval);
            timeElapsed += puppetDeathHandlerData.checkForDeathInterval;
        }
        
        TurnPuppetOff();
    }
    
    public Vector3 GetHipsPosition()
    {
        return !hipsRigidbody ? Vector3.zero : hipsRigidbody.position;
    }

    public bool GetHasBeenInstantKilled()
    {
        return _hasBeenInstantKilled;
    }
    
    public void ResetObject()
    {
        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) return;

        health.WasKilled += StartPostDeathSequence;

        TurnPuppetOn();

        if (characterCollider)
        {
            characterControllerRigid.velocity = Vector3.zero;
            characterControllerRigid.angularVelocity = Vector3.zero;
            characterControllerRigid.isKinematic = false;

            if (_characterControllerTransform)
            {
                _characterControllerTransform.localPosition = Vector3.zero;
                _characterControllerTransform.localRotation = Quaternion.identity;
            }
            
            characterCollider.isTrigger = false;
        }
        
        _hasBeenKilled = false;
        _hasBeenInstantKilled = false;
    }
    
    private void TurnPuppetOn()
    {
        if (!_hasBeenTurnedOff) return;
        
        _hasBeenTurnedOff = false;

        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(healthBar, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(characterControllerRigid, this, "Must be set in editor.")) return;

        RevivePuppet();
        _puppetMasterGameObject.SetActive(true);
        
        _healthBarGameObject.SetActive(true);

        characterControllerRigid.isKinematic = false;
    }

    public void RevivePuppet()
    {
        puppetMaster.state = PuppetMaster.State.Alive;
        puppetMaster.mode = PuppetMaster.Mode.Active;
    }
}
