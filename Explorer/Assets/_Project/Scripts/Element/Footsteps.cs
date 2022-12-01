using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [Tooltip("Raycast will use footTransform.forward")]
    [SerializeField] private Transform footTransform;
    [SerializeField] private ElementPropertyContainer elementPropertyContainer;
    [SerializeField] private FootstepsSO footstepsSo;

    private bool _isOnFloor;
    private bool _isCoolingDown;


    private void OnEnable()
    {
        _isOnFloor = false;
        _isCoolingDown = false;
    }

    private void FixedUpdate()
    {
        if (_isCoolingDown) return;
        
        CheckForFootstep();
    }

    private void CheckForFootstep()
    {
        if (!footTransform || !footstepsSo) return;

        RaycastHit hit;
        if (Physics.Raycast(footTransform.position, footTransform.forward, out hit, footstepsSo.DistanceToFloor, footstepsSo.GroundLayers))
        {
            if (_isOnFloor) return;

            Footstep(hit);
        }
        else
        {
            _isOnFloor = false;
        }
    }

    private void Footstep(RaycastHit hit)
    {
        var hitElementProperty = hit.transform.GetComponent<ElementPropertyContainer>();
        if (hitElementProperty)
        {
            hitElementProperty.ActivateFx(elementPropertyContainer, hit.point, hit.normal);

            _isOnFloor = true;
            StartCoroutine(StartCooldown());
        }
    }

    private IEnumerator StartCooldown()
    {
        _isCoolingDown = true;
        
        yield return new WaitForSeconds(footstepsSo.CooldownTime);

        _isCoolingDown = false;
    }
}
