using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using UnityEngine;

public class PuppetLauncher : MonoBehaviour
{
    [SerializeField] private PuppetLauncherData puppetLauncherData;
    [SerializeField] private PuppetMaster puppetMaster;
    [SerializeField] private LookAtIK lookAtIk;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform hitTransform;
    [SerializeField] private Transform hipsTransform;
    [SerializeField] private Rigidbody hipsRigidbody;
    
    public bool HasBeenLaunched { get; private set; }

    private bool _isInitialized;
    private Transform _audioSourceTransform;
    private Transform _resetAudioSourceParent;
    private Vector3 _resetAudioSourcePosition;
    private Quaternion _resetAudioSourceRotation;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (DebugLogger.IsNullError(audioSource, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(puppetMaster, this, "Must be set in editor.")) return;

        _audioSourceTransform = audioSource.transform;

        _resetAudioSourceParent = _audioSourceTransform.parent;
        _resetAudioSourcePosition = _resetAudioSourceParent.position;

        _isInitialized = true;
    }

    public void ResetObject()
    {
        if (!_isInitialized) Initialize();
        
        if (DebugLogger.IsNullError(lookAtIk, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(_audioSourceTransform, this)) return;

        lookAtIk.enabled = true;

        _audioSourceTransform.SetParent(_resetAudioSourceParent);
        _audioSourceTransform.position = _resetAudioSourcePosition;
        _audioSourceTransform.rotation = _resetAudioSourceRotation;
        
        hipsRigidbody.velocity = Vector3.zero;
        hipsRigidbody.angularVelocity = Vector3.zero;

        HasBeenLaunched = false;
    }

    public virtual void Launch(float impulse)
    {
        if (!puppetMaster || !hitTransform)
            return;
        
        if (DebugLogger.IsNullError(lookAtIk, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(puppetLauncherData, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(hipsRigidbody, this)) return;
        if (DebugLogger.IsNullError(hipsTransform, this)) return;
        if (DebugLogger.IsNullError(_audioSourceTransform, this)) return;

        lookAtIk.enabled = false;

        var force = impulse * puppetLauncherData.hitForceMultiplier;
        if (force > puppetLauncherData.launchForce.maxValue)
        {
            force = puppetLauncherData.launchForce.maxValue;
        }
        else if (force < puppetLauncherData.launchForce.minValue)
        {
            force = puppetLauncherData.launchForce.minValue;
        }
        
        hipsRigidbody.AddForce(hitTransform.forward * force, ForceMode.Impulse);
        hipsRigidbody.AddForce(Vector3.up * puppetLauncherData.liftForce, ForceMode.Impulse);
        
        puppetLauncherData.characterAudioManager.PlayLaunchSound(audioSource);
        _audioSourceTransform.SetParent(hipsTransform);

        StartCoroutine(HasBeenLaunchedCoroutine());
    }
    
    private IEnumerator HasBeenLaunchedCoroutine()
    {
        yield return new WaitForSeconds(puppetLauncherData.launchDelay);
        HasBeenLaunched = true;
    }
}
