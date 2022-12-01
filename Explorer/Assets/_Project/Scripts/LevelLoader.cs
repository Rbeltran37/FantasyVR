using Sirenix.OdinInspector;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject xrRig;
    private AsyncOperation operation;
    public OVROverlay cubemapOverlay;
    public OVROverlay loadingTextQuadOverlay;
    public Camera mainCamera;
    public float distanceFromCamToLoadText;
    [SerializeField] private OVRScreenFade screenFade;
    public string sceneName = "Start Menu Blockout";
    
    public void LoadLevel(string scene)
    {
        StartCoroutine(LoadAsynchronously(scene));
    }

    public void LoadNetworkedLevel(string scene)
    {
        StartCoroutine(LoadNetworkedScene(scene));
    }

    public void StartOverlayWhenJoiningRoom(string roomName)
    {
        StartCoroutine(InitiateOverlayOnJoinedRoom(roomName));
    }
    
    public void DisableOverlayOnJoinedRoom()
    {
        StartCoroutine(TurnOffOverlayOnJoinedRoom());
    }

    public void RPCInitiateLoadTransition()
    {
        Debug.Log("Load Transition RPC being called.");
        StartCoroutine(InitiateLoadTransition());
    }
    
    private IEnumerator TurnOffOverlayOnJoinedRoom()
    {
        TurnOffOverlay();
        yield return new WaitForSeconds(.1f);
    }

    private IEnumerator InitiateOverlayOnJoinedRoom(string roomName)
    {
        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);
        
        TurnOnOverlay();
        
        yield return new WaitForSeconds(.1f);
        
        PhotonNetwork.JoinRoom(roomName);
    }

    private IEnumerator InitiateLoadTransition()
    {
        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);
        
        TurnOnOverlay();

        //Wait for Overlay to fully load without weird glitch on transition. 
        yield return new WaitForSeconds(.1f);

        //Start loading Async in the background
        //operation = SceneManager.LoadSceneAsync(scene);

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return null;
        }

        //Loading is done above and we wait for activation which has a delay and can give glitch.
        yield return new WaitForSeconds(.1f);

        //Turn off overlay to resume in new scene
        TurnOffOverlay();
        //screenFade.FadeIn();
        //print("fading in");
        DestroyImmediate(xrRig);
        
        //Get new rig to setup for new scene.
        xrRig = GameObject.Find("UnityXRCameraRig");
        OVRManager manager = xrRig.AddComponent<OVRManager>();
        manager.useRecommendedMSAALevel = false;
        manager.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;

        mainCamera = xrRig.GetComponentInChildren<Camera>();
        screenFade = mainCamera.GetComponent<OVRScreenFade>();
        
        xrRig.transform.SetParent(this.transform.parent);
    }
    
    
    IEnumerator LoadNetworkedScene(string scene)
    {
        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);
        
        TurnOnOverlay();

        //Wait for Overlay to fully load without weird glitch on transition. 
        yield return new WaitForSeconds(.1f);

        //Start loading Async in the background
        //operation = SceneManager.LoadSceneAsync(scene);
        PhotonNetwork.LoadLevel(scene);
        
        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return null;
        }

        //Loading is done above and we wait for activation which has a delay and can give glitch.
        yield return new WaitForSeconds(.1f);

        //Turn off overlay to resume in new scene
        TurnOffOverlay();
        //screenFade.FadeIn();
        //print("fading in");
        DestroyImmediate(xrRig);
        
        //Get new rig to setup for new scene.
        xrRig = GameObject.Find("UnityXRCameraRig");
        OVRManager manager = xrRig.AddComponent<OVRManager>();
        manager.useRecommendedMSAALevel = false;
        manager.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;

        mainCamera = xrRig.GetComponentInChildren<Camera>();
        screenFade = mainCamera.GetComponent<OVRScreenFade>();
        
        xrRig.transform.SetParent(this.transform.parent);
        
    }

    IEnumerator LoadAsynchronously(string scene)
    {
        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);
        
        TurnOnOverlay();

       //Wait for Overlay to fully load without werid glitch on transition. 
        yield return new WaitForSeconds(.1f);

        //Start loading Async in the background
        operation = SceneManager.LoadSceneAsync(scene);
        
        //If its not done loading go to next frame and proceed
        while (!operation.isDone)
        {
            //This tracks the percentage of the loading and adjusts slider. 
            /*float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;*/
            
            yield return null;
        }

        //Loading is done above and we wait for activation which has a delay and can give glitch.
        yield return new WaitForSeconds(.1f);

        //Turn off overlay to resume in new scene
        TurnOffOverlay();
        //screenFade.FadeIn();
        //print("fading in");
        DestroyImmediate(xrRig);

        if (!mainCamera)
        {
            DebugLogger.Error("LoadAsynchronously", "No mainCamera in scene.", this);
            yield break; 
        }
        
        var cameraGameObject = mainCamera.gameObject;
        if (!cameraGameObject)
        {
            DebugLogger.Error("LoadAsynchronously", "cameraGameObject is null.", this);
            yield break; 
        }

        var cameraParent = cameraGameObject.transform.parent;
        if (!cameraParent)
        {
            DebugLogger.Error("LoadAsynchronously", "cameraParent is null.", this);
            yield break; 
        }

        xrRig = cameraParent.gameObject;
        
        //Get new rig to setup for new scene.
        //xrRig = GameObject.Find("UnityXRCameraRig");
        OVRManager manager = xrRig.AddComponent<OVRManager>();
        manager.useRecommendedMSAALevel = false;
        manager.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;

        mainCamera = xrRig.GetComponentInChildren<Camera>();
        screenFade = mainCamera.GetComponent<OVRScreenFade>();
        
        xrRig.transform.SetParent(this.transform.parent);
    }

    //Sets the orientation of the cubemap and centers the loading overlay
    private void TurnOnOverlay()
    {
        Transform camTransform = mainCamera.transform;
        Transform uiTextOverlayTrasnform = loadingTextQuadOverlay.transform;
        Vector3 newPos = camTransform.position + camTransform.forward * distanceFromCamToLoadText;
        newPos.y = camTransform.position.y;
        uiTextOverlayTrasnform.position = newPos;
        cubemapOverlay.enabled = true;
        loadingTextQuadOverlay.enabled = true;
    }

    //Disables overlay and exit timewarp space
    private void TurnOffOverlay()
    {
        cubemapOverlay.enabled = false;
        loadingTextQuadOverlay.enabled = false;
    }
}
