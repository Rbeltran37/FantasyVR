using System.Collections;
using System.Reflection;
using UnityEngine;
using RootMotion.Dynamics;

public class ForceRepulse : MonoBehaviour 
{
    [SerializeField] private SimpleAudioEvent simpleAudioEvent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ForceRepulseData forceRepulseData;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private GameObject repulseFx;
    

    private void Awake() 
    {
        if (repulseFx)
            repulseFx.SetActive(false);
    }

    private void OnDisable() 
    {
        if (repulseFx)
            repulseFx.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        var otherRigid = other.GetComponent<Rigidbody>();
        if (!otherRigid) return;
        
        var heading = transform.position - other.transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;

        var rangeMultiplier = (forceRepulseData.radius / distance) * forceRepulseData.force;
        if (rangeMultiplier > forceRepulseData.cap)
            rangeMultiplier = forceRepulseData.cap;
        
        var broadcaster = otherRigid.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster != null) 
        {
            if (broadcaster.puppetMaster.GetMuscleIndex(HumanBodyBones.Hips) == broadcaster.muscleIndex) 
            {
                LiftPuppet(broadcaster.puppetMaster, direction, rangeMultiplier);
            }
        }
        else 
        {
            otherRigid.AddForce(Vector3.up * forceRepulseData.hover, ForceMode.Impulse);
            otherRigid.AddForce(-direction * rangeMultiplier, ForceMode.Impulse);
        }
    }
    
    public void Setup(ForceRepulseData creatorForceRepulseData)
    {
        if (repulseFx)
        {
            var pushFxParticles = repulseFx.GetComponent<ParticleSystem>();
            if (pushFxParticles)
            {
                var shape = pushFxParticles.shape;
                //shape.radius = forceRepulseData.radius;
            }
        }
        
        if (!GetComponent<SphereCollider>())
            gameObject.AddComponent<SphereCollider>();

        if (!sphereCollider)
            sphereCollider = GetComponent<SphereCollider>();

        forceRepulseData = creatorForceRepulseData;
        
        sphereCollider.isTrigger = true;
        sphereCollider.radius = forceRepulseData.radius;
        sphereCollider.enabled = false;
    }
    
    public void Use() 
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, "", this);

        StartCoroutine(DisableColliderCoroutine());
    }

    private IEnumerator DisableColliderCoroutine() 
    {
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, "", this);

        sphereCollider.enabled = true;
        repulseFx.SetActive(false);
        repulseFx.gameObject.SetActive(true);

        if (simpleAudioEvent && audioSource)
        {
            simpleAudioEvent.Play(audioSource);
        }

        yield return null;

        sphereCollider.enabled = false;
    }

    private void LiftPuppet(PuppetMaster puppet, Vector3 direction, float rangeMultiplier) 
    {
        StartCoroutine(Push(puppet.transform.GetChild(0).transform, direction, rangeMultiplier));
    }

    private IEnumerator Push(Transform bodyPart, Vector3 direction, float rangeMultiplier) 
    {
        if (!bodyPart) yield break;
        
        var broadcaster = bodyPart.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster) 
        {
            var rigid = bodyPart.GetComponent<Rigidbody>();
            var rigidCollider = rigid.GetComponent<Collider>();
            var upwardForce = Vector3.up * forceRepulseData.hover;
            var directionalForce = -direction * rangeMultiplier;
            var transformPosition = transform.position;

            broadcaster.Hit(forceRepulseData.unpin, upwardForce, rigidCollider.ClosestPoint(transformPosition));
            broadcaster.Hit(forceRepulseData.unpin, directionalForce, rigidCollider.ClosestPoint(transformPosition));
            
            yield return new WaitForFixedUpdate();
            
            rigid.AddForce(upwardForce, ForceMode.Impulse);
            rigid.AddForce(directionalForce, ForceMode.Impulse);
        }

        var childCount = bodyPart.childCount;
        for (int i = 0; i < childCount; i++) 
        {
            Push(bodyPart.GetChild(i), direction, rangeMultiplier);
        }
    }
}
