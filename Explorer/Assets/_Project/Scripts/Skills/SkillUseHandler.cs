using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillUseHandler : MonoBehaviour
{
    [SerializeField] private Transform leftEmitter;
    [SerializeField] private Transform rightEmitter;
    [SerializeField] private Transform leftHandSkills;
    [SerializeField] private Transform rightHandSkills;
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private AudioSource uiAudioSource;
    
    private Skill _currentLeftHandSkill;
    private Skill _currentRightHandSkill;
    private bool _isUsingLeftSkill;
    private bool _isUsingRightSkill;

    public Skill CurrentLeftHandSkill => _currentLeftHandSkill;

    public Skill CurrentRightHandSkill => _currentRightHandSkill;

    private bool IsRightHandedSkillTwoHanded => _currentRightHandSkill && _currentRightHandSkill.IsTwoHanded();
    private bool IsLeftHandedSkillTwoHanded => _currentLeftHandSkill && _currentLeftHandSkill.IsTwoHanded();
    
    
    private void Awake()
    {
        StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Initialize(leftHandSkills, leftEmitter);
        yield return new WaitForEndOfFrame();
        Initialize(rightHandSkills, rightEmitter);
    }

    private void Initialize(Transform skillsTransform, Transform emitter) 
    {
        skillsTransform.SetParent(emitter);
        skillsTransform.localPosition = new Vector3();
        skillsTransform.localEulerAngles = new Vector3();
    }

    [Button]
    public void AttemptStartUseLeftSkill() 
    {
        //TODO for using Bow Weapon Ability, ie ArrowRain
        if (!_currentLeftHandSkill)
        {
            if (_currentRightHandSkill && _currentRightHandSkill.IsUsedByOtherHand())
            {
                _currentRightHandSkill.UseAbility();
            }
            
            return;
        }
        
        AttemptStartUse(_currentLeftHandSkill, true);
    }

    [Button]
    public void AttemptStartUseRightSkill() 
    {
        if (!_currentRightHandSkill)
        {
            if (_currentLeftHandSkill && _currentLeftHandSkill.IsUsedByOtherHand())
            {
                _currentLeftHandSkill.UseAbility();
            }
            
            return;
        }
        
        AttemptStartUse(_currentRightHandSkill, false);
    }
    
    private void AttemptStartUse(Skill skill, bool isLeftSkill)
    {
        //Not enough mana or count
        if (!skill.CanCast())
        {
            if (DebugLogger.IsNullError(uiAudioSource, this, "Must be set in editor.")) return;
            if (DebugLogger.IsNullError(holsterManager, this, "Must be set in editor.")) return;
            
            uiAudioSource.PlayOneShot(holsterManager.invalidClip);
            return;
        }
        
        skill.StartUse();
        
        if (isLeftSkill)
        {
            _isUsingLeftSkill = true;
        }
        else
        {
            _isUsingRightSkill = true;
        }
    }

    [Button]
    public void AttemptEndUseLeftSkill()
    {
        if (!_isUsingLeftSkill || !_currentLeftHandSkill) return;
        
        AttemptEndUse(_currentLeftHandSkill, true);
    }

    [Button]
    public void AttemptEndUseRightSkill()
    {
        if (!_isUsingRightSkill || !_currentRightHandSkill) return;
        
        AttemptEndUse(_currentRightHandSkill, false);
    }
    
    private void AttemptEndUse(Skill skill, bool isLeftSkill)
    {
        if (isLeftSkill)
        {
            _isUsingLeftSkill = false;
        }
        else
        {
            _isUsingRightSkill = false;
        }

        if (DebugLogger.IsNullError(skill, this)) return;
        
        if (!skill.CanCast()) return;

        skill.EndUse();
    }

    public void EquipSkill(bool isLeftHandGrabbing, Skill skillToEquip)
    {
        if (skillToEquip == null) return;
        
        var isLeftHandUsing = isLeftHandGrabbing;
        if (skillToEquip.IsUsedByOtherHand()) isLeftHandUsing = !isLeftHandUsing;
        
        var currentEquippedSkill = isLeftHandUsing ? _currentLeftHandSkill : _currentRightHandSkill;
        var otherHandSkill = isLeftHandUsing ? _currentRightHandSkill : _currentLeftHandSkill;

        //TODO Invalid equip attempt, requires both hands to be free
        
        //is already equipped
        if (skillToEquip == currentEquippedSkill)
        {
            skillToEquip.Equip();
            return;
        }
        
        //not already equipped
        if (currentEquippedSkill != null) currentEquippedSkill.UnEquip();

        skillToEquip.Equip();

        //Is two handed or other skill is two handed
        if (skillToEquip.IsTwoHanded() || otherHandSkill && otherHandSkill.IsTwoHanded())
        {
            if (otherHandSkill) otherHandSkill.UnEquip();
            
            RemoveOtherSkill(isLeftHandUsing);
        }
        
        if (isLeftHandUsing)
        {
            _currentLeftHandSkill = skillToEquip;
        }
        else
        {
            _currentRightHandSkill = skillToEquip;
        }
    }

    public void UnequipSkill(bool isLeftHandUsing)
    {
        var currentEquippedSkill = isLeftHandUsing ? _currentLeftHandSkill : _currentRightHandSkill;
        var otherHandSkill = isLeftHandUsing ? _currentRightHandSkill : _currentLeftHandSkill;
        
        if (currentEquippedSkill) currentEquippedSkill.UnEquip();

        if (isLeftHandUsing && _currentLeftHandSkill)
        {
            _currentLeftHandSkill = null;
        }
        else if (!isLeftHandUsing && _currentRightHandSkill)
        {
            _currentRightHandSkill = null;
        }
        
        //Is two handed or other skill is two handed
        if (otherHandSkill && otherHandSkill.IsTwoHanded())
        {
            otherHandSkill.UnEquip();
            
            RemoveOtherSkill(isLeftHandUsing);
        }
    }
    
    private void RemoveOtherSkill(bool isLeftHandUsing)
    {
        if (isLeftHandUsing && _currentRightHandSkill)
        {
            _currentRightHandSkill.UnEquip();
            _currentRightHandSkill = null;
        }
        else if (!isLeftHandUsing && _currentLeftHandSkill)
        {
            _currentLeftHandSkill.UnEquip();
            _currentLeftHandSkill = null;
        }
    }

    public bool CanEquip(Skill skill)
    {
        if (!skill) return false;
        if (skill.IsTwoHanded() && (_currentLeftHandSkill || _currentRightHandSkill)) return false;
        if (IsLeftHandedSkillTwoHanded) return false;
        if (IsRightHandedSkillTwoHanded) return false;

        return true;
    }
}
