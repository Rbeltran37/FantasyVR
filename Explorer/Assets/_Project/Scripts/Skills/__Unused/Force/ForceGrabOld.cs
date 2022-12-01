/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class ForceGrab : MonoBehaviour
{
    public Transform headset;
    public Transform referencePosition;
    public float grabRange = 14;
    public float grabRadius = 1;
    public float grabSpeed = .2f;
    public float grabDistanceLimit = 20;
    public LayerMask enemyLayer;
    public AudioSource audioSource;
    public Vector3 referenceVector;

    private MuscleCollisionBroadcaster targetBroadcaster;
    private Vector3 initialPosition;
    private bool isGrabbing = false;


    private void FixedUpdate() {
        
        if (isGrabbing) {

            referencePosition.position = initialPosition;

            if (!targetBroadcaster) {

                getTarget();
            }
            else {

                if (Vector3.Distance(targetEnemyAI.transform.position, transform.position) > grabDistanceLimit) {
                    endGrab();
                    return;
                }

                grabTarget(targetEnemyAI.transform, targetBroadcaster);
            }
        }
        else {

            endGrab();
        }
    }


    public void use() {

        isGrabbing = true;
        audioSource.enabled = true;
        referencePosition.gameObject.SetActive(true);
    }

    public void endUse() {

        endGrab();
    }

    private void getTarget() {

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, grabRadius, transform.forward, out hit, grabRange, enemyLayer)) {

            var broadcaster = hit.transform.GetComponent<MuscleCollisionBroadcaster>();
            if (broadcaster) {

                targetBroadcaster = broadcaster;
                if (targetBroadcaster) {

                    targetEnemyAI = targetBroadcaster.puppetMaster.transform.parent.GetComponentInChildren<EnemyAI>();
                    if (targetEnemyAI) {

                        targetEnemyAI.isForceGrabbed(true);
                    }

                    initialPosition = transform.position;
                }
            }
        }
    }


    private void endGrab() {

        isGrabbing = false;
        referencePosition.gameObject.SetActive(false);

        if (targetEnemyAI) {

            targetEnemyAI.isForceGrabbed(false);
            targetEnemyAI = null;
        }

        audioSource.enabled = false;
    }

    private void grabTarget(Transform navParent, MuscleCollisionBroadcaster broadcaster) {

        var xVector = transform.position.x - initialPosition.x;
        var yVector = transform.position.y - initialPosition.y;
        var zVector = transform.position.z - initialPosition.z;
        var targetPosition = new Vector3(navParent.position.x + xVector, navParent.position.y + yVector, navParent.position.z + zVector);

        targetEnemyAI.isForceGrabbed(true);

        // Calculate the journey length.
        var journeyLength = Vector3.Distance(targetPosition, navParent.position);
        var distanceToDash = journeyLength < grabSpeed ? journeyLength : grabSpeed;

        // Set our position as a fraction of the distance between the markers.
        navParent.position += (targetPosition - navParent.position).normalized * distanceToDash;
    }
}
*/
