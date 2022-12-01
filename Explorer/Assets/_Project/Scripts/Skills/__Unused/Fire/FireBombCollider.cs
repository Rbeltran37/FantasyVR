using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class FireBombCollider : MonoBehaviour
{
    public FireBombExplosion fireBombExplosion;
    public Rigidbody referenceRigid;
    public Rigidbody rigid;
    public float throwForceMultiplier = 3;
    public int bodyDamage = 3;
    public int headDamage = 3;
    public float hitForce = 0;
    public float explosionRadius = 3;
    public float lifetime = 1;


    private void OnTransformParentChanged() {

        if (!transform.parent) {

            rigid.isKinematic = false;
            rigid.useGravity = true;
            rigid.AddForce(referenceRigid.velocity * throwForceMultiplier, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision) {

        explode();
    }


    public void updateExplosion() {

        fireBombExplosion.bodyDamage = bodyDamage;
        fireBombExplosion.headDamage = headDamage;
        fireBombExplosion.hitForce = hitForce;

        var scale = (1 / transform.localScale.x) * explosionRadius;
        fireBombExplosion.transform.localScale = new Vector3(scale, scale, scale);
    }


    private void explode() {

        Destroy(gameObject, lifetime);
        if (fireBombExplosion) {

            fireBombExplosion.gameObject.SetActive(true);
            fireBombExplosion.transform.SetParent(null);
            Destroy(fireBombExplosion.gameObject, lifetime);
        }
        gameObject.SetActive(false);
    }
}
