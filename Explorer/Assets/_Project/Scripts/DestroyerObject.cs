using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerObject : MonoBehaviour
{
    [SerializeField] private float explosionForce;
    [SerializeField] private Health health;

    private void OnCollisionEnter(Collision other)
    {
        if (health && !health.isAlive)
            return;

        var destructibleCell = other.transform.GetComponent<DestructibleCell>();
        if (destructibleCell)
        {
            destructibleCell.toggleSupportedCells();
            other.collider.attachedRigidbody.AddForce(transform.forward * explosionForce, ForceMode.Impulse);
        }
    }
}

