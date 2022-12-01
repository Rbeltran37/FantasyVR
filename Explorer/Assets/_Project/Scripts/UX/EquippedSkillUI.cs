using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSkillUI : MonoBehaviour
{
    [SerializeField] private Transform emitterTransform;
    [SerializeField] private Transform interactorTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject canvas;
    [SerializeField] private SkillUseHandler skillUseHandler;
    [SerializeField] private TargetAcquisition targetAcquisition;
    [SerializeField] private Image skillImage;
    [SerializeField] private TMP_Text skillTextMeshPro;
    [SerializeField] private TMP_Text[] textMeshPros;
    [SerializeField] private Image[] images;
    [SerializeField] private ModifierTypeSO[] modifierTypeSos;
    [SerializeField] private bool isLeftSkill;
    [SerializeField] private float upAngleThreshold = 30f;
    
    private Skill _currentSkill;
    private Transform _currentTarget;

    private Skill CurrentSkill =>
        isLeftSkill ? skillUseHandler.CurrentLeftHandSkill : skillUseHandler.CurrentRightHandSkill;
    
    private const float MAX_LEVEL = 12f;
    
    
    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if (Vector3.Angle(emitterTransform.forward, Vector3.up) > upAngleThreshold)
        {
            DisableUI();
            return;
        }

        _currentTarget = targetAcquisition.AcquireTarget(cameraTransform);
        if (!_currentTarget || !_currentTarget.Equals(interactorTransform))
        {
            DisableUI();
            return;
        }
        
        if (!CurrentSkill)
        {
            DisableUI();
            return;
        }
        
        SetupUI();
        EnableUI();
    }

    private void Initialize()
    {
        if (!cameraTransform)
        {
            var mainCamera = Camera.main;
            if (mainCamera != null) cameraTransform = mainCamera.transform;
        }
        
        for (var i = 0; i < modifierTypeSos.Length; i++)
        {
            var modifierType = modifierTypeSos[i];
            images[i].color = modifierType.Color;
        }
        
        DisableUI();
    }
    
    private void EnableUI()
    {
        canvas.SetActive(true);
    }

    private void DisableUI()
    {
        canvas.SetActive(false);
    }

    private void SetupUI()
    {
        _currentSkill = CurrentSkill;

        skillImage.sprite = CurrentSkill.SkillSo.cooldownUiSprite;
        skillImage.color = CurrentSkill.SkillSo.uiColor;
        skillTextMeshPro.text = CurrentSkill.SkillSo.SkillName;

        var modifierData = _currentSkill.ModifierData;
        for (var i = 0; i < modifierTypeSos.Length; i++)
        {
            var modifierType = modifierTypeSos[i];
            images[i].fillAmount = modifierData.GetCurrentLevel(modifierType) / MAX_LEVEL;
            textMeshPros[i].text = $"{modifierType.Name}: {modifierData.GetCurrentValue(modifierType)}";
        }
    }
}
