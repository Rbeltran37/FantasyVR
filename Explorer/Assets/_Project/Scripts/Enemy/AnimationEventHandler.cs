using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RootMotion.Dynamics;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private DamageDealt leftHitCollider;
    [SerializeField] private DamageDealt rightHitCollider;
    [SerializeField] private SimpleAI simpleAi;
    [SerializeField] private List<GameObject> primaryWeapons = new List<GameObject>();
    [SerializeField] private List<GameObject> secondaryWeapons = new List<GameObject>();

    public Action PrimaryWasEquipped;
    public Action SecondaryWasEquipped;
    

    private void Start()
    {
        DisableBothHitColliders();
    }

    public void SetLeftHitCollider(DamageDealt damageDealt)
    {
        if (DebugLogger.IsNullInfo(damageDealt, this)) return;

        leftHitCollider = damageDealt;
    }
    
    public void SetRightHitCollider(DamageDealt damageDealt)
    {
        if (DebugLogger.IsNullInfo(damageDealt, this)) return;

        rightHitCollider = damageDealt;
    }

    public void AddWeapon(GameObject weapon, bool isPrimary)
    {
        if (isPrimary)
        {
            primaryWeapons.Add(weapon);
        }
        else
        {
            secondaryWeapons.Add(weapon);
        }
    }
    
    public void Clear()
    {
        primaryWeapons.Clear();
        secondaryWeapons.Clear();
    }

    public void StartAimLeft()
    {
        if (simpleAi) simpleAi.SetAttackIkValues(true, false);
    }

    public void StartAimRight()
    {
        if (simpleAi) simpleAi.SetAttackIkValues(false, true);
    }

    public void EndAim()
    {
        if (simpleAi) simpleAi.SetDefaultIkValues(true, true);
    }

    public void EnableLeftHitCollider()
    {
        if (!leftHitCollider) return;

        leftHitCollider.canDealDamage = true;
        
        if (simpleAi) simpleAi.SetAttackIkValues(true, false);
    }

    public void EnableRightHitCollider()
    {
        if (!rightHitCollider) return;

        rightHitCollider.canDealDamage = true;
        
        if (simpleAi) simpleAi.SetAttackIkValues(false, true);
    }

    public void DisableLeftHitCollider()
    {
        if (!leftHitCollider) return;

        leftHitCollider.canDealDamage = false;
        
        if (simpleAi) simpleAi.SetDefaultIkValues(true, false);
    }

    public void DisableRightHitCollider()
    {
        if (!rightHitCollider) return;

        rightHitCollider.canDealDamage = false;
        
        if (simpleAi) simpleAi.SetDefaultIkValues(false, true);
    }

    public void EnableBothHitColliders()
    {
        if (!leftHitCollider || !rightHitCollider) return;

        leftHitCollider.canDealDamage = true;
        rightHitCollider.canDealDamage = true;
        
        if (simpleAi) simpleAi.SetAttackIkValues(true, true);    
    }

    public void DisableBothHitColliders()
    {
        if (!leftHitCollider || !rightHitCollider) return;

        leftHitCollider.canDealDamage = false;
        rightHitCollider.canDealDamage = false;
        
        if (simpleAi) simpleAi.SetDefaultIkValues(true, true);
    }
    
    public void EnableHitFramesLeft(UnityEngine.AnimationEvent animationEvent)
    {
        EnableLeftHitCollider();

        StartCoroutine(DisableHitFramesLeft(animationEvent.floatParameter));
    }
    
    public void EnableHitFramesRight(UnityEngine.AnimationEvent animationEvent)
    {
        EnableRightHitCollider();

        StartCoroutine(DisableHitFramesRight(animationEvent.floatParameter));
    }

    public void DisableBothHitFrames()
    {
        StartCoroutine(DisableHitFramesLeft(0));
        StartCoroutine(DisableHitFramesRight(0));
    }

    public void EquipPrimaryWeapons()
    {
        UnEquipSecondaryWeapons();
        
        foreach (var weapon in primaryWeapons)
        {
            weapon.SetActive(true);
        }

        if (!simpleAi)
        {
            DebugLogger.Error(nameof(EquipPrimaryWeapons), $"{nameof(simpleAi)} is null. Must be set in editor.", this);
            return;
        }
        
        simpleAi.ChangeWeapons(true, false);
        
        PrimaryWasEquipped?.Invoke();
    }
    
    public void UnEquipPrimaryWeapons()
    {
        foreach (var weapon in primaryWeapons)
        {
            weapon.SetActive(false);
        }

        if (DebugLogger.IsNullError(simpleAi, this, "Must be set in editor.")) return;
        
        simpleAi.ChangeWeapons(false, false);
    }
    
    public void EquipSecondaryWeapons()
    {
        UnEquipPrimaryWeapons();
        
        foreach (var weapon in secondaryWeapons)
        {
            weapon.SetActive(true);
        }

        if (DebugLogger.IsNullError(simpleAi, this, "Must be set in editor.")) return;

        simpleAi.ChangeWeapons(false, true);
        
        SecondaryWasEquipped?.Invoke();
    }
    
    public void UnEquipSecondaryWeapons()
    {
        foreach (var weapon in secondaryWeapons)
        {
            weapon.SetActive(false);
        }

        if (DebugLogger.IsNullError(simpleAi, this, "Must be set in editor.")) return;

        simpleAi.ChangeWeapons(false, false);
    }

    private IEnumerator DisableHitFramesLeft(float time)
    {
        if (!leftHitCollider) yield break;
        
        yield return new WaitForSeconds(time);
        leftHitCollider.canDealDamage = false;
    }
    
    private IEnumerator DisableHitFramesRight(float time)
    {
        if (!rightHitCollider) yield break;
        
        yield return new WaitForSeconds(time);
        rightHitCollider.canDealDamage = false;
    }
}
