using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    [SerializeField] private GameObject networkedPlayerPrefab;
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Instantiate player.
        var playerInstance = PhotonNetwork.Instantiate(networkedPlayerPrefab.name, Vector3.zero, Quaternion.identity, 0);
        if (!playerInstance)
        {
            DebugLogger.Error("OnSceneLoaded", $"playerInstance is null.", this);
            enabled = false;
            return;
        }
    }
}
