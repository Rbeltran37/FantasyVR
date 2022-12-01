using System;
using System.Collections;
using System.Collections.Generic;
using Sigtrap.VrTunnellingPro;
using Sirenix.OdinInspector;
using UnityEngine;
using VRUiKits.Utils;

public class UserDataReferenceHelper : MonoBehaviour
{
    public float heightValue;
    public int handednessValue;
    public int smoothTurningValue;
    public int snapTurningValue;
    public int snapTurnDegreesValue;
    public int comfortValue;
    
    [SerializeField] private LocomotionManager locomotionManager;
    
    /*
     * This will be called any time we load or save in order to set the appropriate
     * data to the Locomotion Manager.
     */
    public void UpdateSettingsData()
    {
        locomotionManager.stickAssignedThumbstick = (LocomotionManager.AssignedThumbstick) handednessValue;
        locomotionManager.smoothAssignedThumbstick = (LocomotionManager.AssignedThumbstick) smoothTurningValue;
        locomotionManager.snapAssignedThumbstick = (LocomotionManager.AssignedThumbstick) snapTurningValue;
        locomotionManager.comfortLevel = (LocomotionManager.ComfortLevel) comfortValue;
    }

    //Might need some cleanup 
    public void SetMenuSettings()
    {
        Debug.Log("Updating settings....");
        handednessValue = (int) locomotionManager.stickAssignedThumbstick;
        smoothTurningValue = (int) locomotionManager.smoothAssignedThumbstick;
        snapTurningValue = (int) locomotionManager.snapAssignedThumbstick;
        comfortValue = (int) locomotionManager.comfortLevel;
    }
}
