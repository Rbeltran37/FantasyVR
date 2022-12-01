using System;
using System.Collections;
using UnityEngine;
using RootMotion.Dynamics;


public class ForcePush : MonoBehaviour 
{
    [SerializeField] private SimpleAudioEvent simpleAudioEvent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ForcePushData forcePushData;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject pushFx;


    private void Awake()
    {
        if (pushFx)
            pushFx.SetActive(false);
    }

    private void OnDisable() 
    {
        if (pushFx)
            pushFx.gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {

        var otherRigidBody = other.GetComponent<Rigidbody>();
        if (otherRigidBody != null) 
        {
            var broadcaster = otherRigidBody.GetComponent<MuscleCollisionBroadcaster>();
            if (broadcaster != null) 
            {
                if (broadcaster.puppetMaster.GetMuscleIndex(HumanBodyBones.Hips) == broadcaster.muscleIndex) 
                {
                    StartCoroutine(Push(broadcaster, other));
                }
            }
            else {
                otherRigidBody.AddForce(Vector3.up * forcePushData.hover, ForceMode.Impulse);       //Hover
                otherRigidBody.AddForce(transform.forward * forcePushData.force, ForceMode.Impulse);       //Push
            }
        }
    }
    
    public void Setup(ForcePushData creatorForcePushData)
    {
        if (pushFx)
        {
            var pushFxParticles = pushFx.GetComponent<ParticleSystem>();
            if (pushFxParticles)
            {
                var shape = pushFxParticles.shape;
                shape.radius = forcePushData.radius;
            }
        }
        
        if (!GetComponent<BoxCollider>())
            gameObject.AddComponent<BoxCollider>();

        if (!boxCollider)
            boxCollider = GetComponent<BoxCollider>();

        forcePushData = creatorForcePushData;
        
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(forcePushData.radius * 2, forcePushData.radius * 2, forcePushData.range);
        boxCollider.center = new Vector3(0, 0, forcePushData.range / 2);
        boxCollider.enabled = false;
    }
    
    public void Use() 
    {
        StartCoroutine(DisableColliderCoroutine());
    }

    private IEnumerator DisableColliderCoroutine() 
    {
        boxCollider.enabled = true;
        pushFx.SetActive(false);
        pushFx.gameObject.SetActive(true);

        if (simpleAudioEvent && audioSource)
        {
            simpleAudioEvent.Play(audioSource);
        }

        yield return new WaitForFixedUpdate();

        boxCollider.enabled = false;
    }

    private IEnumerator Push(MuscleCollisionBroadcaster broadcaster, Collider other) 
    {
        var hipsIndex = (int) HumanBodyBones.Hips;
        var thisTransform = transform;
        var transformForward = thisTransform.forward;
        var transformPosition = thisTransform.position;
        var upwardForce = Vector3.up * forcePushData.hover;
        var forwardForce = transformForward * forcePushData.force;

        broadcaster.Hit(forcePushData.unpin, upwardForce, other.ClosestPoint(transformPosition));
        broadcaster.Hit(forcePushData.unpin, forwardForce, other.ClosestPoint(transformPosition));
        
        yield return new WaitForFixedUpdate();
        
        broadcaster = broadcaster.puppetMaster.muscles[hipsIndex].broadcaster;    //ADDED
        broadcaster.puppetMaster.muscles[hipsIndex].rigidbody.AddForce(upwardForce, ForceMode.Impulse);    //Hover
        broadcaster.puppetMaster.muscles[hipsIndex].rigidbody.AddForce(forwardForce, ForceMode.Impulse);    //Push
    }
}
