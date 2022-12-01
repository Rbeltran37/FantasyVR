using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class ForceFunctions : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private bool useDistance;
    [SerializeField] private float range = 10;
    [SerializeField] private float force = 550;
    [SerializeField] private float unpin = 150;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private QueryTriggerInteraction queryTriggerInteraction;
    
    private HashSet<PuppetMaster> _puppetsHit = new HashSet<PuppetMaster>();

    private const float CAP_MULTIPLIER = 1.5f;
    private const float LIFT_MULTIPLIER = .2f;
    private const int HIPS = 0;

    
    public void SetRange(float value)
    {
        range = value;
    }

    public void SetForce(float value)
    {
        force = value;
    }

    public void SetUnpin(float value)
    {
        unpin = value;
    }
    
    public void ForceRepulse()
    {
        var radius = range / 2;
        var collidersHit = Physics.OverlapSphere(origin.position, radius, hitLayers, queryTriggerInteraction);
        foreach (var colliderHit in collidersHit)
        {
            ApplyRepulse(colliderHit);
        }
    }

    public void ResetValues()
    {
        _puppetsHit = new HashSet<PuppetMaster>();
    }
    
    public void ApplyRepulse(Collider targetCollider)
    {
        var targetRigid = targetCollider.GetComponent<Rigidbody>();
        if (!targetRigid) return;

        var heading = origin.position - targetCollider.transform.position;
        var direction = heading.normalized;

        var broadcaster = targetRigid.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster)
        {
            var puppetHit = broadcaster.puppetMaster;
            if (_puppetsHit.Contains(puppetHit)) return;

            _puppetsHit.Add(puppetHit);

            LiftPuppet(puppetHit, broadcaster, direction);

            return;
        }

        targetRigid.AddForce(Vector3.up * force * LIFT_MULTIPLIER, ForceMode.Impulse);
        targetRigid.AddForce(-direction * force, ForceMode.Impulse);
    }
    
    private void LiftPuppet(PuppetMaster puppetMaster, MuscleCollisionBroadcaster broadcaster, Vector3 direction)
    {
        var hips = puppetMaster.muscles[HIPS].transform;
        var rigid = hips.GetComponent<Rigidbody>();
        var rigidCollider = rigid.GetComponent<Collider>();
        var originPosition = origin.position;

        var radius = range;
        var distance = Vector3.Distance(originPosition, hips.position);
        var adjustedForce = force;
        if (useDistance)
        {
            adjustedForce = (radius / distance) * force;
            if (adjustedForce > CAP_MULTIPLIER)
                adjustedForce = CAP_MULTIPLIER;
        }
        
        var upwardForce = Vector3.up * force * LIFT_MULTIPLIER;
        var directionalForce = -direction * adjustedForce;

        broadcaster.Hit(unpin, upwardForce, rigidCollider.ClosestPoint(originPosition));
        broadcaster.Hit(unpin, directionalForce, rigidCollider.ClosestPoint(originPosition));
            
        rigid.AddForce(upwardForce, ForceMode.Impulse);
        rigid.AddForce(directionalForce, ForceMode.Impulse);
    }
}
