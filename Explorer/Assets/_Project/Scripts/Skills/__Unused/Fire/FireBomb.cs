using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBomb : MonoBehaviour
{
    public GameObject fireBombPrefab;
    public Transform spawnPoint;
    public FireBombCollider fireBombCollider;
    public float throwForceMultiplier = 3;
    public int bodyDamage = 3;
    public int headDamage = 3;
    public float hitForce = 0;
    public float explosionRadius = 3;
    public float lifetime = 1;

    private GameObject currentBomb;


    private void Awake() {

        updateFireBombCollider();
    }


    public void createBomb() {

        var bomb = Instantiate(fireBombPrefab, spawnPoint);
        bomb.transform.localPosition = new Vector3();
        bomb.SetActive(true);
        currentBomb = bomb;
    }

    public void throwBomb() {

        currentBomb.transform.parent = null;
    }

    public void updateFireBombCollider() {

        fireBombCollider.throwForceMultiplier = throwForceMultiplier;
        fireBombCollider.bodyDamage = bodyDamage;
        fireBombCollider.headDamage = headDamage;
        fireBombCollider.hitForce = hitForce;
        fireBombCollider.explosionRadius = explosionRadius;
        fireBombCollider.lifetime = lifetime;

        fireBombCollider.updateExplosion();
    }
}
