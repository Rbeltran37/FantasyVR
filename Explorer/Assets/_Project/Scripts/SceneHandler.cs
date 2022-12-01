using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : Singleton<SceneHandler>
{
    [SerializeField] private GameObject ovrManager;
    [SerializeField] private GameObject loadScreenComponents;
    [SerializeField] private OVRScreenFade screenFade;
    [SerializeField] private Color fadeoutColor = Color.white;
    [SerializeField] private string sceneToLoad;
    
    private void Awake()
    {
        ovrManager = Instantiate(Resources.Load("OVR Manager") as GameObject);
        loadScreenComponents = Instantiate(Resources.Load("CompositorLayerLoadingScreen") as GameObject);
        
        if(!ovrManager)
            DebugLogger.Error("Awake", "ovr manager is missing.", this);
        
        if(!loadScreenComponents)
            DebugLogger.Error("Awake", "load screen components is missing.", this);
        
        ovrManager.transform.SetParent(transform);
        loadScreenComponents.transform.SetParent(transform);
    }

    private void Start()
    {
        ovrManager.SetActive(true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /*
     * This gets the current camera and OVR screen fade attached to it.
     * Null checks just in case we are missing something. 
     */
    private void GetLoadingObjects()
    {
        screenFade = FindObjectOfType<OVRScreenFade>();

        if(!screenFade)
            DebugLogger.Error("Start", "Unable to find OVR Screen Fade.", this);
        
        if(!loadScreenComponents)
            DebugLogger.Error("Start", "Loading Screen components are missing. Check for cubemap overlay.", this);
    }

    /*
     * Start the fade out and load transition.
     */
    public void StartLoadTransition(string sceneToLoad)
    {
        ovrManager.SetActive(true);
        GetLoadingObjects();
        screenFade.fadeColor = fadeoutColor;
        screenFade.FadeOut();
        float fadeOutTime = screenFade.fadeTime;
        StartCoroutine(WaitForCubeMapOverlay(fadeOutTime, sceneToLoad));
    }

    /*
     * Start load transition for when you join a room from lobby. Typically the start menu.
     */
    public void StartLoadTransitionForJoiningRoom(string roomName)
    {
        ovrManager.SetActive(true);
        GetLoadingObjects();
        screenFade.fadeColor = fadeoutColor;
        screenFade.FadeOut();
        float fadeOutTime = screenFade.fadeTime;
        StartCoroutine(WaitForCubeMapOverlayForJoining(fadeOutTime, roomName));
    }
    
    public void StartLoadTransitionForJoiningRoom()
    {
        ovrManager.SetActive(true);
        GetLoadingObjects();
        screenFade.fadeColor = fadeoutColor;
        screenFade.FadeOut();
        float fadeOutTime = screenFade.fadeTime;
        StartCoroutine(WaitForCubeMapOverlayForJoining(fadeOutTime));
    }

    public void EnablePseudoLoadForLevels()
    {
        ovrManager.SetActive(true);
        GetLoadingObjects();
        screenFade.fadeColor = fadeoutColor;
        screenFade.FadeOut();
        StartCoroutine(WaitForCubeMapOverlayForPseudoLoad(GetScreenFadeTime()));
    }
    
    public void DisablePseudoLoadForLevels()
    {
        ovrManager.SetActive(false);
        GetLoadingObjects();
        screenFade.fadeColor = fadeoutColor;
        loadScreenComponents.SetActive(false);
        screenFade.FadeIn();
    }

    /*
     * We wait for the fade to complete and then enable the cubemap overlay loading screen. 
     */
    private IEnumerator WaitForCubeMapOverlay(float fadeTime, string sceneToLoad)
    {
        yield return new WaitForSeconds(fadeTime);
        loadScreenComponents.SetActive(true);
        DisableAllObjectsInScene();
        StartCoroutine(WaitToLoad(sceneToLoad));
    }
    
    /*
     * Waits to start load for overlay and fade when joining room.
     */
    private IEnumerator WaitForCubeMapOverlayForJoining(float fadeTime, string roomName)
    {
        yield return new WaitForSeconds(fadeTime);
        loadScreenComponents.SetActive(true);
        DisableAllObjectsInScene();
        StartCoroutine(WaitToLoadForJoining(roomName));
    }
    
    private IEnumerator WaitForCubeMapOverlayForJoining(float fadeTime)
    {
        yield return new WaitForSeconds(fadeTime);
        loadScreenComponents.SetActive(true);
        StartCoroutine(WaitToLoadForJoining());
    }
    
    private IEnumerator WaitForCubeMapOverlayForPseudoLoad(float fadeTime)
    {
        yield return new WaitForSeconds(fadeTime);
        loadScreenComponents.SetActive(true);
    }

    /*
     * Masterclient loads level for all clients. 
     */
    private IEnumerator WaitToLoad(string sceneToLoad)
    {
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.OfflineMode)
        {
            DebugLogger.Info("WaitToLoad", "Offline mode detected. LoadSceneAsync called.", this);
            SceneManager.LoadSceneAsync(sceneToLoad);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(sceneToLoad);
                
                /*while (PhotonNetwork.LevelLoadingProgress < 1)
                {
                    //you could implement loading bar with this logic
                    yield return null;
                }*/
            }
        }
        
        yield return new WaitForSeconds(.1f);
    }
    
    /*
     * This waits to load when joining room. Typically from a lobby like the start menu.
     */
    private IEnumerator WaitToLoadForJoining(string roomName)
    {
        yield return new WaitForSeconds(1f);
        
        PhotonNetwork.JoinRoom(roomName);

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            //you could implement loading bar with this logic
            yield return null;
        }
        
        yield return new WaitForSeconds(.1f);
    }
    
    private IEnumerator WaitToLoadForJoining()
    {
        yield return new WaitForSeconds(1f);

        PhotonNetwork.JoinRandomRoom();

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            //you could implement loading bar with this logic
            yield return null;
        }
        
        yield return new WaitForSeconds(.1f);
    }
    
    /*
     * Disable cubemap overlay loading screen when a scene is loaded. 
     */
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        loadScreenComponents.SetActive(false);
        ovrManager.SetActive(false);
    }

    private void DisableAllObjectsInScene()
    {
        //Get the main camera gameobject
        GameObject mainCameraGameObject = Camera.main.gameObject;
        
        //Get the parent gameobject (rig)
        GameObject xrRig = mainCameraGameObject.transform.parent.gameObject;
        
        //Unparent it to make it, its own root object
        xrRig.transform.SetParent(null);
        Rigidbody rigRb = xrRig.GetComponent<Rigidbody>();
        
        if(rigRb)
            rigRb.isKinematic = true;
        
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);

        foreach (var gameObject in rootObjects)
        {
            if(!gameObject.Equals(xrRig))
                gameObject.SetActive(false);
        }
    }

    public float GetScreenFadeTime()
    {
        return screenFade.fadeTime;
    }
}
