using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;


public class LightProbeBaker : MonoBehaviour
{
    [Button]
    public void BakeLightProbes(){
        LightProbes.Tetrahedralize();
    }

    [Button]
    public void LoadNewScene(string scene){
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }
    [Button]
    public void UnloadNewScene(string scene){
        SceneManager.UnloadScene(scene);
    }
}

