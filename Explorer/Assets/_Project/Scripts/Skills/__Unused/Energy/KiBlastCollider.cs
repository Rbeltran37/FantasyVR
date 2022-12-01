using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;

public class KiBlastCollider : MonoBehaviour
{
    public Transform target;
    public float turnRate = .1f;
    public float speed = .01f;
    public int bodyDamage = 1;
    public int headDamage = 1;
    public GameObject explodeFX;
    public Rigidbody rigid;
    public float force = 200;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (target) {

            honeInOnTarget();
        }

        transform.position += transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other) {

        var broadcaster = other.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster) {

            var puppet = broadcaster.puppetMaster;
            if (puppet) {
                /*var damageHandler = puppet.transform.parent.GetComponentInChildren<PuppetDamageHandler>();
                if (damageHandler) {
                    damageHandler.triggerHit(broadcaster.muscleIndex, bodyDamage, headDamage, force, transform.forward);
                }*/
            }
        }

        explode();
    }

    private void OnCollisionEnter(Collision other)
    {
        var broadcaster = other.collider.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster) {

            var puppet = broadcaster.puppetMaster;
            if (puppet) {
                /*var damageHandler = puppet.transform.parent.GetComponentInChildren<PuppetDamageHandler>();
                if (damageHandler) {
                    damageHandler.triggerHit(broadcaster.muscleIndex, bodyDamage, headDamage, force, transform.forward);
                }*/
            }
        }

        explode();
    }


    private void honeInOnTarget() {

        var heading = target.position - transform.position;
        float dirNum = isTargetAhead(heading);
        if (dirNum == 1) {

            dirNum = isTargetToTheLeftOrRight(heading);
            var amountTurnedHorizontally = 0f;
            if (dirNum == 1) {

                amountTurnedHorizontally = turnRate;
            }
            else if (dirNum == -1) {

                amountTurnedHorizontally = -turnRate;
            }

            dirNum = isTargetAboveOrBelow(heading);
            var amountTurnedVertically = 0f;
            print(dirNum);
            if (dirNum == 1) {

                amountTurnedVertically = turnRate;
            }
            else if (dirNum == -1) {

                amountTurnedVertically = -turnRate;
            }

            turn(amountTurnedVertically, amountTurnedHorizontally);
        }
    }

    // Returns 1 if target is ahead
    private float isTargetAhead(Vector3 heading) {

        return AngleDir(transform.up, heading, transform.right);
    }

    // Returns 1 if target is to the right, -1 if it is to the left
    private float isTargetToTheLeftOrRight(Vector3 heading) {

        return AngleDir(transform.forward, heading, transform.up);
    }

    // Returns 1 if target is to the above, -1 if it is below
    private float isTargetAboveOrBelow(Vector3 heading) {

        return AngleDir(transform.forward, heading, transform.right);
    }

    private void turn(float vertical, float horizontal) {

        transform.localEulerAngles+= new Vector3(vertical, horizontal, 0);
    }

    private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
        var perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f) {
            return 1f;
        }
        else if (dir < 0f) {
            return -1f;
        }
        else {
            return 0f;
        }
    }

    private void explode() {

        if (explodeFX) {

            explodeFX.transform.SetParent(null);
            explodeFX.SetActive(true);
            Destroy(explodeFX, 2);
        }
        gameObject.SetActive(false);
        Destroy(gameObject, 1);
    }
}
