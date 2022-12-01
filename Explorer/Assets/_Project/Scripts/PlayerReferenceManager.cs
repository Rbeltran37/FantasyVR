using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerReferenceManager : MonoBehaviour
{
    [SerializeField] public Transform unityXRCameraRig;
    [SerializeField] public Transform headAnchor;
    [SerializeField] public Transform walkingHeadTarget;
    [SerializeField] public Transform leftAnchor;
    [SerializeField] public Transform rightAnchor;
    [SerializeField] public Transform headTarget;
    [SerializeField] public Transform leftTarget;
    [SerializeField] public Transform rightTarget;
    [SerializeField] public GameObject uiMenu;
    [SerializeField] public float height;
    [SerializeField] public XRRayInteractor leftRayInteractor;
    [SerializeField] public XRRayInteractor rightRayInteractor;
    [SerializeField] public MenuController menuController;
    [SerializeField] public Save save;
}
