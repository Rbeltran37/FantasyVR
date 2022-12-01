using UnityEngine;

[CreateAssetMenu(fileName = "Explosion", menuName = "ScriptableObjects/Skills/Abilities", order = 1)]
public class ExplosionSO : ScriptableObject
{
    public SimpleAudioEvent simpleAudioEvent;
    public float explosionRadius = 2;
    public float cooldown = 1;
    public LayerMask hittableLayers;
}