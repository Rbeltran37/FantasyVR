using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

public class Jump : MonoBehaviour 
{
    [SerializeField] private Transform playArea;
    [SerializeField] private Transform headset;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Rigidbody playAreaRigidbody;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private LocomotionManager locomotionManager;
    [SerializeField] private CharacterAudioManager characterAudioManager;
    
    public bool IsJumping { get; private set; }
    
    private bool _isCoolingDown;
    private bool _canEndJump;
    private int _numJumps = 1;

    public Action HasJumped;
    public Action HasLanded;

    private const float JUMP_BUFFER = .5f;

    
    private void Awake() {
        
        _numJumps = locomotionManager.jumps;
    }

    private void OnEnable()
    {
        EndJump();
    }

    private void FixedUpdate() 
    {
        if (!IsJumping) return;

        if (groundCheck.IsGrounded)
        {
            EndJump();
            return;
        }
        
        var playAreaHeight = playArea.position.y;
        var headPosition = headset.position;
        var playerModelPosition = new Vector3(headPosition.x, playAreaHeight, headPosition.z);
        modelRoot.position = playerModelPosition;
    }

    public void StartJump() 
    {
        if (_numJumps <= 0 || _isCoolingDown || !groundCheck.IsGrounded)
            return;
        
        groundCheck.UnStick();

        _canEndJump = false;
        StartCoroutine(JumpBufferCoroutine());

        playAreaRigidbody.AddForce(Vector3.up * locomotionManager.jumpForce, ForceMode.Impulse);
        IsJumping = true;
        _numJumps--;

        if (audioSource && characterAudioManager)
        {
            characterAudioManager.PlayJumpSound(audioSource);
        }

        HasJumped?.Invoke();
    }

    private void EndJump()
    {
        if (!_canEndJump) return;
        
        IsJumping = false;
        _numJumps = locomotionManager.jumps;
        
        var velocity = playAreaRigidbody.velocity;
        playAreaRigidbody.velocity = new Vector3(velocity.x, 0, velocity.z);
        
        StartCoroutine(JumpCooldownCoroutine());

        HasLanded?.Invoke();
    }

    private IEnumerator JumpCooldownCoroutine()
    {
        _isCoolingDown = true;
        yield return new WaitForSeconds(locomotionManager.jumpCooldown);
        _isCoolingDown = false;
    }

    private IEnumerator JumpBufferCoroutine()
    {
        yield return new WaitForSeconds(JUMP_BUFFER);
        _canEndJump = true;
    }
}
