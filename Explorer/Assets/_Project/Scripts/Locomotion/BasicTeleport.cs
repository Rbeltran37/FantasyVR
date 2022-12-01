using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;

public class BasicTeleport : MonoBehaviour
{
    [SerializeField] private Transform _handAnchor;
    [SerializeField] private Transform _playArea;
    [SerializeField] private float _teleportRange = 10;
    [SerializeField] private float _teleportDelay = .1f;
    [SerializeField] private float _teleportAreaRadius = .5f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private PuppetMaster _puppetMaster;
    [SerializeField] private Transform _teleportTarget;
    [SerializeField] private bool _isLeftHand = false;
    [SerializeField] private float _onSensitivity = .85f;
    [SerializeField] private Transform _currentTeleportTarget;
    [SerializeField] private List<SkinnedMeshRenderer> playerModelMeshes = new List<SkinnedMeshRenderer>();
    
    
    private List<UnityEngine.XR.InputDevice> _devices = new List<UnityEngine.XR.InputDevice>();
    private bool _hasBeenCalled = false;
    private bool _isValidTeleportArea = false;

    private const float TravelTime = .5f;
    private const float TeleportHeightBuffer = 3;


    private void Start()
    {
        SetupTarget();
        UpdateTarget();
    }

    private void SetupTarget()
    {
        _currentTeleportTarget.position = _playArea.position;
        _teleportTarget.SetParent(_handAnchor);
        _teleportTarget.localPosition = new Vector3(0, 0, _teleportRange);
    }

    private void Update()
    {
        if (_devices.Count == 0)
            InitializeControllers();

        if (IsBeingCalled())
        {
            Scan();
            _hasBeenCalled = true;
        }
        else if (_hasBeenCalled)
        {
            StartTeleport();
            _hasBeenCalled = false;
        }
    }
    
    
    public void InitializeControllers() {

        if (_isLeftHand) {

            UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.LeftHanded, _devices);
        }
        else {

            UnityEngine.XR.InputDevices.GetDevicesWithRole(UnityEngine.XR.InputDeviceRole.RightHanded, _devices);
        }
    }

    //Called when thumbstick is pointed down
    private bool IsBeingCalled()
    {
        var axisValue = new Vector2();
        foreach (var device in _devices) {

            if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis,
                    out axisValue)
                && !axisValue.Equals(new Vector2())) {

                var y = axisValue.y;
                if (y > _onSensitivity)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    

    [Button]
    public void StartTeleport()
    {
        if (!_isValidTeleportArea || _currentTeleportTarget.position == _playArea.position)
            return;

        StartCoroutine(Teleport());
    }

    [Button]
    public void Scan()
    {
        RaycastHit hit;
        
        //Shoot Spherecast at Target, Searching for the ground layer, without hitting enemies
        if (Physics.SphereCast(_handAnchor.position, _teleportAreaRadius, _handAnchor.forward,
            out hit, _teleportRange, _groundLayer))
        {
            _currentTeleportTarget.position = hit.point;

            if (Physics.SphereCast(_handAnchor.position, _teleportAreaRadius, _handAnchor.forward,
                out hit, _teleportRange, _obstacleLayer))
            {
                _isValidTeleportArea = false;
            }
            else
            {
                _isValidTeleportArea = true;
            }

            UpdateTarget();
        }
        else
        {
            //Shoot Spherecast downward searching for the ground layer, without hitting enemies
            if (Physics.SphereCast(_teleportTarget.position, _teleportAreaRadius, Vector3.down,
                out hit, _teleportRange + TeleportHeightBuffer, _groundLayer))
            {
                _currentTeleportTarget.position = hit.point;

                if (Physics.SphereCast(_teleportTarget.position, _teleportAreaRadius, Vector3.down,
                    out hit, _teleportRange + TeleportHeightBuffer, _obstacleLayer))
                {
                    _isValidTeleportArea = false;
                }
                else
                {
                    _isValidTeleportArea = true;
                }

                UpdateTarget();
            }
        }
    }

    private void UpdateTarget()
    {
        //Setup valid/invalid visual
        if (_isValidTeleportArea)
        {
            _currentTeleportTarget.gameObject.SetActive(true);
        }
        else
        {
            _currentTeleportTarget.gameObject.SetActive(false);
        }
    }

    private IEnumerator ActivatePuppet()
    {
        yield return new WaitForSeconds(TravelTime);
        _puppetMaster.mode = PuppetMaster.Mode.Active;
        
    }

    private IEnumerator Teleport()
    {
        //Disable puppet colliders
        _puppetMaster.mode = PuppetMaster.Mode.Disabled;
        TogglePlayerModel(false);

        //Teleport to target position
        _playArea.position = _currentTeleportTarget.position;
        
        //Teleport puppet and enable colliders
        _puppetMaster.Teleport(_currentTeleportTarget.position, _playArea.rotation, true);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(_teleportDelay);
        
        //Reactivate after waiting
        _puppetMaster.mode = PuppetMaster.Mode.Active;
        TogglePlayerModel(true);
        
        //Reset
        _currentTeleportTarget.position = _playArea.position;
        _isValidTeleportArea = false;
        UpdateTarget();
    }

    private void TogglePlayerModel(bool state)
    {
        foreach (var mesh in playerModelMeshes)
        {
            mesh.enabled = state;
        }
    }
}
