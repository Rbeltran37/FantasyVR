using System;
using Sigtrap.VrTunnellingPro;
using Sirenix.OdinInspector;
using UnityEngine;
using Zinnia.Data.Attribute;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData/LocomotionManager", order = 0)]
public class LocomotionManager : ScriptableObject
{
    [Header("RigidbodyStickLocomotion")]
    [EnumToggleButtons] public AssignedThumbstick stickAssignedThumbstick;
    [Range(0, 1)] public float stickLocomotionOffSensitivity = .1f;
    [Range(0, 10)] public float stickLocomotionSpeed = 3.0F;
    [Range(0, 1)] public float runAxisThreshold = .5f;
    public bool stickUseHeadsetForward = true;
    public bool useMoveInPlace = true;
    
    [Header("SmoothTurning")]
    [EnumToggleButtons] public AssignedThumbstick smoothAssignedThumbstick;
    [Range(0, 1)] public float smoothOffSensitivity = .1f;
    public float smoothTurningSpeed = 1.0F;      // Movement speed in units per second.
    
    [Header("SnapTurning")]
    [EnumToggleButtons] public AssignedThumbstick snapAssignedThumbstick;
    [EnumToggleButtons] public SnapTurnDegrees snapTurnDegrees;
    [Range(0, 1)] public float snapOffSensitivity = .1f;
    [Range(0, 1)] public float snapOnSensitivity = .85f;

    [Header("HeightAdjust")] 
    public float heightAdjustHeightOffset = .1f;
    
    [Header("Jump")]
    public float jumpForce = 5;
    public float jumpCooldown = .2f;
    public int jumps = 1;
    
    [Header("GroundCheck")]
    public float groundOffset = .1f;
    public float fallingRaycastDistance = .15f;
    public float groundedRaycastDistance = .15f;
    public LayerMask groundLayers;
    
    [Header("CombatDash")]
    public float dashDistance = 15;
    public float dashSpeed = .06f;
    public float dashSphereRadius = 1.5f;
    public float dashSetDistance = .7f;
    public float dashMinDistance = 1.4f;
    public float dashAgentStunTime = 1;
    public bool dashStunOnStart = false;
    public LayerMask dashTargetLayer;
    
    [Header("ComfortLevel")]
    [EnumToggleButtons] public ComfortLevel comfortLevel;
    
    [Header("MoveInPlace")]
    [Tooltip("The speed in which to move the play area.")]
    public float moveInPlaceSpeedScale = 1;
    [Tooltip("The maximun speed in game units. (If 0 or less, max speed is uncapped)")]
    public float moveInPlaceMaxSpeed = 4;
    [Tooltip("The speed in which the play area slows down to a complete stop when the engage button is released. This deceleration effect can ease any motion sickness that may be suffered.")]
    public float moveInPlaceDeceleration = 0.1f;
    public float moveInPlaceSensitivity = 0.02f;
    public bool moveInPlaceUseHeadset = true;


    public enum AssignedThumbstick
    {
        None, 
        Left, 
        Right
    }

    public enum SnapTurnDegrees
    {
        Fifteen = 15,
        Thirty = 30,
        FortyFive = 45,
        Ninety = 90,
    }
    
    public enum ComfortLevel
    {
        None, 
        Low, 
        Medium, 
        High
    }
}