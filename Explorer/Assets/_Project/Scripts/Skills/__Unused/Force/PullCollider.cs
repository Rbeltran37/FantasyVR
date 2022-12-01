/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using UnityEngine.AI;

public class PullCollider : MonoBehaviour {

    public BoxCollider coll;
    public Transform handAnchor;
    public Transform playArea;

    public float range = 10;
    public float radius = .5f;
    public float speed = .6f;
    public float setPosition = .75f;

    
    private EnemyAI enemyAI;

    private void Start() {

        if (!GetComponent<BoxCollider>())
            gameObject.AddComponent<BoxCollider>();

        coll = GetComponent<BoxCollider>();
        coll.isTrigger = true;
        coll.size = new Vector3(radius * 2, radius * 2, range);
        coll.center = new Vector3(0, 0, range / 2);
        coll.enabled = false;
    }


    private void OnDisable() {

        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other) {

        if (other.GetComponent<Rigidbody>()) {

            var broadcaster = other.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();
            if (broadcaster != null) {
                
                if (broadcaster.puppetMaster.GetMuscleIndex(HumanBodyBones.Hips) == broadcaster.muscleIndex ||
                    broadcaster.puppetMaster.GetMuscleIndex(HumanBodyBones.Spine) == broadcaster.muscleIndex) {

                    if (!enemyAI) {

                        enemyAI = broadcaster.puppetMaster.transform.parent.GetComponentInChildren<EnemyAI>();
                    }

                    pullTarget(enemyAI.transform, broadcaster);
                }
            }
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<Rigidbody>()) {
            var broadcaster = other.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

            if (broadcaster != null) {

                if (broadcaster.puppetMaster.GetMuscleIndex(HumanBodyBones.Hips) == broadcaster.muscleIndex ||
                    broadcaster.puppetMaster.GetMuscleIndex(HumanBodyBones.Spine) == broadcaster.muscleIndex) {

                    if (enemyAI && enemyAI == broadcaster.puppetMaster.transform.parent.GetComponentInChildren<EnemyAI>()) {

                        enemyAI.isForcePulled(false);

                        if (enemyAI.isActiveAndEnabled) {

                            broadcaster.puppetMaster.muscles[(int)HumanBodyBones.Hips].rigidbody.velocity = new Vector3();
                            broadcaster.puppetMaster.muscles[(int)HumanBodyBones.Spine].rigidbody.velocity = new Vector3();
                        }

                        enemyAI = null;
                    }
                }
            }
        }
    }


    public void use() {

        coll.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void endUse() {

        coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        if (enemyAI) {

            enemyAI.isForcePulled(false);
            enemyAI = null;
        }
    }


    public void pullTarget(Transform navParent, MuscleCollisionBroadcaster broadcaster) {

        var targetPosition = new Vector3(handAnchor.position.x, playArea.position.y, handAnchor.position.z) + handAnchor.forward * setPosition;
        targetPosition = new Vector3(targetPosition.x, playArea.position.y, targetPosition.z);

        // Calculate the journey length.
        var journeyLength = Vector3.Distance(targetPosition, navParent.position);
        var distanceToDash = journeyLength < speed ? journeyLength : speed;

        enemyAI.isForcePulled(true);

        broadcaster.puppetMaster.muscles[(int)HumanBodyBones.Hips].transform.LookAt(handAnchor);
        broadcaster.puppetMaster.muscles[(int)HumanBodyBones.Spine].transform.LookAt(handAnchor);

        // Set our position as a fraction of the distance between the markers.
        navParent.position += (targetPosition - navParent.position).normalized * distanceToDash;
    }
}
*/
