using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollider : MonoBehaviour
{
    [SerializeField] private Collider damageCollider;

    private void OnCollisionEnter(Collision collision)
    {
        if (damageCollider.enabled == false)
            return;

        damageCollider.enabled = false;
        this.enabled = false;
    }
}
