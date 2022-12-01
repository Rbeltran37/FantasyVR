using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mortar : MonoBehaviour
{
    public bool isFiring = false;
    [SerializeField] private Transform mortarBase;
    [SerializeField] private Transform mortarBucket;
    [SerializeField] private Transform launchPoint;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private PUNPlayerTargetManager playerTargetManager;
    [SerializeField] private float minTimeBetweenSpawn;
    [SerializeField] private float maxTimeBetweenSpawn;
    [SerializeField] private Transform indicator;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private float timeUntilHit;
    [SerializeField] private AudioSource mortarFireAudioSource;
    [SerializeField] private AudioSource mortarRotationAudioSource;
    [SerializeField] private LayerMask layersForCollisionCheck;
    [SerializeField] private GameObject VFX;
    [SerializeField] private float bucketRotationSpeedModifier;
    private Vector3 _updatedVelocity;
    private MortarProjectile _projectile;
    private bool _hasPositioned = false;
    private Transform _target;
    private PooledObject _currBall;
    private Vector3 _playerPosition;
    private List<PlayerTarget> _playerList = new List<PlayerTarget>();
    private Rigidbody _rigidbody;
    private Renderer _indicatorRenderer;
    private float _timeBetweenSpawn;
    private const int RESOLUTION_CHECKS = 30;
    private readonly List<GameObject> _obstacles = new List<GameObject>();
    private const float INDICATOR_OFFSET = 0.1f;
    private const string COLOR_VARIABLE_NAME = "_BaseColor";
    private const float OBSTACLE_CHECK_DISTANCE = 5f;
    private static readonly int _BaseColor = Shader.PropertyToID(COLOR_VARIABLE_NAME);

    private void Awake()
    {
        objectPool.InitializePool();
    }

    private void Start()
    {
        _indicatorRenderer = indicator.GetComponent<Renderer>();
        StartCoroutine(IsActivated());
    }

    [Button]
    public void StartMortarLaunch()
    {
        StartCoroutine(IsActivated());
    }
    
    [Button]
    public void StopMortarLaunch()
    {
        isFiring = false;
    }
    
    private IEnumerator IsActivated()
    {
        yield return new WaitForSeconds(3f);

        while (isFiring)
        {
            _timeBetweenSpawn = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn);
            yield return new WaitForSeconds(_timeBetweenSpawn);
            
            SetRandomTarget();
            GetSpawnedBall();
            yield return StartCoroutine(RotateBase());
            yield return StartCoroutine(RotateBucket());
            ShootBall();
        }
    }

    private IEnumerator RotateBase()
    {
        var hasRotated = false;
        mortarRotationAudioSource.Play();
        
        while (!hasRotated)
        {
            var targetDirection = _playerPosition - mortarBase.position;
            var singleStep = rotationSpeed * Time.deltaTime;
            var updatedTargetDir = new Vector3(targetDirection.x, 0, targetDirection.z);
            var newDirection = Vector3.RotateTowards(mortarBase.forward, updatedTargetDir, singleStep, 0.0f);
            mortarBase.rotation = Quaternion.LookRotation(newDirection);
            if (Vector3.Angle(mortarBase.forward, updatedTargetDir) < 1)
                hasRotated = true;
            
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator RotateBucket()
    {
        var hasRotated = false;
        var distance = Vector3.Distance(_playerPosition, mortarBucket.position);
        var velocity = PhysicsUtility.HitTargetAtTime(_currBall.transform.position, _playerPosition, new Vector3(0f, Physics.gravity.y, 0f), timeUntilHit);
        var angle = GetAngle(distance, velocity.magnitude);
        var updatedTargetDir = new Vector3(angle, 0, 0);
        var singleStep = (rotationSpeed*bucketRotationSpeedModifier) * Time.deltaTime;
        
        while (!hasRotated)
        {
            var difference = updatedTargetDir.x - mortarBucket.localEulerAngles.x;
            //DebugLogger.Info("Difference: " + difference);

            if (difference > Mathf.Epsilon)
            {
                mortarBucket.Rotate((updatedTargetDir) * singleStep);
                if (difference <= .1f)
                {
                    hasRotated = true;
                    _hasPositioned = true;
                    break;
                }
            }
            
            if (difference < Mathf.Epsilon)
            {
                mortarBucket.Rotate((-updatedTargetDir) * singleStep);
                if (difference >= -.1f)
                {
                    hasRotated = true;
                    _hasPositioned = true;
                    break;
                }
            }
            
            yield return new WaitForFixedUpdate();
        }
        
        mortarRotationAudioSource.Stop();
        _hasPositioned = true;
    }

    private void EnableIndicator()
    {
        indicator.gameObject.SetActive(true);
    }

    private void SetRandomTarget()
    {
        _playerList = playerTargetManager.GetPlayerTargets();
        var randomIndex = Random.Range (0, _playerList.Count);
        _playerPosition = _playerList[randomIndex].GetPlayerController().transform.position;
    }
    
    private void ShootBall()
    {
        if (_currBall)
        {
            _projectile = _currBall.GetComponent<MortarProjectile>();
            if (!_projectile) return;
        }
        
        _currBall.transform.position = launchPoint.position;
        _projectile.SetIndicator(indicator);
        _projectile.SetTimeUntilHit(timeUntilHit);
        _projectile.SetTimeOfLaunch(Time.deltaTime);
        _projectile.Spawn();
        _updatedVelocity = PhysicsUtility.HitTargetAtTime(_currBall.transform.position, _playerPosition, new Vector3(0f, Physics.gravity.y, 0f), timeUntilHit);
        _rigidbody = _projectile.GetRigidbody();
        if(_rigidbody) _rigidbody.velocity = _updatedVelocity;
        VFX.SetActive(true);

        CheckForObstacles();
        EnableIndicator();
        mortarFireAudioSource.Play();
        _hasPositioned = false;
    }

    void CheckForObstacles()
    {
        _obstacles.Clear();
        Vector3 previousDrawPoint = launchPoint.position;
        var height = new Vector3(_playerPosition.x, _playerPosition.y + INDICATOR_OFFSET, _playerPosition.z);

        int resolution = RESOLUTION_CHECKS;
        for (int i = 1; i <= resolution; i++) {
            float simulationTime = i / (float)resolution * timeUntilHit;
            Vector3 displacement = _updatedVelocity * simulationTime + Vector3.up * (Physics.gravity.y * simulationTime * simulationTime) / 2f;
            Vector3 drawPoint = launchPoint.position + displacement;
            
            //only checking on the way down from the parabola. Attempt to optimize obstacle search
            if (i >= (resolution / 2))
            {
                var fromPosition = previousDrawPoint;
                var toPosition = drawPoint;
                var direction = toPosition - fromPosition;

                RaycastHit hit;
                if (Physics.Raycast(fromPosition, direction, out hit, OBSTACLE_CHECK_DISTANCE, layersForCollisionCheck))
                {
                    _obstacles.Add(hit.transform.gameObject);  
                }
            }
            previousDrawPoint = drawPoint;
        }
        
        indicator.position = height;
        _indicatorRenderer.material.SetColor(_BaseColor, _obstacles.Count == 1 ? Color.white : Color.black);
    }

    private Vector3 GetSpawnedBall()
    {
        _currBall = objectPool.GetPooledObject();
        return _currBall.transform.position;
    }
    
    float GetAngle(float distance, float speed){
        float gravity = 9.81f;
        float angle = 0.5f*(Mathf.Asin ((gravity * distance) / (speed*speed)));
        return angle*Mathf.Rad2Deg;
    }
}