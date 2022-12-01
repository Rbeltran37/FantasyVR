using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion;
using TMPro;
using UnityEngine;

public class AnimRecorder : MonoBehaviour
{
    public HumanoidBaker animationRecorder;
    public TextMeshProUGUI uiPanel;

    private void Awake()
    {
        if(!animationRecorder)
            DebugLogger.Error("Awake", "Missing Humanoid Baker", this);
    }

    public void StartRecording()
    {
        if(animationRecorder)
            animationRecorder.StartBaking();
        
        DisplayRecording();
    }

    public void StopRecording()
    {
        if(animationRecorder)
            animationRecorder.StopBaking();
        
        ResetText();
    }

    private void DisplayRecording()
    {
        uiPanel.text = "Recording....";
    }

    private void ResetText()
    {
        uiPanel.text = "Press the 'a' button to record.";
    }
}
