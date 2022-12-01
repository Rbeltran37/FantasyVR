using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class BusterShotCollider : MonoBehaviour
{
    public GameObject explodeFX;
    public int bodyDamage = 1;
    public int headDamage = 1;
    public float force = 100;
    public int numHits = 1;

    private List<Transform> enemiesHit = new List<Transform>();


    private void OnTriggerEnter(Collider other) {

        var broadcaster = other.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster) {

            var puppet = broadcaster.puppetMaster;
            if (puppet && !enemiesHit.Contains(puppet.transform)) {

                /*
                var damageHandler = puppet.transform.parent.GetComponentInChildren<PuppetDamageHandler>();
                if (damageHandler) {
                    damageHandler.triggerHit(broadcaster.muscleIndex, bodyDamage, headDamage, force, transform.forward);
                    enemiesHit.Add(puppet.transform);
                    numHits--;
                }
                */
            }
        }
        else {

            explode();
        }

        if (numHits <= 0)
            explode();
    }

    private void explode() {

        if (explodeFX) {

            explodeFX.transform.SetParent(null);
            explodeFX.SetActive(true);
            Destroy(explodeFX, 2);
        }
        Destroy(gameObject, 1);
        gameObject.SetActive(false);
    }
}
