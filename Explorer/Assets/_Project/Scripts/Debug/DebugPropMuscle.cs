using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;

public class DebugPropMuscle : MonoBehaviour
{
    [SerializeField] private PropMuscle propMuscle;
    [SerializeField] private bool findAndEquipOnEnable;
    

    private void OnEnable()
    {
        if (!findAndEquipOnEnable) return;
        
        FindAndEquipProp();
    }

    [Button]
    public void FindAndEquipProp()
    {
        var props = FindObjectsOfType<PuppetMasterProp>();
        if (DebugLogger.IsNullOrEmptyDebug(props, this, $"Prop not found in scene.")) return;

        foreach (var prop in props)
        {
            if (!prop.isPickedUp)
            {
                propMuscle.currentProp = prop;
                return;
            }
        }
    }
}
