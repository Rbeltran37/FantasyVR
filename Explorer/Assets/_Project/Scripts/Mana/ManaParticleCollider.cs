using System;
using System.Collections;
using UnityEngine;

public class ManaParticleCollider : MonoBehaviour
{
    [SerializeField] private float manaAmount;
    

    private void OnParticleCollision(GameObject other)
    {
        var playerMouth = other.GetComponent<PlayerMouth>();
        if (!playerMouth) return;
        
        playerMouth.AddMana(manaAmount);
    }
}
