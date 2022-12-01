using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthParticleCollider : MonoBehaviour
{
    [SerializeField] private float healthAmount;
    

    private void OnParticleCollision(GameObject other)
    {
        var playerMouth = other.GetComponent<PlayerMouth>();
        if (!playerMouth) return;
        
        playerMouth.AddHealth(healthAmount);
    }
}
