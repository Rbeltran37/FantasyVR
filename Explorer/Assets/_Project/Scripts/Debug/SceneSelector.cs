using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    private const string PREFAB = "[SceneManager]";
    
    
    public static void LoadSceneMode(string sceneName) {

        var levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader)
        {
            levelLoader.LoadLevel(sceneName);
        }
        else
        {
            var levelLoaderGameObject = Instantiate(Resources.Load(PREFAB)) as GameObject;
            if (!(levelLoaderGameObject is null))
            {
                levelLoader = levelLoaderGameObject.GetComponent<LevelLoader>();
                if (levelLoader)
                {
                    var mainCamera = Camera.main;
                    if (mainCamera != null)
                    {
                        levelLoader.mainCamera = mainCamera;
                        
                        var screenFade = mainCamera.GetComponent<OVRScreenFade>();
                        if (!screenFade)
                        {
                            screenFade = mainCamera.gameObject.AddComponent<OVRScreenFade>();
                            //levelLoader.SetOvrScreenFade(screenFade);
                        }
                        
                        var xrRig = mainCamera.transform.parent;
                        if (xrRig)
                        {
                            levelLoader.xrRig = xrRig.gameObject;
                            var ovrManager = xrRig.GetComponent<OVRManager>();
                            if (!ovrManager)
                            {
                                ovrManager = xrRig.gameObject.AddComponent<OVRManager>();
                                if (ovrManager)
                                {
                                    ovrManager.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
                                }
                            }
                        }
                    }

                    Debug.Log("Loading...");
                    levelLoader.LoadLevel(sceneName);
                }
            }
            else
            {
                
                Debug.Log("LevelLoader script not found in scene");
                SceneManager.LoadSceneAsync(sceneName);
            }
        }
    }
}
