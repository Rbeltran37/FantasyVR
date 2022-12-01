using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class HolsterCollider : MonoBehaviour, IGrabbable
{
    [SerializeField] private HolsterSetupHelper holsterSetupHelper;
    [SerializeField] private HolsterCooldown holsterCooldown;
    [SerializeField] private SkillCount leftSkillCount;
    [SerializeField] private SkillCount rightSkillCount;
    [SerializeField] private HolsterModifierContainer holsterModifierContainer;
    [SerializeField] private Transform leftHandSkillsTransform;
    [SerializeField] private Transform rightHandSkillsTransform;
    [SerializeField] private Transform holsterFxTransform;
    [SerializeField] private SkillUseHandler skillUseHandler;
    [SerializeField] private Interactor leftInteractor;
    [SerializeField] private Interactor rightInteractor;
    [SerializeField] private Outline outline;

    private bool _inHolster = false;
    private GameObject _currentLeftSkillGameObject;
    private GameObject _currentRightSkillGameObject;
    private GameObject _currentEquipFx;
    private GameObject _currentHolsterRing;
    private GameObject _currentHolsterModel;
    private Skill _currentLeftSkill;
    private Skill _currentRightSkill;
    private ManaPool _manaPool;
    private Interactor _currentInteractor;

    public Action<bool> HolsterWasEntered;
    public Action<bool> SkillWasEquipped;
    public Action HolsterWasExited;
    public Action InvalidEquipWasAttempted;

    
    private void OnTriggerEnter(Collider other)
    {
        CheckIfInHolster(other);
    }

    private void CheckIfInHolster(Collider other)
    {
        var interactor = other.GetComponent<Interactor>();
        if (!interactor) return;
        
        HolsterWasEntered?.Invoke(interactor.IsLeftHand);
        _inHolster = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_inHolster)
            return;
        
        CheckIfInHolster(other);
    }

    private void OnTriggerExit(Collider other) {

        var interactor = other.GetComponent<Interactor>();
        if (!interactor) return;
        
        _inHolster = false;
        HolsterWasExited?.Invoke();
    }

    private void EquipSkill(bool isLeftHandGrabbing) {

        if (!_inHolster) return;

        var skillToEquip = GetSkillToEquip(isLeftHandGrabbing);
        var offHandSkill = GetOffHandSkill(isLeftHandGrabbing);

        if (!skillToEquip || !skillToEquip.SkillSo) return;

        var isTwoHanded = skillToEquip.IsTwoHanded() || (offHandSkill && offHandSkill.IsTwoHanded());
        if (isTwoHanded)
        {
            if (rightSkillCount)
            {
                rightSkillCount.ZeroOutCount();
            }

            if (leftSkillCount)
            {
                leftSkillCount.ZeroOutCount();
            }
        }

        SkillWasEquipped?.Invoke(isLeftHandGrabbing);

        if (skillToEquip.IsUsedByOtherHand())
        {
            OppositeHandCountReset(isLeftHandGrabbing, isTwoHanded, skillToEquip);
        }
        else
        {
            SameHandCountReset(isLeftHandGrabbing, isTwoHanded, skillToEquip);
        }
        
        if (DebugLogger.IsNullError(holsterCooldown, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(skillUseHandler, this, "Must be set in editor.")) return;
        
        holsterCooldown.ActivateCooldown(skillToEquip);
        skillUseHandler.EquipSkill(isLeftHandGrabbing, skillToEquip);
    }

    private Skill GetSkillToEquip(bool isLeftHandGrabbing)
    {
        return isLeftHandGrabbing ? _currentLeftSkill : _currentRightSkill;
    }
    
    private Skill GetOffHandSkill(bool isLeftHandGrabbing)
    {
        return isLeftHandGrabbing ? _currentRightSkill : _currentLeftSkill;
    }

    private void SameHandCountReset(bool isLeftHand, bool isTwoHanded, Skill skillToEquip)
    {
        if (isLeftHand && leftSkillCount)
        {
            leftSkillCount.SetBaseCountAndColor(skillToEquip, skillToEquip.SkillSo.uiColor);
            leftSkillCount.ResetCount();
            if (isTwoHanded)
                rightSkillCount.ZeroOutCount();

            skillToEquip.SetSkillCount(leftSkillCount);
        }
        else if (!isLeftHand && rightSkillCount)
        {
            rightSkillCount.SetBaseCountAndColor(skillToEquip, skillToEquip.SkillSo.uiColor);
            rightSkillCount.ResetCount();
            if (isTwoHanded)
                leftSkillCount.ZeroOutCount();
            
            skillToEquip.SetSkillCount(rightSkillCount);
        }
    }

    private void OppositeHandCountReset(bool isLeftHand, bool isTwoHanded, Skill skillToEquip)
    {
        if (!isLeftHand && leftSkillCount)
        {
            leftSkillCount.SetBaseCountAndColor(skillToEquip, skillToEquip.SkillSo.uiColor);
            leftSkillCount.ResetCount();
            if (isTwoHanded)
                rightSkillCount.ZeroOutCount();
            
            skillToEquip.SetSkillCount(leftSkillCount);
        }
        else if (isLeftHand && rightSkillCount)
        {
            rightSkillCount.SetBaseCountAndColor(skillToEquip, skillToEquip.SkillSo.uiColor);
            rightSkillCount.ResetCount();
            if (isTwoHanded)
                leftSkillCount.ZeroOutCount();
            
            skillToEquip.SetSkillCount(rightSkillCount);
        }
    }

    //TODO isLeftHandSkill bool may be obsolete
    public void SetupNewSkill(Skill leftSkill, Skill rightSkill, PlayerHand leftPlayerHand, PlayerHand rightPlayerHand)
    {
        if (DebugLogger.IsNullError(leftSkill, this)) return;
        if (DebugLogger.IsNullError(rightSkill, this)) return;
        if (DebugLogger.IsNullError(leftSkill.SkillSo, this)) return;
        if (DebugLogger.IsNullError(rightSkill.SkillSo, this)) return;
        
        SetupSkillGameObjects(leftSkill, rightSkill, leftPlayerHand, rightPlayerHand);

        var skill = _currentLeftSkill;
        
        SetupHolsterModifierContainer();
        SetupEquipFx(skill);
        SetupHolsterRing(skill);
        SetupHolsterModel(skill);
        SetupSkillCount(_currentLeftSkill, _currentRightSkill);
        SetupCooldown(skill);
        
        SetupFeedbackFx();
    }

    public void SetupFeedbackFx()
    {
        EquipSkill(IsGrabbedBy(leftInteractor));
        skillUseHandler.UnequipSkill(IsGrabbedBy(leftInteractor));
        holsterCooldown.UnequipSkill();
    }

    private void SetupSkillGameObjects(Skill leftSkill, Skill rightSkill, PlayerHand leftPlayerHand, PlayerHand rightPlayerHand)
    {
        if (_currentLeftSkill)
        {
            _currentLeftSkill.UnEquip();
            Destroy(_currentLeftSkillGameObject);
        }

        if (_currentRightSkill)
        {
            _currentRightSkill.UnEquip();
            Destroy(_currentRightSkillGameObject);
        }

        _currentLeftSkillGameObject = Instantiate(leftSkill.gameObject, leftHandSkillsTransform);
        if (DebugLogger.IsNullError(_currentLeftSkillGameObject, this)) return;

        var leftSkillTransform = _currentLeftSkillGameObject.transform;
        leftSkillTransform.localPosition = Vector3.zero;
        leftSkillTransform.localEulerAngles = Vector3.zero;

        _currentLeftSkill = _currentLeftSkillGameObject.GetComponent<Skill>();
        _currentLeftSkill.Setup(leftPlayerHand, _manaPool);

        _currentRightSkillGameObject = Instantiate(rightSkill.gameObject, rightHandSkillsTransform);
        if (DebugLogger.IsNullError(_currentRightSkillGameObject, this)) return;
        
        var rightSkillTransform = _currentRightSkillGameObject.transform;
        rightSkillTransform.localPosition = Vector3.zero;
        rightSkillTransform.localEulerAngles = Vector3.zero;

        _currentRightSkill = _currentRightSkillGameObject.GetComponent<Skill>();
        _currentRightSkill.Setup(rightPlayerHand, _manaPool);
    }

    private void SetupCooldown(Skill skill)
    {
        holsterCooldown.SetupNewCooldown(skill, _currentHolsterModel, _currentEquipFx);
    }

    private void SetupHolsterModifierContainer()
    {
        if (DebugLogger.IsNullWarning(holsterModifierContainer, this, "Must be set in editor.")) return;
        
        holsterModifierContainer.SetModifierData(_currentLeftSkill, _currentRightSkill);
    }

    private void SetupEquipFx(Skill skill)
    {
        if (DebugLogger.IsNullWarning(holsterFxTransform, this, "Must be set in editor.")) return;

        if (_currentEquipFx)
            Destroy(_currentEquipFx);

        if (DebugLogger.IsNullError(skill, this)) return;

        if (skill.SkillSo.equipFx)
            _currentEquipFx = Instantiate(skill.SkillSo.equipFx, holsterFxTransform);
    }

    private void SetupHolsterModel(Skill skill)
    {
        if (_currentHolsterModel)
            Destroy(_currentHolsterModel);

        if (skill.SkillSo.holsterModel)
        {
            _currentHolsterModel = Instantiate(skill.SkillSo.holsterModel, holsterFxTransform);
            if (_currentHolsterModel)
            {
                var holsterFxTransformScale = holsterFxTransform.localScale;
                var holsterModelScale = _currentHolsterModel.transform.localScale;
                
                _currentHolsterModel.transform.localScale = new Vector3(
                    holsterModelScale.x * holsterFxTransformScale.x,
                    holsterModelScale.y * holsterFxTransformScale.y,
                    holsterModelScale.z * Math.Abs(holsterFxTransformScale.z));
            }
        }
    }

    private void SetupHolsterRing(Skill skill)
    {
        //TODO use objectPools?
        if (_currentHolsterRing)
            Destroy(_currentHolsterRing);

        if (skill.SkillSo.holsterRing)
        {
            _currentHolsterRing = Instantiate(skill.SkillSo.holsterRing, holsterFxTransform);
            if (_currentHolsterRing)
            {
                var holsterFxTransformScale = holsterFxTransform.localScale;
                var holsterRingScale = _currentHolsterRing.transform.localScale;
                _currentHolsterRing.transform.localScale = new Vector3(
                    holsterRingScale.x * Math.Abs(holsterFxTransformScale.x),
                    holsterRingScale.y * holsterFxTransformScale.y,
                    holsterRingScale.z * Math.Abs(holsterFxTransformScale.z));
            }
        }
    }

    private void SetupSkillCount(Skill leftSkill, Skill rightSkill)
    {
        if (DebugLogger.IsNullError(leftSkillCount, this)) return;

        leftSkillCount.ClearRings();
        leftSkillCount.SetBaseCountAndColor(leftSkill, leftSkill.SkillSo.uiColor);

        if (DebugLogger.IsNullError(rightSkillCount, this)) return;

        rightSkillCount.ClearRings();
        rightSkillCount.SetBaseCountAndColor(rightSkill, rightSkill.SkillSo.uiColor);
    }

    private void InvalidEquipAttempt()
    {
        InvalidEquipWasAttempted?.Invoke();
    }

    public void SetupHolster(SkillContainer skillContainer)
    {
        if (DebugLogger.IsNullError(skillContainer, this)) return;
        if (DebugLogger.IsNullError(holsterSetupHelper, this, "Must be set in editor.")) return;

        holsterSetupHelper.SetupHolster(this, skillContainer);
        
    }

    public void SetManaPool(ManaPool manaPool)
    {
        _manaPool = manaPool;
    }

    public void AttemptGrab(Interactor interactor)
    {
        if (DebugLogger.IsNullError(holsterCooldown, this)) return;
            
        if (holsterCooldown.IsCooldownActive())
        {
            InvalidEquipAttempt();
            return;
        }
        
        var isLeftHandGrabbing = IsLeftHandGrabbing(interactor);
        var skillToEquip = GetSkillToEquip(isLeftHandGrabbing);
        if (!skillUseHandler.CanEquip(skillToEquip))
        {
            InvalidEquipAttempt();
            return;
        }
        
        Grab(interactor);
    }

    public void Grab(Interactor interactor)
    {
        var isLeftHandGrabbing = IsLeftHandGrabbing(interactor);
        _currentInteractor = isLeftHandGrabbing ? leftInteractor : rightInteractor;
        
        EquipSkill(isLeftHandGrabbing);
    }

    public void AttemptUnGrab()
    {
        if (!_currentInteractor) return;

        UnGrab();
    }

    public void UnGrab()
    {
        if (DebugLogger.IsNullError(skillUseHandler, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(holsterCooldown, this, "Must be set in editor.")) return;

        skillUseHandler.UnequipSkill(IsGrabbedBy(leftInteractor));
        holsterCooldown.UnequipSkill();

        _currentInteractor = null;
    }

    public bool IsGrabbed()
    {
        return _currentInteractor;
    }

    public bool IsGrabbedBy(Interactor interactor)
    {
        return _currentInteractor == interactor;
    }
    
    private bool IsLeftHandGrabbing(Interactor interactor)
    {
        return interactor == leftInteractor;
    }

    public void ToggleHighlight(bool state)
    {
        if (outline) outline.enabled = state;
    }
}
