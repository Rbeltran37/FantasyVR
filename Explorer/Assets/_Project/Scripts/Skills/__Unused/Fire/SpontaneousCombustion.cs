using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpontaneousCombustion : MonoBehaviour
{
    public Transform centerMass;
    public GameObject colliderPrefab;
    public SpontaneousCombustionCollider spontaneousCombustionCollider;
    public float colliderRadius = 5;
    public int headDamage = 3;
    public int bodyDamage = 3;
    public float hitForce = 0;
    public float lifetime = 1;


    public void use() {

        var combustion = Instantiate(colliderPrefab, centerMass);
        combustion.transform.localPosition = new Vector3();
        combustion.SetActive(true);
        combustion.transform.SetParent(null);
    }

    public void updateCollider() {

        spontaneousCombustionCollider.headDamage = headDamage;
        spontaneousCombustionCollider.bodyDamage = bodyDamage;
        spontaneousCombustionCollider.hitForce = hitForce;
        spontaneousCombustionCollider.lifetime = lifetime;

        spontaneousCombustionCollider.transform.localScale = new Vector3(colliderRadius, colliderRadius, colliderRadius);
    }
}
