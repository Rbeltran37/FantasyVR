using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class FireBreathCollider : MonoBehaviour
{
    public int headDamage = 2;
    public int bodyDamage = 2;

    public float hitForce = 0;


    private List<Transform> enemiesHit = new List<Transform>();


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
