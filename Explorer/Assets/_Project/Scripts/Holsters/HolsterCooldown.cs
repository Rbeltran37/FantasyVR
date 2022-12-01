using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HolsterCooldown : MonoBehaviour
{
    [SerializeField] private HolsterManager holsterManager;
    [SerializeField] private HolsterCollider holsterCollider;
    [SerializeField] private CooldownUIObject cooldownUiObject;
    [SerializeField] private AudioSource audioSource;
    
    public bool isReady = true;

    private GameObject _holsterModel;
    private GameObject _equipFx;
    private float _cooldownTime = 3;
    private Coroutine _coroutine;


    public bool IsCooldownActive()
    {
        return !isReady;
    }

    public void ActivateCooldown(Skill skill)
    {
        if (IsCooldownActive()) return;
        
        if (DebugLogger.IsNullError(skill, this)) return;
        if (DebugLogger.IsNullError(holsterCollider, this)) return;

        if (_equipFx) _equipFx.SetActive(true);

        SetCooldown(skill);
        
        isReady = false;
        
        if (_holsterModel) _holsterModel.SetActive(false);
    }

    public void UnequipSkill()
    {
        if (_coroutine != null) return;
        
        _coroutine = StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        if (cooldownUiObject) cooldownUiObject.StartCooldown();

        yield return new WaitForSeconds(_cooldownTime);
        
        isReady = true;
        
        if (_holsterModel) _holsterModel.SetActive(true);
        if (_equipFx) _equipFx.SetActive(false);
        
        if (audioSource)
        {
            audioSource.clip = holsterManager.cooldownClip;
            audioSource.Play();
        }

        _coroutine = null;
    }

    public void SetupNewCooldown(Skill skill, GameObject holsterModel, GameObject equipFx)
    {
        if (DebugLogger.IsNullError(skill, this)) return;
        
        SetCooldown(skill);
        
        _holsterModel = holsterModel;
        _equipFx = equipFx;
        
        if (DebugLogger.IsNullError(cooldownUiObject, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(skill, this)) return;
        
        cooldownUiObject.SetupNewCooldownUi(skill);
        SetupFx(skill);
    }
    
    private void SetupFx(Skill skill)
    {
        ActivateCooldown(skill);
        UnequipSkill();
    }

    private void SetCooldown(Skill skill)
    {
        _cooldownTime = skill.Cooldown;
        
        if (DebugLogger.IsNullWarning(cooldownUiObject, this, "Must be set in editor.")) return;
        
        cooldownUiObject.SetCooldownTime(_cooldownTime);
    }
}
