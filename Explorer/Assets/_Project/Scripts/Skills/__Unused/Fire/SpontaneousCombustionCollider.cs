using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class SpontaneousCombustionCollider : MonoBehaviour
{
    public int headDamage = 3;
    public int bodyDamage = 3;
    public float hitForce = 0;
    public float lifetime = 1;

    
    private List<Transform> enemiesHit = new List<Transform>();


    private void OnEnable() {

        Destroy(gameObject, lifetime);
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
                    damageHandler.triggerHit(broadcaster.muscleIndex, bodyDamage, headDamage, hitForce, -direction);
                    enemiesHit.Add(puppet.transform);
                }*/
            }
        }
    }
}
