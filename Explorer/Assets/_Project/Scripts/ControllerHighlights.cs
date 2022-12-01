using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ControllerHighlights : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer leftGripMeshRenderer;
    [SerializeField] private SkinnedMeshRenderer rightGripMeshRenderer;
    private Material material;
    [SerializeField] private Material[] leftMaterials;
    [SerializeField] private Material[] rightMaterials;
    [SerializeField] private float speed = 1;
    [SerializeField] private float brightnessMultiplier = .5f;
    [SerializeField] private bool initiateBlink = true;
    [SerializeField] private Color buttonHighlightColor;
    [SerializeField] private CalibrationInitiation calibrationInitiation;
    [SerializeField] private Animator textAnimator;
    private static readonly int _IsActivated = Animator.StringToHash("isActivated");
    private bool isBlinking = false;

    private void Start()
    {
        if (leftGripMeshRenderer && rightGripMeshRenderer)
        {
            leftMaterials = leftGripMeshRenderer.materials;
            rightMaterials = rightGripMeshRenderer.materials;
        }
        else
        {
            Debug.Log("Skinned Mesh Renderer not found on one of the controllers.");
        }
    }

    private void Update()
    {
        if (isBlinking)
        {
            double t = (Mathf.Sin(Time.time * speed) + 1) / 2.0;
            
            leftGripMeshRenderer.materials[1].color =
                Color.Lerp(Color.black, buttonHighlightColor * brightnessMultiplier, (float) t);
                
            rightGripMeshRenderer.materials[1].color =
                Color.Lerp(Color.black, buttonHighlightColor * brightnessMultiplier, (float) t);
        }

        /*leftGripMeshRenderer.materials[1].color = Color.black;
        rightGripMeshRenderer.materials[1].color = Color.black;*/
    }

    public IEnumerator Blink()
    {
        if (leftMaterials.Length < 2 || rightMaterials.Length < 2)
        {
            Debug.Log("Highlight material not applied to one of the skinned mesh renderers.");
            yield return null;
        }
        
        textAnimator.SetBool(_IsActivated, true);
        isBlinking = true;
        yield return null;
    }
}