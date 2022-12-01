using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationReferenceHelper : MonoBehaviour
{
    public AnimationEventHandler animationEventHandler;
    public SimpleAI simpleAi;
    public Handedness handedness;
    
    public enum Handedness
    {
        Left,
        Right,
        Both
    }
}
