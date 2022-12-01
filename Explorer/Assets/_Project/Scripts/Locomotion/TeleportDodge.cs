using System;
using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using Sirenix.OdinInspector;
using UnityEngine;

public class TeleportDodge : MonoBehaviour
{
    //Changed from private to public for accessibility in CopyPosition.cs
    [SerializeField] public Transform _headset;
    
    [SerializeField] private Transform _playArea;
    [SerializeField] private float _maxScanRadius = 3;
    [SerializeField] private float _teleportRange = 5;
    [SerializeField] private float _teleportHeightRange = 5;
    [SerializeField] private float _teleportDelay = .1f;
    
    
    [SerializeField] private float _teleportAreaRadius = .5f;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private PuppetMaster _puppetMaster;
    [SerializeField] private Transform _teleportTarget;
    [SerializeField] private bool _isLeftHand = false;
    [SerializeField] private float _onSensitivity = -.85f;
    
    
    
    private List<UnityEngine.XR.InputDevice> _devices = new List<UnityEngine.XR.InputDevice>();
    private Collider _currentHitTarget;
    private Vector3 _currentTeleportTargetPosition;
    private bool _hasBeenCalled = false;

    private const float TravelTime = .5f;

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
                if (y < _onSensitivity)
                {
                    return true;
                }
            }
        }
        
        return false;
    }


    private void ScanForTarget()
    {
        //OverlapSphere from player, if target is hit, set target to nearest target
        Collider[] collidersHit = Physics.OverlapSphere(_headset.position, _maxScanRadius, _enemyLayer);
        if (collidersHit.Length > 0)
        {
            var nearestDistance = _maxScanRadius;
            foreach (var colliderHit in collidersHit)
            {
                var targetDistance = Vector3.Distance(colliderHit.transform.position, _headset.position);
                if (targetDistance < nearestDistance)
                {
                    nearestDistance = targetDistance;
                    _currentHitTarget = colliderHit;
                }
            }
        }
    }

    private void ScanForTeleportArea()
    {
        if (!_currentHitTarget)
            return;
        
        //Set Teleport Target
        var headsetPosition = _headset.position;
        var headsetFloor = new Vector3(headsetPosition.x, _playArea.position.y, headsetPosition.z);
        var direction = _currentHitTarget.transform.position - headsetFloor;
        
        //Check behind, in front, and to the side of the target for a valid Teleport Area
        var behindTargetPosition = direction.normalized * _teleportRange;
        var inFrontOfTargetPosition = -behindTargetPosition;
        var toTheSideOfTargetPosition = Quaternion.Euler(0, -90, 0) * inFrontOfTargetPosition;
        RaycastHit hit;
        if (Physics.SphereCast(behindTargetPosition + Vector3.up * _teleportHeightRange, _teleportAreaRadius, Vector3.down,
            out hit, _teleportHeightRange, _groundLayer))
        {
            _currentTeleportTargetPosition = hit.point;
        }
        else if (Physics.SphereCast(inFrontOfTargetPosition + Vector3.up * _teleportHeightRange, _teleportAreaRadius,
            Vector3.down, out hit, _teleportHeightRange, _groundLayer))
        {
            _currentTeleportTargetPosition = hit.point;
        }
        else if (Physics.SphereCast(toTheSideOfTargetPosition + Vector3.up * _teleportHeightRange, _teleportAreaRadius,
            Vector3.down, out hit, _teleportHeightRange, _groundLayer))
        {
            _currentTeleportTargetPosition = hit.point;
        }
        else if (Physics.SphereCast(-toTheSideOfTargetPosition + Vector3.up * _teleportHeightRange, _teleportAreaRadius,
            Vector3.down, out hit, _teleportHeightRange, _groundLayer))
        {
            _currentTeleportTargetPosition = hit.point;
        }
    }

    [Button]
    public void StartTeleport()
    {
        if (!_currentHitTarget || _currentTeleportTargetPosition == _playArea.position)
            return;

        StartCoroutine(Teleport());
    }

    [Button]
    public void Scan()
    {
        ScanForTarget();
        ScanForTeleportArea();
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

        //Teleport to target position
        var headsetPosition = _headset.position;
        var headsetFloor = new Vector3(headsetPosition.x, _playArea.position.y, headsetPosition.z);
        _playArea.position += (_currentTeleportTargetPosition - headsetFloor);

        //Rotate playArea around headset position, to point headset towards target
        _teleportTarget.position = _headset.position;
        _teleportTarget.LookAt(_currentHitTarget.transform);
        var headsetDifferenceAngle = Vector3.SignedAngle(_headset.forward, _teleportTarget.forward, Vector3.up);
        _playArea.RotateAround(_teleportTarget.position, Vector3.up, headsetDifferenceAngle);
        
        //Teleport puppet and enable colliders
        _puppetMaster.Teleport(headsetFloor, _teleportTarget.rotation, true);
        
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(_teleportDelay);
        
        //Reactivate after waiting
        _puppetMaster.mode = PuppetMaster.Mode.Active;
        
        //Reset
        _currentTeleportTargetPosition = _playArea.position;
        _currentHitTarget = null;
    }
}
