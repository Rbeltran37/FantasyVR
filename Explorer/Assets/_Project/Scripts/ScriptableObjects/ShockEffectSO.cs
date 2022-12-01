using UnityEngine;

[CreateAssetMenu(fileName = "_ShockEffectSO", menuName = "ScriptableObjects/SharedData/ShockEffectSO", order = 1)]
public class ShockEffectSO : ScriptableObject
{
    public Material shockEffectMaterial;
    public float electricityAmount = 1;
    public float electricityAmountMin = .5f;
    
    public int amountId = -1;
    
    private const string ELECTRICITY_AMOUNT = "Vector1_9921ED3C";
    
    public void SetNameIds()
    {
        amountId = Shader.PropertyToID(ELECTRICITY_AMOUNT);
    }
}