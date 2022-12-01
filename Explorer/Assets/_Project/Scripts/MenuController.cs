using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Movement;
using Sirenix.OdinInspector;
using Sigtrap.VrTunnellingPro;
using UnityEngine;
using UnityEngine.UI;
using VRUiKits.Utils;

public class MenuController : MonoBehaviour
{
    [Header("Menu Orientation")]
    [SerializeField] private GameObject headLocation;

    [Header("General Settings")]
    [SerializeField] private LocomotionManager locomotionManager;
    [SerializeField] private OptionsManager handednessOptionsManager;
    [SerializeField] private OptionsManager turningOptionsManager;
    [SerializeField] private OptionsManager comfortOptionsManager;
    [SerializeField] private OptionsManager locomotionOptionsManager;
    [SerializeField] public RigidbodyStickLocomotion rigidbodyStickLocomotion;
    [SerializeField] public SmoothTurning smoothTurning;
    [SerializeField] public SnapTurning snapTurning;
    [SerializeField] private TunnellingMobile tunnellingMobile;
    [SerializeField] public Button heightCalibrationButton;
    [SerializeField] private Save save;
    [SerializeField] private UserDataReferenceHelper userDataReferenceHelper;
    
    
    
    
    private bool isRightHanded;

    private void Awake()
    {
        if (handednessOptionsManager)
        {
            handednessOptionsManager.OnOptionSelected += SelectHandedness;
        }

        if (turningOptionsManager)
        {
            turningOptionsManager.OnOptionSelected += SelectTurning;
        }
        
        if (comfortOptionsManager)
        {
            comfortOptionsManager.OnOptionSelected += SelectComfort;
        }


        LoadDataIntoSettings();
        /*SelectHandedness(1);
        SelectTurning(1);
        SelectComfort(0);*/
    }

    private void OnEnable()
    {
        LoadDataIntoSettings();
    }

    private void OnDestroy()
    {
        if (handednessOptionsManager)
        {
            handednessOptionsManager.OnOptionSelected -= SelectHandedness;
        }
        
        if (turningOptionsManager)
        {
            turningOptionsManager.OnOptionSelected -= SelectTurning;
        }
        
        if (comfortOptionsManager)
        {
            comfortOptionsManager.OnOptionSelected -= SelectComfort;
        }
    }

    private void OnDisable()
    {
        save.SavePlayer();
    }

    private void OrientateUIPosition()
    {
        Vector3 menuPos = new Vector3(0, headLocation.transform.rotation.y, 0);
        transform.position = Vector3.RotateTowards(transform.position, menuPos, 0, 0);
    }

    private void Update()
    {
        OrientateUIPosition();
    }

    public void SelectHandedness(int index)
    {
        if (!locomotionManager)
            return;
        
        if (index == 0)
        {
            isRightHanded = false;
            locomotionManager.stickAssignedThumbstick = LocomotionManager.AssignedThumbstick.Right;

            locomotionManager.snapAssignedThumbstick =
                locomotionManager.snapAssignedThumbstick == LocomotionManager.AssignedThumbstick.None
                    ? LocomotionManager.AssignedThumbstick.None
                    : LocomotionManager.AssignedThumbstick.Left;
            
            locomotionManager.smoothAssignedThumbstick = 
                locomotionManager.smoothAssignedThumbstick == LocomotionManager.AssignedThumbstick.None
                    ? LocomotionManager.AssignedThumbstick.None
                    : LocomotionManager.AssignedThumbstick.Left;
        }

        if (index == 1)
        {
            isRightHanded = true;
            locomotionManager.stickAssignedThumbstick = LocomotionManager.AssignedThumbstick.Left;

            locomotionManager.snapAssignedThumbstick =
                locomotionManager.snapAssignedThumbstick == LocomotionManager.AssignedThumbstick.None 
                    ? LocomotionManager.AssignedThumbstick.None
                    : LocomotionManager.AssignedThumbstick.Right;
            
            locomotionManager.smoothAssignedThumbstick = 
                locomotionManager.smoothAssignedThumbstick == LocomotionManager.AssignedThumbstick.None 
                    ? LocomotionManager.AssignedThumbstick.None 
                    : LocomotionManager.AssignedThumbstick.Right;
        }
        
        if(rigidbodyStickLocomotion) rigidbodyStickLocomotion.InitializeController();
        if(smoothTurning) smoothTurning.InitializeControllers();
        if(snapTurning) snapTurning.InitializeControllers();
    }

    public void SelectTurning(int index)
    {
        if (index == 0)
        {
            if (isRightHanded)
            {
                locomotionManager.snapAssignedThumbstick = LocomotionManager.AssignedThumbstick.Right;
                
                locomotionManager.smoothAssignedThumbstick =
                    LocomotionManager.AssignedThumbstick.None;
                
            }
            else
            {
                locomotionManager.snapAssignedThumbstick = LocomotionManager.AssignedThumbstick.Left;

                locomotionManager.smoothAssignedThumbstick =
                    LocomotionManager.AssignedThumbstick.None;
            }
        }

        if (index == 1)
        {
            if (isRightHanded)
            {
                locomotionManager.smoothAssignedThumbstick = LocomotionManager.AssignedThumbstick.Right;

                locomotionManager.snapAssignedThumbstick =
                    LocomotionManager.AssignedThumbstick.None;
            }
            else
            {
                locomotionManager.smoothAssignedThumbstick = LocomotionManager.AssignedThumbstick.Left;

                locomotionManager.snapAssignedThumbstick =
                    LocomotionManager.AssignedThumbstick.None;
            }
        }
        
        smoothTurning.InitializeControllers();
        snapTurning.InitializeControllers();
    }

    public void SelectComfort(int index)
    {
        if (index == 0)
        {
            locomotionManager.comfortLevel = LocomotionManager.ComfortLevel.None;
            tunnellingMobile.enabled = false;
        }
        
        //Add LOW and MEDIUM settings here. Update Options Manager...

        if (index == 1)
        {
            tunnellingMobile.enabled = true;
            locomotionManager.comfortLevel = LocomotionManager.ComfortLevel.High;
        }
    }

    [Button]
    public void LoadDataIntoSettings()
    {
        handednessOptionsManager.DeactivateOption(handednessOptionsManager.ActiveIndex());
        handednessOptionsManager.ActivateOption((int) locomotionManager.stickAssignedThumbstick-1);

        turningOptionsManager.DeactivateOption(turningOptionsManager.ActiveIndex());
        turningOptionsManager.ActivateOption(
            locomotionManager.snapAssignedThumbstick == LocomotionManager.AssignedThumbstick.None ? 1 : 0);
        
        comfortOptionsManager.DeactivateOption(comfortOptionsManager.ActiveIndex());
        comfortOptionsManager.ActivateOption(
            locomotionManager.comfortLevel == LocomotionManager.ComfortLevel.High ? 1 : 0);
        
        //The reason it is -2 is because the Locomotion Manager enum for comfort is set to contain
        //more levels of comfort but the UI does not contain them yet.
        SelectComfort((int)locomotionManager.comfortLevel-2);
    }
}
