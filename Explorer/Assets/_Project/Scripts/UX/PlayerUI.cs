using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform playAreaTransform;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TargetAcquisition targetAcquisition;
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private TMP_Text[] textMeshPros;
    [SerializeField] private Image[] images;
    [SerializeField] private SkillContainer[] skillContainers;

    private Transform _currentTarget;
    private Dictionary<SkillContainer, Sprite> _sprites = new Dictionary<SkillContainer, Sprite>();
    private Dictionary<SkillContainer, Color> _colors = new Dictionary<SkillContainer, Color>();

    private const int LEFT_BACK_HOLSTER = 0;
    private const int LEFT_BACK_WAIST_HOLSTER = 1;
    private const int RIGHT_BACK_HOLSTER = 2;
    private const int RIGHT_WAIST_HOLSTER = 3;
    private const float MAX_LEVEL = 12f;


    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        _currentTarget = targetAcquisition.AcquireTarget(cameraTransform);
        if (!_currentTarget || !_currentTarget.Equals(thisTransform))
        {
            DisableUI();
            return;
        }

        SetupUI();
        EnableUI();
    }

    private void LateUpdate()
    {
        var transformPosition = thisTransform.position;
        var x = transformPosition.x;
        var y = playAreaTransform.position.y;
        var z = transformPosition.z;
        thisTransform.position = new Vector3(x, y, z);
    }

    private void Initialize()
    {
        if (!cameraTransform)
        {
            var mainCamera = Camera.main;
            if (mainCamera != null) cameraTransform = mainCamera.transform;
        }

        _sprites = new Dictionary<SkillContainer, Sprite>();
        _colors = new Dictionary<SkillContainer, Color>();
        foreach (var skillContainer in skillContainers)
        {
            _sprites.Add(skillContainer, skillContainer.SkillSo.cooldownUiSprite);
            _colors.Add(skillContainer, skillContainer.SkillSo.uiColor);
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
        images[LEFT_BACK_HOLSTER].sprite = _sprites[holsterManager.leftBackSkill];
        images[LEFT_BACK_WAIST_HOLSTER].sprite = _sprites[holsterManager.leftWaistSkill];
        images[RIGHT_BACK_HOLSTER].sprite = _sprites[holsterManager.rightBackSkill];
        images[RIGHT_WAIST_HOLSTER].sprite = _sprites[holsterManager.rightWaistSkill];
        
        images[LEFT_BACK_HOLSTER].color = _colors[holsterManager.leftBackSkill];
        images[LEFT_BACK_WAIST_HOLSTER].color = _colors[holsterManager.leftWaistSkill];
        images[RIGHT_BACK_HOLSTER].color = _colors[holsterManager.rightBackSkill];
        images[RIGHT_WAIST_HOLSTER].color = _colors[holsterManager.rightWaistSkill];
        
        textMeshPros[LEFT_BACK_HOLSTER].text = holsterManager.leftBackSkill.skillName;
        textMeshPros[LEFT_BACK_WAIST_HOLSTER].text = holsterManager.leftWaistSkill.skillName;
        textMeshPros[RIGHT_BACK_HOLSTER].text = holsterManager.rightBackSkill.skillName;
        textMeshPros[RIGHT_WAIST_HOLSTER].text = holsterManager.rightWaistSkill.skillName;
    }
}
