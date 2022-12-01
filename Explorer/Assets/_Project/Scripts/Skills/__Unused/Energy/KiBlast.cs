using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class KiBlast : MonoBehaviour
{
    public GameObject kiBlastBallPrefab;
    public float lifetime = 10;
    public float sphereRadius = 4;
    public float spherecastDistance = 10;
    public LayerMask enemyLayer;
    


    public void fire() {

        var ball = Instantiate(kiBlastBallPrefab, transform.position, transform.rotation);
        if (ball) {

            ball.SetActive(true);
            Destroy(ball, lifetime);

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, spherecastDistance, enemyLayer)) {

                var broadcaster = hit.transform.GetComponent<MuscleCollisionBroadcaster>();
                if (broadcaster) {

                    var puppet = broadcaster.puppetMaster;
                    if (puppet) {

                        var ballColliderScript = ball.GetComponent<KiBlastCollider>();
                        if (ballColliderScript) {

                            ballColliderScript.target = puppet.muscles[(int)HumanBodyBones.Spine].transform;
                        }
                    }
                }
            }
        }
    }
}
