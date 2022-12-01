using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class FinalFlashCollider : MonoBehaviour
{
    public Transform parent;
    public float range = 10;
    public float radius = 5;
    public int headDamage = 5;
    public int bodyDamage = 5;
    public float hitForce = 800;
    public float duration = 1;

    private float timer = 0;
    private List<Transform> enemiesHit = new List<Transform>();
    public Transform initialTransform;


    private void OnEnable() {
        
        initialTransform.position = parent.position;
        initialTransform.rotation = parent.rotation;
        initialTransform.SetParent(null);
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration) {

            resetBeam();
            return;
        }

        var currentRange = (timer / duration) * range;
        var currentRadius = (timer / duration) * radius;
        transform.localScale = new Vector3(currentRadius, currentRange, currentRadius);
        transform.localPosition = new Vector3(0, 0, currentRange + .2f);
    }


    private void OnTriggerEnter(Collider other) {

        var broadcaster = other.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster) {

            var puppet = broadcaster.puppetMaster;
            if (puppet && !enemiesHit.Contains(puppet.transform)) {

                /*var damageHandler = puppet.transform.parent.GetComponentInChildren<PuppetDamageHandler>();
                if (damageHandler) {
                    
                    damageHandler.triggerHit(broadcaster.muscleIndex, bodyDamage, headDamage, hitForce, initialTransform.forward);
                    enemiesHit.Add(puppet.transform);
                }*/
            }
        }
    }


    private void resetBeam() {

        initialTransform.SetParent(parent);
        transform.localPosition = new Vector3();
        transform.localEulerAngles = new Vector3(90, 0, 0);
        transform.localScale = new Vector3();
        enemiesHit = new List<Transform>();
        gameObject.SetActive(false);
    }
}
