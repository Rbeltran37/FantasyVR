using UnityEngine;

[CreateAssetMenu(fileName = "_HandAnimationHandlerSO", menuName = "ScriptableObjects/CharacterData/HandAnimationHandlerData", order = 0)]
public class HandAnimationHandlerData : ScriptableObject
{
    public RangedFloat openHand;
    [Range(0, .5f)] public float openHandSpeed = .12f;
    [Range(0, .1f)] public float openHandInterval = .02f;
    
    public RangedFloat closeHand;
    [Range(0, .5f)] public float closeHandSpeed = .08f;
    [Range(0, .1f)] public float closeHandInterval = .02f;
    
    public RangedFloat restHand;
    [Range(0, .5f)] public float restHandSpeed = .12f;
    [Range(0, .1f)] public float restHandInterval = .02f;
    
    public RangedFloat grabHand;
    [Range(0, .5f)] public float grabHandSpeed = .14f;
    [Range(0, .1f)] public float grabHandInterval = .02f;
    
    public string leftHandString = "Left_Hand";
    public string rightHandString = "Right_Hand";
}