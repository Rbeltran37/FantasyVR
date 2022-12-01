using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

public class ForcePull : SkillInstance
{
    [SerializeField] private ForcePullData forcePullData;
    [SerializeField] private TargetAcquisition targetAcquisition;
    [SerializeField] private AudioSource handAudioSource;
    [SerializeField] private AudioSource targetAudioSource;
    [SerializeField] private GameObject startUseFx;
    [SerializeField] private GameObject holdFx;
    

    private Transform _cameraParent;
    private Transform _target;
    private Rigidbody _targetRigid;
    private float _pullTimer;
    private MuscleCollisionBroadcaster _muscleCollisionBroadcaster;
    private Vector3 _targetPullPosition;

    private void Awake()
    {
        var camera = Camera.main;
        if (camera != null)
        {
            _cameraParent = camera.transform.parent;
        }
        
        if (startUseFx)
            startUseFx.SetActive(false);
        
        if (holdFx)
            holdFx.SetActive(false);
    }
    
    private void FixedUpdate()
    {
        if (!_targetRigid || _targetPullPosition.Equals(Vector3.zero)) return;
        
        SetTargetPosition();
        SetTargetVelocity();

        if (holdFx)
            holdFx.transform.position = _target.position;

        _pullTimer -= Time.fixedDeltaTime;
        if (_pullTimer < 0)
        {
            EndGrab();
        }
    }

    private void SetTargetVelocity()
    {
        var pullVelocity = Vector3.zero;
        if (Vector3.Distance(_target.position, _targetPullPosition) > forcePullData.stabilizingDistance)
        {
            pullVelocity = (_targetPullPosition - _targetRigid.position).normalized * forcePullData.pullSpeed;
        }

        _targetRigid.velocity = pullVelocity;
        _targetRigid.angularVelocity = Vector3.zero;
    }

    private void SetTargetPosition()
    {
        var tempHeight = 1f;
        if (_cameraParent)
        {
            tempHeight = _cameraParent.position.y + forcePullData.liftHeight;
        }
        
        var transformPosition = transform.position;
        var directionVector = (_target.position - transformPosition).normalized;
        directionVector = new Vector3(directionVector.x, tempHeight, directionVector.z) * forcePullData.targetDistance;
        _targetPullPosition = new Vector3(transformPosition.x, tempHeight, transformPosition.z) + directionVector;
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

        _target = targetAcquisition.AcquireTarget(transform);
        if (_target)
            Grab();
        
        if (startUseFx)
        {
            if (_target)
            {
                startUseFx.transform.position = _target.position;
            }
            else
            {
                startUseFx.transform.localPosition = new Vector3(0, 0, targetAcquisition.GetRange());
            }
            startUseFx.SetActive(true);
        }
        
        /*if (skillAudioManager)
        {
            skillAudioManager.PlayStartUseSound(handAudioSource);
        }*/
    }

    private void Grab()
    {
        if (!_target) return;
        
        if (holdFx)
            holdFx.SetActive(true);
        
        /*if (skillAudioManager)
        {
            skillAudioManager.PlayHoldSound(targetAudioSource);
            skillAudioManager.PlayHoldSound(handAudioSource);
        }*/

        var broadcaster = _target.GetComponent<MuscleCollisionBroadcaster>();
        if (broadcaster)
        {
            StartCoroutine(PullPuppet(broadcaster, _target.GetComponent<Collider>()));
        }
        
        _pullTimer = forcePullData.maxPullTime;
    }

    private IEnumerator UnPinCoroutine()
    {
        while (_muscleCollisionBroadcaster && _target)
        {
            _muscleCollisionBroadcaster.Hit(forcePullData.unpin, Vector3.up, _target.position);
            yield return new WaitForSeconds(.1f);
        }
    }
    
    private void EndGrab()
    {
        _target = null;
        _targetRigid = null;
        _targetPullPosition = Vector3.zero;

        if (startUseFx)
            startUseFx.SetActive(false);
        
        if (holdFx)
            holdFx.SetActive(false);
        
        /*if (skillAudioManager)
            skillAudioManager.EndHoldSound(targetAudioSource);*/
    }

    private IEnumerator PullPuppet(MuscleCollisionBroadcaster broadcaster, Collider collider)
    {
        if (!broadcaster || !collider) yield break;
        
        var hipIndex = (int) HumanBodyBones.Hips;
        var thisTransform = transform;
        var transformPosition = thisTransform.position;
        var muscle = broadcaster.puppetMaster.muscles[hipIndex];
        
        _muscleCollisionBroadcaster = muscle.broadcaster;    //ADDED
        _muscleCollisionBroadcaster.Hit(forcePullData.unpin, Vector3.up * forcePullData.unpinVectorLength, 
            collider.ClosestPoint(transformPosition));

        yield return new WaitForFixedUpdate();

        _targetRigid = muscle.rigidbody;
        if (!_targetRigid) yield break;

        _target = _targetRigid.transform;
        _targetRigid.velocity = Vector3.zero;

        SetTargetPosition();
        StartCoroutine(UnPinCoroutine());
    }
}
