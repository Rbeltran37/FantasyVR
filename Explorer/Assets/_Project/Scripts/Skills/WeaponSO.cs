using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_WeaponSO", menuName = "ScriptableObjects/Skills/WeaponData", order = 1)]
public class WeaponSO : SkillSO
{
    public GameObject weaponPrefab;
    public Vector3 localPosition;
    public Vector3 localEuler;
    public float weaponRigidMass = 15f;
    public float minHitVelocity = 4;
    public float hitBufferTime = .3f;
    public bool isDefaultLocalXScalePositive = true;
    public SimpleHapticEvent HitHapticEvent;

    public void PlayHitHaptics(ControllerHaptics controllerHaptics)
    {
        if (DebugLogger.IsNullError(HitHapticEvent, this)) return;
        
        HitHapticEvent.Play(controllerHaptics);
    }
}
