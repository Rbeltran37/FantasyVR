using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using RootMotion.Demos;

public class CombatDash : MonoBehaviour
{
    private const string COMBAT_DASH_TARGET_POSITION = "[CombatDashTargetPosition]";
    [SerializeField] private LocomotionManager locomotionManager;
    
    //Changed from private to public for accessibility in CopyPosition.cs
    [SerializeField] public Transform playArea;
    [SerializeField] public Transform headset;
    
    [SerializeField] private Transform playerController;
    [SerializeField] private GameObject aimFX;
    [SerializeField] private GameObject dashFX;
    [SerializeField] private GameObject impactFX;
    [SerializeField] private SimpleAudioEvent simpleAudioEvent;
    [SerializeField] private AudioSource audioSource;
    public GameObject[] otherLocomotions;

    private Transform _targetPosition;
    private Transform _currentTarget;
    private SimpleAI _simpleAi;
    private bool _isAiming = false;
    private bool _isDashing = false;
    private float _stunTimer;


    // Start is called before the first frame update
    void Start()
    {
        _targetPosition = new GameObject(COMBAT_DASH_TARGET_POSITION).transform;
        _stunTimer = locomotionManager.dashAgentStunTime;

        if (!playerController)
        {
            DebugLogger.Error(nameof(Start), $"{nameof(playerController)} is null.", this);
            return;
        }

        if (!dashFX)
        {
            DebugLogger.Error(nameof(Start), $"{nameof(dashFX)} is null.", this);
            return;
        }

        dashFX.transform.SetParent(playerController);
        dashFX.transform.localPosition = new Vector3();
        
        if (!impactFX)
        {
            DebugLogger.Error(nameof(Start), $"{nameof(impactFX)} is null.", this);
            return;
        }

        //impactFX.transform.SetParent(null);
        impactFX.transform.localPosition = new Vector3();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isAiming)
            SetTargetPosition();
        else if (_isDashing)
            MoveTowardsTarget();
        else {

            if (_simpleAi && !_currentTarget) {
                _stunTimer -= Time.deltaTime;
                if (_stunTimer < 0) {

                    _simpleAi = null;
                }
            }
                
            ToggleLocomotions(true);
            _targetPosition.localPosition = playArea.position;
        }
    }

    public void Aim() {

        if ((_simpleAi && !_currentTarget) || (_currentTarget && _isDashing))
            return;

        _isAiming = true;
        _isDashing = false;
    }
    public void Dash() {

        _isAiming = false;

        if (!_currentTarget)
            return;

        _isDashing = true;

        if (audioSource && simpleAudioEvent)
            simpleAudioEvent.Play(audioSource);

        if (dashFX)
        {
            var position = dashFX.transform.position;
            position = new Vector3(position.x, headset.position.y, position.z);
            dashFX.transform.position = position;
            dashFX.transform.LookAt(_currentTarget.position + Vector3.up);
            dashFX.SetActive(false);
            dashFX.SetActive(true);
        }
    }

    private void ToggleLocomotions(bool state) {

        foreach (var locomotionGameObject in otherLocomotions)
            locomotionGameObject.SetActive(state);
    }

    private void SetTargetPosition() {

        ToggleLocomotions(false);
        ScanForTarget();
    } 

    private void ScanForTarget()
    {
        _targetPosition.parent = null;
        _targetPosition.position = headset.position;
        _targetPosition.position += headset.forward * locomotionManager.dashDistance;

        _currentTarget = null;
        _simpleAi = null;

        RaycastHit hit;
        if (Physics.SphereCast(new Vector3(headset.position.x, headset.position.y, headset.position.z), 
            locomotionManager.dashSphereRadius, _targetPosition.position - headset.position, out hit, 
            locomotionManager.dashDistance, locomotionManager.dashTargetLayer)) {

            if (hit.distance < locomotionManager.dashMinDistance)
            {
                aimFX.SetActive(false);
                return;
            }

            _targetPosition.position = new Vector3(hit.point.x, hit.transform.position.y, hit.point.z) + 
                                       (headset.position - _targetPosition.position).normalized * 
                                       locomotionManager.dashSetDistance;
            _targetPosition.parent = hit.transform;
            _currentTarget = hit.transform;
            _simpleAi = _currentTarget.GetComponent<SimpleAI>();

            aimFX.transform.position = _currentTarget.position;
            aimFX.SetActive(true);
        }
        else {

            aimFX.SetActive(false);
        }
    }

    public void MoveTowardsTarget() {

        var headsetFloorPosition = new Vector3(headset.position.x, playArea.position.y, headset.position.z);

        // Calculate the journey length.
        var journeyLength = Vector3.Distance(headsetFloorPosition, _targetPosition.position);
        var distanceToDash = journeyLength < locomotionManager.dashSpeed ? journeyLength : locomotionManager.dashSpeed;

        if (locomotionManager.dashStunOnStart && _simpleAi)
        {
            _simpleAi.Stun(locomotionManager.dashAgentStunTime);
        }

        if (journeyLength < locomotionManager.dashSetDistance) {

            _isDashing = false;

            if (!locomotionManager.dashStunOnStart && _simpleAi) {
                _simpleAi.Stun(locomotionManager.dashAgentStunTime);
            }

            if (impactFX)
            {
                var currentTargetPosition = _currentTarget.transform.position;
                currentTargetPosition = new Vector3(currentTargetPosition.x, headset.position.y, currentTargetPosition.z);
                impactFX.transform.position = currentTargetPosition;
                impactFX.transform.LookAt(_currentTarget.position + Vector3.up);
                impactFX.SetActive(false);
                impactFX.SetActive(true);
            }

            _currentTarget = null;
            _simpleAi = null;
            _targetPosition.parent = null;

            aimFX.SetActive(false);
        }

        // Set our position as a fraction of the distance between the markers.
        playArea.position += (_targetPosition.position - headsetFloorPosition).normalized * distanceToDash;
    }
}
