using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private Transform playArea;
    [SerializeField] private Rigidbody playAreaRigidbody;
    [SerializeField] private LocomotionManager locomotionManager;

    private bool _shouldStickToGround;
    
    public bool IsGrounded { get; private set; }

    private const float UNSTICK_BUFFER = .5f;


    private void FixedUpdate()
    {
        IsGrounded = CheckIfGrounded();

        if (!IsGrounded && CheckIfFalling()) _shouldStickToGround = true;
        
        if (IsGrounded && _shouldStickToGround) StickToGround();
    }

    public void UnStick()
    {
        StartCoroutine(UnStickCoroutine());
    }

    private IEnumerator UnStickCoroutine()
    {
        _shouldStickToGround = false;
        yield return new WaitForSeconds(UNSTICK_BUFFER);
        _shouldStickToGround = true;
    }
    
    private bool CheckIfGrounded()
    {
        return Physics.Raycast(playArea.position + playArea.up * locomotionManager.groundOffset,
            Vector3.down, locomotionManager.groundedRaycastDistance, locomotionManager.groundLayers);
    }

    private void StickToGround()
    {
        var playAreaRigidbodyVelocity = playAreaRigidbody.velocity;
        playAreaRigidbody.velocity = new Vector3(playAreaRigidbodyVelocity.x, 0, playAreaRigidbodyVelocity.z);

        _shouldStickToGround = false;
    }

    private bool CheckIfFalling()
    {
        return Physics.Raycast(playArea.position + playArea.up * locomotionManager.groundOffset,
            Vector3.down, locomotionManager.fallingRaycastDistance, locomotionManager.groundLayers);
    }
}
