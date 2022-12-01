using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class KamehamehaExplosion : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public SphereCollider sphereCollider;
    public int minBodyDamage = 5;
    public int minHeadDamage = 5;
    public int maxBodyDamage = 10;
    public int maxHeadDamage = 10;
    public float minForce = 700;
    public float maxForce = 1200;
    public float minExplosionRadius = 2;
    public float maxExplosionRadius = 5;
    public float lifetime = 2;
    public float explosionTime = 1;


    private float chargeExplosionRadius;
    private float chargeForce;
    private int chargeBodyDamage;
    private int chargeHeadDamage;
    private float maxScale = 5;
    private float explosionTimer = 0;
    private List<Transform> enemiesHit = new List<Transform>();


    private void FixedUpdate() {

        explosionTimer += Time.deltaTime;
        var currentScale = explosionTimer / explosionTime;
        var scale = maxScale * currentScale;
        transform.localScale = new Vector3(scale, scale, scale);
        if (explosionTimer > explosionTime) {

            if (meshRenderer) {
                meshRenderer.enabled = false;
            }

            if (sphereCollider) {
                sphereCollider.enabled = false;
            }

            this.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other) {

        var broadcaster = other.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster) {

            var puppet = broadcaster.puppetMaster;
            if (puppet && !enemiesHit.Contains(puppet.transform)) {

                /*var damageHandler = puppet.transform.parent.GetComponentInChildren<PuppetDamageHandler>();
                if (damageHandler) {

                    var heading = transform.position - other.transform.position;
                    var distance = heading.magnitude;
                    var direction = heading / distance;
                    damageHandler.triggerHit(broadcaster.muscleIndex, chargeBodyDamage, chargeHeadDamage, chargeForce, -direction);
                    enemiesHit.Add(puppet.transform);
                }*/
            }
        }
    }


    public void setExplosionParameters(float chargeFactor) {

        chargeExplosionRadius = chargeFactor * (maxExplosionRadius - minExplosionRadius);
        maxScale = minExplosionRadius + chargeExplosionRadius;

        chargeForce = chargeFactor * (maxForce - minForce);
        chargeForce = minForce + chargeForce;

        chargeBodyDamage = (int) (chargeFactor * (maxBodyDamage - minBodyDamage));
        chargeBodyDamage += minBodyDamage;

        chargeHeadDamage = (int) (chargeFactor * (maxHeadDamage - minHeadDamage));
        chargeHeadDamage += minBodyDamage;
    }
}
