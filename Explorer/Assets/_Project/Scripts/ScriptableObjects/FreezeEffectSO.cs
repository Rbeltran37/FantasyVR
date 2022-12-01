using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FreezeEffectData", menuName = "ScriptableObjects/SharedData/FreezeEffectData", order = 1)]
public class FreezeEffectSO : ScriptableObject
{
    public Material iceEffectMaterial;
    public float unSlowSpeed = .001f;
    public float unSlowInterval = .01f;
    public float iceColorStrAdd = 1;
    public float iceEffectRadiusSpeed = .025f;
    public float iceEffectRadiusInterval = .01f;
    public float maxRadius = 2;
    
    public int pointId = -1;
    public int radiusId = -1;
    public int iceColorStrId = -1;
    
    private const string ICE_EFFECT_ICE_COLOR_STR = "_IceColorStr";
    private const string ICE_EFFECT_RADIUS = "_Radius";
    private const string ICE_EFFECT_POINT = "_Point";
    

    //TODO explore only calling once
    public void SetNameIds()
    {
        pointId = Shader.PropertyToID(ICE_EFFECT_POINT);
        radiusId = Shader.PropertyToID(ICE_EFFECT_RADIUS);
        iceColorStrId = Shader.PropertyToID(ICE_EFFECT_ICE_COLOR_STR);
    }
}
