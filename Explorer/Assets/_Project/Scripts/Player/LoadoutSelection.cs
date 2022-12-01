using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

//TODO may be obsolete
public class LoadoutSelection : MonoBehaviour
{
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private HolsterSetupHelper holsterSetupHelper;
    [SerializeField] private XRUIInputModule xruiInputModule;
    [SerializeField] private List<HeroClass> heroClasses = new List<HeroClass>();
    [SerializeField] private Text heroClassText;
    [SerializeField] private Text leftWaistHolsterText;
    [SerializeField] private Text rightWaistHolsterText;
    [SerializeField] private Text leftBackHolsterText;
    [SerializeField] private Text rightBackHolsterText;

    private HeroClass _currentHeroClass;
    private List<SkillContainer> _skills = new List<SkillContainer>();
    private int _numClasses = 0;
    private int _currentClassIndex = -1;
    private int _numSkills = 0;
    private int _currentLeftWaistIndex = 0;
    private int _currentRightWaistIndex = 0;
    private int _currentLeftBackIndex = 0;
    private int _currentRightBackIndex = 0;


    private void Awake()
    {
        _numClasses = heroClasses.Capacity;
        
        GetNextClass();
        SetSkillSet();

        if (xruiInputModule) xruiInputModule.enabled = true;
    }

    private void SetSkillSet()
    {
        if (heroClasses == null || heroClasses.Count == 0) return;
        if (!_currentHeroClass) _currentHeroClass = heroClasses[0];
        if (!holsterManager) return;
        
        //holsterManager.skillContainerManager = _currentHeroClass.skillContainerManager;
        
        if (!_currentHeroClass || !_currentHeroClass.skillContainerManager) return;
        
        _skills = _currentHeroClass.skillContainerManager.skillContainers;
        if (_skills == null || _skills.Count == 0) return;

        _numSkills = _skills.Count;
        _currentLeftWaistIndex = 0;
        _currentRightWaistIndex = 0; 
        _currentLeftBackIndex = 0; 
        _currentRightBackIndex = 0;
        
        SetupHolster(true, true);
        SetupHolster(false, true);
        SetupHolster(true, false);
        SetupHolster(false, false);
    }

    [Button]
    public void GetNextClass()
    {
        _currentClassIndex++;
        if (_currentClassIndex >= _numClasses) _currentClassIndex = 0;

        _currentHeroClass = heroClasses[_currentClassIndex];

        if (heroClassText && _currentHeroClass) heroClassText.text = _currentHeroClass.heroClassName;
        
        SetSkillSet();
    }

    [Button]
    public void GetPreviousClass()
    {
        _currentClassIndex--;
        if (_currentClassIndex < 0) _currentClassIndex = _numClasses - 1;

        _currentHeroClass = heroClasses[_currentClassIndex];
        
        if (heroClassText && _currentHeroClass) heroClassText.text = _currentHeroClass.heroClassName;
        
        SetSkillSet();
    }

    [Button]
    public void GetNextLeftWaistSkill()
    {
        GetNextSkill(true, true);
    }
    
    [Button]
    public void GetNextRightWaistSkill()
    {
        GetNextSkill(false, true);
    }
    
    [Button]
    public void GetNextLeftBackSkill()
    {
        GetNextSkill(true, false);
    }
    
    [Button]
    public void GetNextRightBackSkill()
    {
        GetNextSkill(false, false);
    }
    
    [Button]
    public void GetPreviousLeftWaistSkill()
    {
        GetPreviousSkill(true, true);
    }
    
    [Button]
    public void GetPreviousRightWaistSkill()
    {
        GetPreviousSkill(false, true);
    }
    
    [Button]
    public void GetPreviousLeftBackSkill()
    {
        GetPreviousSkill(true, false);
    }
    
    [Button]
    public void GetPreviousRightBackSkill()
    {
        GetPreviousSkill(false, false);
    }

    public void SetHolsterSetupHelper(HolsterSetupHelper holsterHelper)
    {
        holsterSetupHelper = holsterHelper;
    }

    private void GetNextSkill(bool isLeft, bool isWaist)
    {
        if (isLeft)
        {
            if (isWaist)
            {
                _currentLeftWaistIndex++;
                if (_currentLeftWaistIndex >= _numSkills) _currentLeftWaistIndex = 0;

                SetupHolster(true, true);
            }
            else
            {
                _currentLeftBackIndex++;
                if (_currentLeftBackIndex >= _numSkills) _currentLeftBackIndex = 0;
                
                SetupHolster(true, false);
            }
        }
        else
        {
            if (isWaist)
            {
                _currentRightWaistIndex++;
                if (_currentRightWaistIndex >= _numSkills) _currentRightWaistIndex = 0;

                SetupHolster(false, true);
            }
            else
            {
                _currentRightBackIndex++;
                if (_currentRightBackIndex >= _numSkills) _currentRightBackIndex = 0;
                
                SetupHolster(false, false);
            }
        }
    }

    private void GetPreviousSkill(bool isLeft, bool isWaist)
    {
        if (isLeft)
        {
            if (isWaist)
            {
                _currentLeftWaistIndex--;
                if (_currentLeftWaistIndex < 0) _currentLeftWaistIndex = _numSkills - 1;

                SetupHolster(true, true);
            }
            else
            {
                _currentLeftBackIndex--;
                if (_currentLeftBackIndex < 0) _currentLeftBackIndex = _numSkills - 1;
                
                SetupHolster(true, false);
            }
        }
        else
        {
            if (isWaist)
            {
                _currentRightWaistIndex--;
                if (_currentRightWaistIndex < 0) _currentRightWaistIndex = _numSkills - 1;

                SetupHolster(false, true);
            }
            else
            {
                _currentRightBackIndex--;
                if (_currentRightBackIndex < 0) _currentRightBackIndex = _numSkills - 1;
                
                SetupHolster(false, false);
            }
        }
    }
    
    private void SetupHolster(bool isLeft, bool isWaist)
    {
        if (isLeft)
        {
            if (isWaist)
            {
                var skillToEquip = _skills[_currentLeftWaistIndex];
                
                if (holsterManager) holsterManager.leftWaistSkill = skillToEquip;
                if (holsterSetupHelper && skillToEquip) holsterSetupHelper.SetupLeftWaistHolster(skillToEquip);
                if (leftWaistHolsterText) leftWaistHolsterText.text = skillToEquip.skillName;
            }
            else
            {
                var skillToEquip = _skills[_currentLeftBackIndex];
                
                if (holsterManager) holsterManager.leftBackSkill = skillToEquip;
                if (holsterSetupHelper && skillToEquip) holsterSetupHelper.SetupLeftBackHolster(skillToEquip);
                if (leftBackHolsterText) leftBackHolsterText.text = skillToEquip.skillName;
            }
        }
        else
        {
            if (isWaist)
            {
                var skillToEquip = _skills[_currentRightWaistIndex];
                
                if (holsterManager) holsterManager.rightWaistSkill = skillToEquip;
                if (holsterSetupHelper && skillToEquip) holsterSetupHelper.SetupRightWaistHolster(skillToEquip);
                if (rightWaistHolsterText) rightWaistHolsterText.text = skillToEquip.skillName;
            }
            else
            {
                var skillToEquip = _skills[_currentRightBackIndex];
                
                if (holsterManager) holsterManager.rightBackSkill = skillToEquip;
                if (holsterSetupHelper && skillToEquip) holsterSetupHelper.SetupRightBackHolster(skillToEquip);
                if (rightBackHolsterText) rightBackHolsterText.text = skillToEquip.skillName;
            }
        }
    }
}
