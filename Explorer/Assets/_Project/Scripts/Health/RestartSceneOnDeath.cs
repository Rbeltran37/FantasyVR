using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartSceneOnDeath : MonoBehaviour
{
    [SerializeField] private Health health;
    
    [SerializeField] private float restartDelay = 3;

    private void Awake()
    {
        if (health)
            health.WasKilled += RestartScene;
    }

    private void OnDestroy()
    {
        if (health)
            health.WasKilled -= RestartScene;
    }

    private void RestartScene()
    {
        StartCoroutine(RestartSceneCoroutine());
    }

    private IEnumerator RestartSceneCoroutine()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
