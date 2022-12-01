using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropGearHandler : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private GameObject[] weaponSlots;

    private bool _hasBeenSetup;
    private Dictionary<EnemyWeaponInstance, DropGearContainer> _activeContainers = new Dictionary<EnemyWeaponInstance, DropGearContainer>();
    

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void Setup()
    {
        if (_hasBeenSetup) return;

        _hasBeenSetup = true;
        
        Subscribe();
        EnableWeaponSlots();
    }

    private void Subscribe()
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;

        health.WasKilled += Drop;
    }
    
    private void Unsubscribe()
    {
        if (DebugLogger.IsNullError(health, this, "Must be set in editor.")) return;

        health.WasKilled -= Drop;
    }
    
    private void EnableWeaponSlots()
    {
        foreach (var weaponSlot in weaponSlots)
        {
            weaponSlot.SetActive(true);
        }
    }

    public void Add(EnemyWeaponInstance enemyWeaponInstance)
    {
        if (DebugLogger.IsNullError(enemyWeaponInstance, this)) return;
        
        if (_activeContainers.ContainsKey(enemyWeaponInstance))
        {
            DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(_activeContainers)} contains Key {nameof(enemyWeaponInstance)}={enemyWeaponInstance}");
            return;
        }
        
        _activeContainers.Add(enemyWeaponInstance, enemyWeaponInstance.DropGearContainer);
    }

    private void Drop()
    {
        foreach (var value in _activeContainers.Values)
        {
            if (!value.gameObject.activeSelf) continue;
            
            value.SpawnDropGear();
        }

        DisableWeaponSlots();
    }

    private void DisableWeaponSlots()
    {
        foreach (var weaponSlot in weaponSlots)
        {
            weaponSlot.SetActive(false);
        }
    }

    public void Clear()
    {
        _hasBeenSetup = false;
        
        Unsubscribe();
        
        _activeContainers.Clear();
    }
}
