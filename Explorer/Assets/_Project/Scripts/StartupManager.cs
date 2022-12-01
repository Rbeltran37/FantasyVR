using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StartupManager : MonoBehaviour
{
    [SerializeField] private DragonReveal dragonReveal;
    [SerializeField] private ControllerHighlights controllerHighlights;
    [SerializeField] private CalibrationInitiation calibrationInitiation;
    private const string MENU_SCENE = "__Cody_Menu";
    private float timeToWait = 2.0f;
    public PersistentAudioMixer audioMixer;

    private void Start()
    {
        PhotonNetwork.OfflineMode = true;
        StartCoroutine((StartupScene()));
    }

    private IEnumerator StartupScene()
    {
        yield return Waiting(timeToWait);
        yield return dragonReveal.MoveFromTo();
        yield return controllerHighlights.Blink();
        yield return calibrationInitiation.TriggerCheck();
        yield return Waiting(timeToWait);
        yield return Finito();
        yield return dragonReveal.StartFireBreath();
    }

    private IEnumerator Waiting(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
    }

    private IEnumerator Finito()
    {
        SceneHandler.Instance.StartLoadTransition(MENU_SCENE);
        audioMixer.Fade();
        yield return null;
    }
}
