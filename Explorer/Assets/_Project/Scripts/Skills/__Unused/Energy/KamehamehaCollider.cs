using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class KamehamehaCollider : MonoBehaviour
{
    public int minBodyDamage = 5;
    public int minHeadDamage = 5;
    public int maxBodyDamage = 10;
    public int maxHeadDamage = 10;
    public float minForce = 700;
    public float maxForce = 1200;
    public float minBeamRadius = 1;
    public float maxBeamRadius = 3;
    public float minExplosionRadius = 2;
    public float maxExplosionRadius = 5;
    public float minChargeTime = 2;
    public float maxChargeTime = 5;
    public float explosionTime = 1;
    public KamehamehaExplosion kamehamehaExplosion;


    private bool hasExploded = false;


    private void Awake() {

        updateExplosion();
    }


    private void OnTriggerEnter(Collider other) {

        if (!hasExploded) {

            explode();
            print(other.transform.name);
        }
    }


    public void setBeamParameters(float chargeFactor) {

        var chargeBeamRadius = chargeFactor * (maxBeamRadius - minBeamRadius);
        var scale = minBeamRadius + chargeBeamRadius;
        transform.localScale = new Vector3(scale, scale, scale);

        updateExplosion();
        kamehamehaExplosion.setExplosionParameters(chargeFactor);
    }


    private void updateExplosion() {

        if (kamehamehaExplosion) {

            kamehamehaExplosion.minBodyDamage = minBodyDamage;
            kamehamehaExplosion.minHeadDamage = minHeadDamage;
            kamehamehaExplosion.maxBodyDamage = maxBodyDamage;
            kamehamehaExplosion.maxHeadDamage = maxHeadDamage;
            kamehamehaExplosion.minForce = minForce;
            kamehamehaExplosion.maxForce = maxForce;
            kamehamehaExplosion.minExplosionRadius = minExplosionRadius;
            kamehamehaExplosion.maxExplosionRadius = maxExplosionRadius;
            kamehamehaExplosion.explosionTime = explosionTime;
        }
    }

    private void explode() {

        if (kamehamehaExplosion) {

            kamehamehaExplosion.transform.SetParent(null);
            kamehamehaExplosion.gameObject.SetActive(true);
            Destroy(kamehamehaExplosion.gameObject, kamehamehaExplosion.lifetime);
        }
        hasExploded = true;
        gameObject.SetActive(false);
    }
}
