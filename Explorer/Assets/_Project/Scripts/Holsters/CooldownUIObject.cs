using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUIObject : MonoBehaviour
{
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private GameObject cooldownUIParent;
    [SerializeField] private Image containerImage;
    [SerializeField] private Image fillImage;

    private float _cooldownTime = DEFAULT_VALUE;
    private const int DEFAULT_VALUE = 3;


    private void Start()
    {
        HideObject();
    }

    private IEnumerator FillImage()
    {
        float currentFill = 0;
        while (currentFill < 1)
        {
            currentFill += Time.deltaTime / _cooldownTime;
            fillImage.fillAmount = currentFill;
            yield return null;
        }
        
        yield return new WaitForSeconds(holsterManager.filledBufferTime);
        HideObject();
    }
    
    private void HideObject()
    {
        if (fillImage.fillAmount < 1)
            return;
        
        cooldownUIParent.SetActive(false);
        fillImage.fillAmount = 0;
    }
    
    [Button]
    public void StartCooldown()
    {
        cooldownUIParent.SetActive(true);
        StartCoroutine(FillImage());
    }

    public void SetupNewCooldownUi(Skill skill)
    {
        if (DebugLogger.IsNullError(skill, this)) return;
        
        var skillSo = skill.SkillSo;
        if (DebugLogger.IsNullError(skillSo, this)) return;

        containerImage.sprite = skillSo.cooldownUiSprite;
        fillImage.sprite = skillSo.cooldownUiSprite;
        fillImage.color = skillSo.uiColor;

        SetCooldownTime(skill.Cooldown);
    }

    public void SetCooldownTime(float time)
    {
        _cooldownTime = time;
    }
}
