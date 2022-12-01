using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ModifierReward : Reward, IGrabbable
{
    [SerializeField] private ModifierTypeLookUp modifierTypeLookUp;
    [SerializeField] private Rigidbody thisRigidbody;
    [SerializeField] private GameObject disabledModel;
    [SerializeField] private GameObject abilityOneModel;
    [SerializeField] private GameObject abilityTwoModel;
    [SerializeField] private GameObject cooldownModel;
    [SerializeField] private GameObject costModel;
    [SerializeField] private GameObject countModel;
    [SerializeField] private GameObject damageModel;
    [SerializeField] private GameObject speedModel;
    [SerializeField] private Transform attachTransform;
    [SerializeField] private float grabSmoothTime = 0.1F;
    [SerializeField] private float grabSlerpRatio = .1f;
    [SerializeField] private float releaseSlerpRatio = .05f;
    [SerializeField] private float releaseSmoothTime = 0.3F;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private AudioSource thisAudioSource;
    [SerializeField] private SimpleAudioEvent grabAudioEvent;
    [SerializeField] private SimpleHapticEvent grabHapticEvent;
    [SerializeField] private Outline outline;
    [SerializeField] private SmoothDampFollow smoothDampFollow;
    [SerializeField] private Transform gazeFollowTransform;

    private bool _canBeGrabbed;
    private bool _grabIsComplete;
    private Vector3 _interactorLocalPosition;
    private Quaternion _interactorLocalRotation;
    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation = Quaternion.identity;
    private Vector3 _velocity = Vector3.zero;
    private Dictionary<ModifierTypeSO, int> _modifiers = new Dictionary<ModifierTypeSO, int>();
    private Dictionary<ModifierTypeSO, GameObject> _rewardTypes = new Dictionary<ModifierTypeSO, GameObject>(); 
    private HolsterModifierContainer _currentHolsterModifierContainer;
    private ModifierTypeSO _currentModifierType;
    private Interactor _currentInteractor;
    private Transform _cameraTransform;

    private const int MODIFIER_TYPE_INDEX = 0;
    private const float GRAB_POSITION_TOLERANCE = .01f;
    private const float RELEASE_POSITION_TOLERANCE = .005f;
    

    protected override void Awake()
    {
        base.Awake();
        
        if (DebugLogger.IsNullError(cooldownModel, this)) return;
        if (DebugLogger.IsNullError(costModel, this)) return;
        if (DebugLogger.IsNullError(countModel, this)) return;
        if (DebugLogger.IsNullError(damageModel, this)) return;
        if (DebugLogger.IsNullError(speedModel, this)) return;
        if (DebugLogger.IsNullError(abilityOneModel, this)) return;
        if (DebugLogger.IsNullError(abilityTwoModel, this)) return;
        if (DebugLogger.IsNullError(disabledModel, this)) return;

        _rewardTypes = new Dictionary<ModifierTypeSO, GameObject>()
        {
            { modifierTypeLookUp.Cooldown, cooldownModel },
            { modifierTypeLookUp.Cost, costModel },
            { modifierTypeLookUp.Count, countModel },
            { modifierTypeLookUp.Power, damageModel },
            { modifierTypeLookUp.Speed, speedModel },
            { modifierTypeLookUp.Ability1, abilityOneModel },
            { modifierTypeLookUp.Ability2, abilityTwoModel },
        };

        if (outline) outline.enabled = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        _spawnPosition = ThisTransform.position;
        if (outline) outline.enabled = false;

        UnfollowCameraGaze();
    }

    private void Update()
    {
        if (_grabIsComplete) return;
        
        if (IsGrabbed())
        {
            UpdateInteractorLocalPose();
            var targetPosition = GetWorldAttachPosition(_currentInteractor);
            if (Vector3.Distance(ThisTransform.position, targetPosition) < GRAB_POSITION_TOLERANCE)
            {
                CompleteGrab();
                return;
            }
        
            var targetRotation = GetWorldAttachRotation(_currentInteractor);

            ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position, targetPosition, ref _velocity, grabSmoothTime);
            ThisTransform.rotation = Quaternion.Slerp(ThisTransform.rotation, targetRotation, grabSlerpRatio);
            return;
        }

        if (Vector3.Distance(ThisTransform.position, _spawnPosition) < RELEASE_POSITION_TOLERANCE)
        {
            ThisTransform.Rotate(0,rotationSpeed * Time.deltaTime,0);
        }
        else
        {
            MoveToSpawnPositionAndRotation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GetHolsterModifierContainer(other);
        if (_currentHolsterModifierContainer)
        {
            FollowCameraGaze();
            SetAndActivateEffectUI(_modifiers, _currentHolsterModifierContainer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var holsterModifierContainer = other.GetComponent<HolsterModifierContainer>();
        if (holsterModifierContainer)
        {
            UnfollowCameraGaze();
            ReleaseHolsterModifierContainer();
            DeactivateEffectUI();
        }
    }

    private void CompleteGrab()
    {
        if (_grabIsComplete) return;
        
        if (grabAudioEvent) grabAudioEvent.Play(thisAudioSource);
        if (grabHapticEvent) grabHapticEvent.Play(_currentInteractor.IsLeftHand);
        SnapToPosition();
        AttachRigidbody();
        if (outline) outline.enabled = false;
        _grabIsComplete = true;
    }

    private void FollowCameraGaze()
    {
        smoothDampFollow.SetFollowTransform(gazeFollowTransform);
    }
    
    private void UnfollowCameraGaze()
    {
        smoothDampFollow.SetFollowTransform(ThisTransform);
    }

    public void AttemptGrab(Interactor interactor)
    {
        if (!_canBeGrabbed) return;
        
        Grab(interactor);
    }

    public void Grab(Interactor interactor)
    {
        _currentInteractor = interactor;
        
        ActivateTypeUI();
    }

    public void AttemptUnGrab()
    {
        if (!_currentInteractor) return;
        
        UnGrab();
    }

    public void UnGrab()
    {
        if (!_currentInteractor) return;

        DetachRigidbody();
        
        _currentInteractor = null;
        _grabIsComplete = false;
        
        Use();
        ResetRigidbody();
        DeactivateTypeUI();
        DeactivateEffectUI();
        ReleaseHolsterModifierContainer();
    }

    public bool IsGrabbed()
    {
        return _currentInteractor;
    }

    public bool IsGrabbedBy(Interactor interactor)
    {
        return _currentInteractor == interactor;
    }

    protected override void Use()
    {
        if (!CanBeUsed()) return;

        if (!_currentHolsterModifierContainer) return;
        if (!_currentHolsterModifierContainer.CanUpgrade(_currentModifierType)) return;

        _currentHolsterModifierContainer.AddModifierTypeToDictionary(_currentModifierType);

        foreach (var modifier in _modifiers)
        {
            _currentHolsterModifierContainer.AddModifier(modifier.Key, modifier.Value);
        }

        var holsterCollider = _currentHolsterModifierContainer.GetComponent<HolsterCollider>();
        if (holsterCollider) holsterCollider.SetupFeedbackFx();
        
        base.Use();
    }
    
    public void ToggleHighlight(bool state)
    {
        if (outline) outline.enabled = state;
    }

    public override void Clear()
    {
        _rewardTypes[_currentModifierType].SetActive(false);
        
        base.Clear();
    }
    
    private void ResetRigidbody()
    {
        if (DebugLogger.IsNullError(thisRigidbody, this, "Must be set in editor.")) return;

        thisRigidbody.velocity = Vector3.zero;
        thisRigidbody.angularVelocity = Vector3.zero;
    }
    
    private void MoveToSpawnPositionAndRotation()
    {
        ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position, _spawnPosition, ref _velocity, releaseSmoothTime);
        transform.rotation = Quaternion.Slerp(ThisTransform.rotation, _spawnRotation, releaseSlerpRatio);

        if (Vector3.Distance(ThisTransform.position, _spawnPosition) < RELEASE_POSITION_TOLERANCE)
        {
            ThisTransform.position = _spawnPosition;
            ThisTransform.rotation = _spawnRotation;
        }
    }

    private void GetHolsterModifierContainer(Collider other)
    {
        var holsterModifierContainer = other.GetComponent<HolsterModifierContainer>();
        if (holsterModifierContainer)
        {
            _currentHolsterModifierContainer = holsterModifierContainer;
        }
    }
    
    private void ReleaseHolsterModifierContainer()
    {
        _currentHolsterModifierContainer = null;
    }

    public void SetModifierData(Dictionary<ModifierTypeSO, int> modifiers, List<ModifierTypeSO> modifierTypeSos)
    {
        if (DebugLogger.IsNullOrEmptyError(modifiers, this)) return;
        if (DebugLogger.IsNullError(_rewardTypes, this)) return;

        _modifiers = modifiers;
        if (DebugLogger.IsNullOrEmptyError(_modifiers, this)) return;

        _currentModifierType = modifierTypeSos[MODIFIER_TYPE_INDEX];
        if (DebugLogger.IsNullError(_currentModifierType, this)) return;
        
        _rewardTypes[_currentModifierType].SetActive(true);
        disabledModel.SetActive(false);
        _canBeGrabbed = true;
        
        SetModifierTypeUI(_modifiers);
    }
    
    private void SnapToPosition()
    {
        UpdateInteractorLocalPose();
        SnapToTarget();
    }
    
    private void AttachRigidbody()
    {
        if (_grabIsComplete) return;
        
        _currentInteractor.Attach(thisRigidbody);
    }
    
    private void DetachRigidbody()
    {
        _currentInteractor.Detach();
    }
    
    private void UpdateInteractorLocalPose()
    {
        // In order to move the Interactable to the Interactor we need to
        // calculate the Interactable attach point in the coordinate system of the
        // Interactor's attach point.
        var attachPosition = attachTransform.position;
        var attachOffset = thisRigidbody.worldCenterOfMass - attachPosition;
        var localAttachOffset = attachTransform.InverseTransformDirection(attachOffset);

        var inverseLocalScale = _currentInteractor.AttachTransform.lossyScale;
        inverseLocalScale = new Vector3(1f / inverseLocalScale.x, 1f / inverseLocalScale.y, 1f / inverseLocalScale.z);
        localAttachOffset.Scale(inverseLocalScale);

        _interactorLocalPosition = localAttachOffset;
        _interactorLocalRotation = Quaternion.Inverse(Quaternion.Inverse(thisRigidbody.rotation) * attachTransform.rotation);
        if (_currentInteractor.IsLeftHand) _interactorLocalRotation *= Quaternion.Euler(0, 0, 180f);
    }
    
    private void SnapToTarget()
    {
        // Compute the unsmoothed target world position and rotation
        var rawTargetWorldPosition = GetWorldAttachPosition(_currentInteractor);
        var rawTargetWorldRotation = GetWorldAttachRotation(_currentInteractor);

        ThisTransform.position = rawTargetWorldPosition;
        ThisTransform.rotation = rawTargetWorldRotation;
    }

    private Vector3 GetWorldAttachPosition(Interactor interactor)
    {
        return interactor.AttachTransform.position + interactor.AttachTransform.rotation * _interactorLocalPosition;
    }

    private Quaternion GetWorldAttachRotation(Interactor interactor)
    {
        return interactor.AttachTransform.rotation * _interactorLocalRotation;
    }
}
