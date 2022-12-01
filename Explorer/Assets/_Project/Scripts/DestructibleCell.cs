using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCell : MonoBehaviour
{
    [SerializeField] private float disableTime = 3;

    private float cellSize = 3;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Collider collider;
    private Rigidbody rigidbody;
    
    public List<DestructibleCell> upperCells;

    private void Start() {

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();

        Ray ray = new Ray(transform.position, -Vector3.up);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, cellSize)) {

            var hitRigid = raycastHit.rigidbody;

            if (hitRigid) {

                var lower = raycastHit.rigidbody.GetComponent<DestructibleCell>();
                if (lower) {

                    lower.upperCells.Add(this);
                }
            }
        }
    }

    public void toggleSupportedCells() {

        rigidbody.isKinematic = false;

        foreach (DestructibleCell cell in upperCells) {

            cell.toggleSupportedCells();
        }

        StartCoroutine(DelayedDisable());
    }

    public void resetObject() 
    {
        rigidbody.isKinematic = true;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    private IEnumerator DelayedDisable() 
    {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
    }
}
