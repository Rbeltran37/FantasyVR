using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Voice.PUN;
using System.Runtime.CompilerServices;
using Oculus.Platform;
using Photon.Voice.Unity;
using Photon.Voice;

public class AudioValidator : MonoBehaviour
{
    [SerializeField] private PhotonVoiceView photonVoiceView;
    [SerializeField] private Recorder recorder;
    [SerializeField] private Speaker speaker;
    [SerializeField] private PhotonVoiceNetwork pvn;

    // Start is called before the first frame update
    void Start()
    {
        pvn = GameObject.Find("Voice Controller").GetComponent<PhotonVoiceNetwork>();

        if (pvn == null)
        {
            Debug.Log("Photon Voice Network not found or missing.");
            return;
        }

        recorder = GameObject.Find("Voice Controller").GetComponent<Recorder>();

        if (recorder == null)
        {
            Debug.Log("Photon Recorder not found or missing.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Button]
    public void InitializeVoiceView()
    {
        photonVoiceView.Init();
    }

    [Button]
    public void PvnDisconnect()
    {
        pvn.Disconnect();
        Debug.Log("Voice network disconnected.");
    }

    [Button]
    public void PvnConnect()
    {
        pvn.ConnectUsingSettings();
        Debug.Log("Voice network connecting");
    }

    [Button]
    public void GetVoiceNetworkInfo()
    {
        Debug.Log("Client State: " + pvn.ClientState);
        Debug.Log("Voice State: " + pvn.VoiceClient);

        Debug.Log("Lobby: " + pvn.Client.CurrentLobby.Name);
        Debug.Log("Room: " + pvn.Client.CurrentRoom.Name);
        Debug.Log("Server Address: " + pvn.Client.CurrentServerAddress);
        Debug.Log("Client App ID: " + pvn.Client.AppId);
        Debug.Log("Client App Version: " + pvn.Client.AppVersion);
    }

    [Button]
    public void GetPhotonViewInfo()
    {
        Debug.Log("Photon View is enabled: " + photonVoiceView.isActiveAndEnabled);
        Debug.Log("Photon View is setup: " + photonVoiceView.IsSetup);
    }

    [Button]
    public void GetSpeakerInfo()
    {
        Debug.Log("Speaker is enabled" + photonVoiceView.SpeakerInUse.isActiveAndEnabled);
        Debug.Log("Speaker is playing: " + photonVoiceView.SpeakerInUse.IsPlaying);
        Debug.Log("Speaker is Linked: " + photonVoiceView.SpeakerInUse.IsLinked);
    }

    [Button]
    public void GetRecorderInfo()
    {
        Debug.Log("Recorder is enabled: " + photonVoiceView.RecorderInUse.isActiveAndEnabled);
        Debug.Log("Recorder is recording: " + photonVoiceView.RecorderInUse.IsRecording);
        Debug.Log("Recorder is transmitting: " + photonVoiceView.RecorderInUse.IsCurrentlyTransmitting);
    }

    [Button]
    public void RestartPhotonVoiceView()
    {
        photonVoiceView.Init();
    }

    [Button]
    public void RestartSpeakerPlayback()
    {
        photonVoiceView.SpeakerInUse.RestartPlayback();
    }

    [Button]
    public void RestartRecorder()
    {
        photonVoiceView.RecorderInUse.RestartRecording();
    }

    [Button]
    public void StartRecording()
    {
        photonVoiceView.RecorderInUse.StartRecording();
    }


}
