using System.Collections;
using System.Collections.Generic;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

public class VoiceLogger : MonoBehaviour
{
    public PhotonVoiceNetwork photonVoiceNetwork;
    public Speaker speaker;

    [Button]
    public void LogData()
    {
        GetComponents();
        
        DebugLogger.Info("Log Data", "Recorder initialization: " + photonVoiceNetwork.PrimaryRecorder.IsInitialized.ToString() + "\n" + 
        "Recorder recording: " + photonVoiceNetwork.PrimaryRecorder.IsRecording.ToString() + "\n" +
        "Recorder transmit: " + photonVoiceNetwork.PrimaryRecorder.TransmitEnabled.ToString() + "\n" +
        "Recorder is transmitting: " + photonVoiceNetwork.PrimaryRecorder.IsCurrentlyTransmitting.ToString() + "\n" +
        "Speaker is linked: " + speaker.IsLinked.ToString(), this);
    }

    //[Button]
    public void GetComponents()
    {
        if (!speaker || !photonVoiceNetwork)
        {
            photonVoiceNetwork = FindObjectOfType<PhotonVoiceNetwork>();
            speaker = FindObjectOfType<Speaker>();
        }
        
        if(!photonVoiceNetwork || !speaker)
            DebugLogger.Error("GetComponents", "Could not find components.", this);
    }
}
