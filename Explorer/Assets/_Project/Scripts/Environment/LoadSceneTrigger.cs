using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneTrigger : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private string nextScene;

    private bool hasLoaded;
    
    private const string PLAYER_TAG = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (hasLoaded) return;
        
        if (other.CompareTag(PLAYER_TAG))
        {
            LoadNextScene();
        }
    }

    [Button]
    private void LoadNextScene()
    {
        if (hasLoaded) return;
        
        if (!levelLoader)
        {
            levelLoader = FindObjectOfType<LevelLoader>();
        }
        
        if (levelLoader)
        {
            levelLoader.LoadLevel(nextScene);
        }
        else
        {
            DebugLogger.Warning("LoadNextScene", $"levelLoader is null. Will load using LoadSceneAsync.", this);
            SceneManager.LoadSceneAsync(nextScene);
        }

        hasLoaded = true;
    }
}
