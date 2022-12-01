using System;
using System.Collections;
using RootMotion.Dynamics;
using UnityEngine;

public class ForceGrab : SkillInstance
{
    [SerializeField] private ForceGrabData forceGrabData;
    [SerializeField] private TargetAcquisition targetAcquisition;
    [SerializeField] private AudioSource handAudio;
    [SerializeField] private AudioSource targetAudio;
    [SerializeField] private GameObject startUseFx;
    [SerializeField] private GameObject holdTargetFx;
    [SerializeField] private GameObject launchFx;
    [SerializeField] private GameObject grabHandFx;
    [SerializeField] private GameObject grabPositionFx;

    private Transform _target;
    private Rigidbody _targetRigid;
    private Vector3 _grabHandReferencePosition;
    private Transform _grabHandReferenceTransform;
    private Vector3 _initialGrabbedPosition;
    private float _grabTimer;
    private MuscleCollisionBroadcaster _muscleCollisionBroadcaster;
    private Transform _cameraParent;


    private void Awake()
    {
        if (startUseFx)
            startUseFx.SetActive(false);
        
        if (holdTargetFx)
            holdTargetFx.SetActive(false);
        
        if (launchFx)
            launchFx.SetActive(false);
        
        if (grabHandFx)
            grabHandFx.SetActive(false);
        
        if (grabPositionFx)
            grabPositionFx.SetActive(false);
        
        _grabHandReferenceTransform = new GameObject("[GrabHandReferenceTransform]").transform;
        var camera = Camera.main;
        if (camera != null)
        {
            _cameraParent = camera.transform.parent;
        }
    }

    private void FixedUpdate()
    {
        if (!_targetRigid || _initialGrabbedPosition.Equals(Vector3.zero)) return;

        var liftVelocity = (_initialGrabbedPosition - _targetRigid.position).normalized;
        if (_muscleCollisionBroadcaster)
            _muscleCollisionBroadcaster.Hit(forceGrabData.unpin, Vector3.up, _target.position);
        
        if (holdTargetFx)
        {
            holdTargetFx.transform.position = _target.transform.position;
        }

        if (LaunchCheck())
        {
            Launch();
            return;
        }
        
        _targetRigid.velocity = liftVelocity;

        _grabTimer -= Time.fixedDeltaTime;
        if (_grabTimer < 0)
        {
            EndGrab();
        }
    }

    public void StartUse()
    {
        AttemptGrab();
    }

    public void EndUse()
    {
        EndGrab();
    }

    private void AttemptGrab()
    {
        if (!targetAcquisition) return;

        if (startUseFx)
        {
            startUseFx.SetActive(true);
        }
        
        /*if (skillAudioManager)
            skillAudioManager.PlayStartUseSound(handAudio);*/
        
        _target = targetAcquisition.AcquireTarget(transform);
        if (_target)
            Grab();
    }

    private void Grab()
    {
        if (!_target) return;

        if (holdTargetFx)
        {
            holdTargetFx.transform.position = _target.transform.position;
            holdTargetFx.SetActive(true);
        }
        
        /*if (skillAudioManager)
            skillAudioManager.PlayHoldSound(handAudio);*/
        
        var broadcaster = _target.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster)
        {
            SetGrabHandReferences();
            StartCoroutine(LiftPuppet(broadcaster, _target.GetComponent<Collider>()));
        }
        _grabTimer = forceGrabData.maxGrabTime;
    }

    private void SetGrabHandReferences()
    {
        _grabHandReferenceTransform.position = transform.position;
        if (_cameraParent)
            _grabHandReferenceTransform.SetParent(_cameraParent);
        
        if (grabHandFx)
            grabHandFx.SetActive(true);
        
        if (grabPositionFx)
            grabPositionFx.SetActive(true);
    }
    
    private IEnumerator LiftPuppet(MuscleCollisionBroadcaster broadcaster, Collider other) 
    {
        if (!broadcaster || !other) yield break;
        
        var hipIndex = (int) HumanBodyBones.Hips;
        var thisTransform = transform;
        var transformPosition = thisTransform.position;
        var muscle = broadcaster.puppetMaster.muscles[hipIndex];

        _muscleCollisionBroadcaster = muscle.broadcaster;    //ADDED
        _muscleCollisionBroadcaster.Hit(forceGrabData.unpin, Vector3.up * forceGrabData.unpinVectorLength, 
            other.ClosestPoint(transformPosition));

        yield return new WaitForFixedUpdate();

        _targetRigid = muscle.rigidbody;
        if (!_targetRigid) yield break;

        _target = _targetRigid.transform;
        var tempHeight = 0f;
        if (Physics.Raycast(_target.position, Vector3.down, forceGrabData.liftHeight, forceGrabData.groundLayers))
        {
            _targetRigid.velocity = Vector3.zero;
            tempHeight = forceGrabData.liftHeight;
        }
        _initialGrabbedPosition = _target.position + Vector3.up * tempHeight;
    }

    private bool LaunchCheck()
    {
        return Vector3.Distance(transform.position, _grabHandReferenceTransform.position) > forceGrabData.handLaunchDistance;
    }
    
    private void Launch()
    {
        if (!_targetRigid) return;

        _targetRigid.AddForce((transform.position - _grabHandReferenceTransform.position).normalized * forceGrabData.launchForce, 
            ForceMode.Impulse);

        if (launchFx)
        {
            launchFx.SetActive(true);
        }

        EndGrab();
        
        /*if (skillAudioManager)
            skillAudioManager.PlayImpactSound(handAudio);*/
    }

    private void EndGrab()
    {
        _target = null;
        _targetRigid = null;
        _initialGrabbedPosition = Vector3.zero;
        _grabHandReferenceTransform.SetParent(null);
        _grabHandReferenceTransform.position = Vector3.zero;
        _muscleCollisionBroadcaster = null;

        if (startUseFx)
            startUseFx.SetActive(false);
        
        if (holdTargetFx)
            holdTargetFx.SetActive(false);
        
        if (launchFx)
            launchFx.SetActive(false);
        
        if (grabHandFx)
            grabHandFx.SetActive(false);
        
        if (grabPositionFx)
            grabPositionFx.SetActive(false);
        
        /*if (skillAudioManager)
            skillAudioManager.EndHoldSound(handAudio);*/
    }
}
