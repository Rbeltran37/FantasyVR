using System;
using System.Collections;
using System.Collections.Generic;
using DigitalOpus.MB.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class SunFlare : MonoBehaviour
{
    public Transform flare;
    public MeshRenderer renderer;
    public Transform lightSource;
    public Transform head;
    public float maxAngle;
    public float maxTransparency;
    public float maxSize;
    public float minSize;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Angle(head.forward, lightSource.position) <= maxAngle)
        {
            UpdateFlareTransparency();
            UpdateFlareSize();
        }
    }
    
    private static float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void UpdateFlareSize()
    {
        float updatedSize = Remap(Vector3.Angle(head.forward, lightSource.position) , maxAngle, 0, minSize, maxSize);
        flare.localScale = new Vector3(updatedSize, updatedSize, updatedSize);
    }

    private void UpdateFlareTransparency()
    {
        float updatedTransparency = Remap(Vector3.Angle(head.forward, lightSource.position) , maxAngle, 0, 0, maxTransparency);
        renderer.material.SetFloat("_transparency", updatedTransparency);
    }
}
