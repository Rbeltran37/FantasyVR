using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Slingshot : MonoBehaviour, IGrabbableOld
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject slingGameObject;
    [SerializeField] private Vector3 ballPositionAdjuster;
    [SerializeField] private float velocity;
    [SerializeField] private float spawnTimeDelay;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider collider;
    [SerializeField] private LineRenderer currBallLineRenderer;
    [SerializeField] private Transform bandParent;
    [SerializeField] private bool isWaiting = false;


    private Laser _laserArc;
    [SerializeField] private GameObject currBall;
    private bool _ready = true;
    private bool _isGrabbed;
    [SerializeField] private Vector3 slingShotStartPosition;
    private Quaternion slingShotStartRotation;

    // Start is called before the first frame update
    void Start()
    {
        slingShotStartPosition = slingGameObject.transform.position;
        slingShotStartRotation = slingGameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        ProjectileHandler();
    }

    public void AttemptGrab(Transform grabParent)
    {
        if (grabParent == null)
        {
            DebugLogger.Warning("AttemptGrab", "Attempting to grab null grabParent.", this);
            return;
        }
        
        Grab(grabParent);
    }

    public void Grab(Transform grabParent)
    {
        if (grabParent == null)
        {
            DebugLogger.Warning("Grab", "Attempting to grab null grabParent.", this);
            return;
        }

        //wait for object to spawn, then allow them to grab.
        if (isWaiting)
            return;
        
        transform.SetParent(grabParent);
        _isGrabbed = true;

        EnableLaserArc();
    }

    public void AttemptUnGrab()
    {
        if (!IsGrabbed())
        {
            return;
        }
        
        UnGrab();
    }

    public void UnGrab()
    {
        transform.SetParent(null);
        _isGrabbed = false;
        
        //Unparent ball so we can launch
        currBall.transform.parent = null;
        
        SetSlingGameObjectPosition();
        LaunchProjectile();
        StartCoroutine(WaitToSpawnBall());
    }

    public bool IsGrabbed()
    {
        return _isGrabbed;
    }

    public bool IsGrabbedBy(Transform grabParent)
    {
        return transform.parent == grabParent;
    }

    private IEnumerator WaitToSpawnBall()
    {
        isWaiting = true;
        yield return new WaitForSeconds(spawnTimeDelay);
        isWaiting = false;
        _ready = true;
    }

    private void SetSlingGameObjectPosition()
    {
        slingGameObject.transform.position = slingShotStartPosition;
        slingGameObject.transform.rotation = slingShotStartRotation;
    }

    private void ProjectileHandler()
    {
        if (_ready)
        {
            currBall = Instantiate(ballPrefab);
            currBall.transform.parent = slingGameObject.transform;
            currBall.transform.localPosition = Vector3.zero + ballPositionAdjuster;
            GetAndSetProjectileComponents(currBall);
            _ready = false;
        }
        currBall.transform.LookAt(slingShotStartPosition);
    }

    private void GetAndSetProjectileComponents(GameObject projectile)
    {
        _laserArc = projectile.GetComponent<Laser>();
        currBallLineRenderer = projectile.GetComponent<LineRenderer>();
        rb = projectile.GetComponent<Rigidbody>();
        collider = projectile.GetComponent<Collider>();
        DisableLaserArc();
    }

    private void EnableLaserArc()
    {
        if (_laserArc)
            _laserArc.enabled = true;
        
        if(currBallLineRenderer)
            currBallLineRenderer.enabled = true;
    }

    private void DisableLaserArc()
    {
        if(_laserArc)
            _laserArc.enabled = false;
        
        if(currBallLineRenderer)
            currBallLineRenderer.enabled = false;
    }

    [Button]
    private void LaunchProjectile()
    {
        DisableLaserArc();
        Vector3 ballPosition = currBall.transform.position;
        collider.isTrigger = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = (slingShotStartPosition - ballPosition) * velocity;
        _laserArc.myVelocity = rb.velocity.magnitude;
    }
    
}
