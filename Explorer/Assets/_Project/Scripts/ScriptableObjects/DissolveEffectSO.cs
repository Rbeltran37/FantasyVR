using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_DissolveEffect", menuName = "ScriptableObjects/SharedData/DissolveEffectSO", order = 1)]
public class DissolveEffectSO : ScriptableObject
{
    public float dissolveDelay = 1;
    public float dissolveSpeed = .01f;
    public float dissolveTimeStep = .04f;
    public float appearSpeed = .01f;
    public float appearTimeStep = .04f;
    
    private int _dissolveId = -1;
    
    public int DissolveId => _dissolveId;
    public float MinDissolveValue => MIN_DISSOLVE_VALUE;
    public float MaxDissolveValue => MAX_DISSOLVE_VALUE;

    private const int MIN_DISSOLVE_VALUE = 0;
    private const int MAX_DISSOLVE_VALUE = 1;
    private const string DISSOLVE_PROPERTY_NAME = "_Dissolve";


    //TODO explore only calling once
    public void SetNameIds()
    {
        _dissolveId = Shader.PropertyToID(DISSOLVE_PROPERTY_NAME);
    }
}
