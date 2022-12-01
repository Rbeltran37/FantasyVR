using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Photon.Pun;
using UnityEngine;

public class DestructibleShell : Destructible
{
    [SerializeField] private PhotonView thisPhotonView;
    [SerializeField] private GameObject intactShellGameObject;
    [SerializeField] private GameObject brokenShellGameObject;
    [SerializeField] private Health health;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private PhysicsObject physicsObject;
    [SerializeField] private DissolveEffect dissolveEffect;
    [SerializeField] private List<DestructibleShellChunk> destructibles;
    [SerializeField] private float collisionSpeedThreshold = 5f;

    private bool _canBreak;

    public Action<Vector3, float> WasDemolished;
    
    private const string BROKEN = "broken";
    private const string INTACT = "intact";
    private const int ENABLE_WAIT_TIME = 5;


    protected override void Awake()
    {
        base.Awake();
        
        if (dissolveEffect)
        {
            dissolveEffect.FinishedDissolving += Disable;
        }
    }

    private void OnEnable()
    {
        if (dissolveEffect) dissolveEffect.FinishedDissolving += Disable;

        CoroutineCaller.Instance.StartCoroutine(IgnoreCollisionsCoroutine());
    }

    private void OnDisable()
    {
        ResetObject();
    }

    private IEnumerator IgnoreCollisionsCoroutine()
    {
        _canBreak = false;

        yield return new WaitForSeconds(ENABLE_WAIT_TIME);

        _canBreak = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_canBreak) return;

        var otherPhysicsObject = other.transform.GetComponent<PhysicsObject>();
        if (!otherPhysicsObject) return;

        DetectCollision(other.GetContact(0).point, otherPhysicsObject);
    }
    
    public override void PopulateParameters()
    {
        if (!ThisTransform) ThisTransform = transform;
        if (!ThisRigidbody)
        {
            ThisRigidbody = ThisTransform.GetComponent<Rigidbody>();
            if (!ThisRigidbody) ThisRigidbody = gameObject.AddComponent<Rigidbody>();
        }
        
        if (!ThisCollider) ThisCollider = GetComponent<Collider>();
        if (!health) health = GetComponent<Health>();
        if (!healthBar) healthBar = GetComponent<HealthBar>();
        
        OriginalLocalPosition = ThisTransform.position;
        OriginalLocalRotation = ThisTransform.rotation;

        if (!intactShellGameObject)
        {
            var intactChildCount = transform.childCount;
            for (int i = 0; i < intactChildCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name.ToLower().Contains(INTACT)) intactShellGameObject = child.gameObject;
            }
        }
        
        if (!brokenShellGameObject)
        {
            var brokenChildCount = transform.childCount;
            for (int i = 0; i < brokenChildCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name.ToLower().Contains(BROKEN)) brokenShellGameObject = child.gameObject;
            }
        }
        
        if (brokenShellGameObject)
        {
            destructibles = new List<DestructibleShellChunk>();
            var childCount = brokenShellGameObject.transform.childCount;
            for (var index = 0; index < childCount; index++)
            {
                var child = brokenShellGameObject.transform.GetChild(index);
                var destructible = child.GetComponent<DestructibleShellChunk>();
                if (!destructible) destructible = child.gameObject.AddComponent<DestructibleShellChunk>();

                destructibles.Add(destructible);
                destructible.PopulateParameters();
            }
        }

        if (!physicsObject)
        {
            physicsObject = GetComponent<PhysicsObject>();
            if (!physicsObject) physicsObject = gameObject.AddComponent<PhysicsObject>();
        }
        
        physicsObject.PopulateParameters();
        
        if (!dissolveEffect)
        {
            dissolveEffect = ThisTransform.GetComponent<DissolveEffect>();
            if (!dissolveEffect)
            {
                dissolveEffect = gameObject.AddComponent<DissolveEffect>();
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        if (DebugLogger.IsNullError(intactShellGameObject, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(brokenShellGameObject, this, "Must be set in editor.")) return;

        intactShellGameObject.SetActive(true);
        brokenShellGameObject.SetActive(false);
    }

    public override void ResetObject()
    {
        base.ResetObject();

        if (health) health.ResetObject();
        if (healthBar) healthBar.ResetObject();
        
        foreach (var destructible in destructibles)
        {
            destructible.ResetObject();
        }
        
        if (DebugLogger.IsNullError(intactShellGameObject, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(brokenShellGameObject, this, "Must be set in editor.")) return;
        
        intactShellGameObject.SetActive(true);
        brokenShellGameObject.SetActive(false);
        ThisCollider.enabled = true;
        
        if (dissolveEffect)
        {
            dissolveEffect.ResetObject();
        }
    }

    public override void Demolish(Vector3 impactOrigin, float force)
    {
        if (IsDestroyed) return;
        
        if (health && health.isAlive) return;

        base.Demolish(impactOrigin, force);
        SendDemolish(impactOrigin, force);
        
        if (DebugLogger.IsNullError(intactShellGameObject, this, "Must be set in editor.")) return;
        if (DebugLogger.IsNullError(brokenShellGameObject, this, "Must be set in editor.")) return;
        
        intactShellGameObject.SetActive(false);
        brokenShellGameObject.SetActive(true);

        ThisCollider.enabled = false;

        foreach (var destructible in destructibles)
        {
            destructible.Demolish(impactOrigin, force);
        }
        
        if (dissolveEffect) dissolveEffect.Dissolve();
        
        WasDemolished?.Invoke(impactOrigin, force);
    }
    
    private void DetectCollision(Vector3 impactOrigin, PhysicsObject otherPhysicsObject)
    {
        var collisionSpeed = physicsObject.GetSpeed() + otherPhysicsObject.GetSpeed();
        if (collisionSpeed > collisionSpeedThreshold)
        {
            var combinedForce = otherPhysicsObject.GetForce() * physicsObject.GetForce();
            Demolish(impactOrigin, combinedForce);
        }
    }

    private void SendDemolish(Vector3 impactOrigin, float force)
    {
        if (DebugLogger.IsNullDebug(thisPhotonView, "Must be set in editor.", this)) return;
        
        thisPhotonView.RPC(nameof(RPCDemolish), RpcTarget.OthersBuffered, impactOrigin, force);
    }

    [PunRPC]
    private void RPCDemolish(Vector3 impactOrigin, float force)
    {
        Demolish(impactOrigin, force);
    }
}
