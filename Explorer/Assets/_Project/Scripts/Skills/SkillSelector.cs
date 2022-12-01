using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillSelector : GrabbableObject
{
    [SerializeField] private SkillContainer skillContainer;
    [SerializeField] private TMP_Text textMeshPro;
    [SerializeField] private float releaseSmoothTime = 0.3F;
    [SerializeField] private float releaseSlerpRatio = .05f;
    [SerializeField] private float rotationSpeed = 30f;

    private HolsterCollider _currentHolster;
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    
    private const float RELEASE_POSITION_TOLERANCE = .005f;

    
    protected override void Awake()
    {
        base.Awake();
        
        _defaultPosition = ThisTransform.position;
        _defaultRotation = ThisTransform.rotation;

        if (DebugLogger.IsNullError(skillContainer, this, "Must be set in editor.")) return;
        
        var skillName = skillContainer.skillName;
        AddLineOfTextToUi(skillName);
    }

    protected override void Update()
    {
        base.Update();
        
        if (IsGrabbed()) return;

        if (Vector3.Distance(ThisTransform.position, _defaultPosition) < RELEASE_POSITION_TOLERANCE)
        {
            SpinObject();
        }
        else
        {
            MoveToDefaultPositionAndRotation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var holsterCollider = other.GetComponent<HolsterCollider>();
        if (!holsterCollider) return;
        
        _currentHolster = holsterCollider;
    }

    private void OnTriggerExit(Collider other)
    {
        var holsterCollider = other.GetComponent<HolsterCollider>();
        if (!holsterCollider) return;
        
        _currentHolster = null;
    }
    
    private void MoveToDefaultPositionAndRotation()
    {
        ThisTransform.position = Vector3.SmoothDamp(ThisTransform.position, _defaultPosition, ref Velocity, releaseSmoothTime);
        ThisTransform.rotation = Quaternion.Slerp(ThisTransform.rotation, _defaultRotation, releaseSlerpRatio);
        
        if (Vector3.Distance(ThisTransform.position, _defaultPosition) < RELEASE_POSITION_TOLERANCE)
        {
            ThisTransform.position = _defaultPosition;
            ThisTransform.rotation = _defaultRotation;
        }
    }

    private void SpinObject()
    {
        ThisTransform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    public override void UnGrab()
    {
        base.UnGrab();
        
        AttemptSetup();
        ResetRigidbody();
    }

    private void ResetRigidbody()
    {
        if (DebugLogger.IsNullError(ThisRigidbody, this, "Must be set in editor.")) return;

        ThisRigidbody.velocity = Vector3.zero;
        ThisRigidbody.angularVelocity = Vector3.zero;
    }

    private void AttemptSetup()
    {
        if (DebugLogger.IsNullInfo(_currentHolster, this)) return;
        
        if (!_currentHolster) return;
        
        _currentHolster.SetupHolster(skillContainer);
    }

    private void AddLineOfTextToUi(string text)
    {
        if (DebugLogger.IsNullError(textMeshPro, this, "Must be set in editor.")) return;
        
        textMeshPro.text += text + "\n";
    }
}
