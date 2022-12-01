using System;
using UnityEngine;

/// <summary>
/// Contols animation for a third person person controller.
/// </summary>
[RequireComponent(typeof(Animator))]
public class CustomCharacterAnimationThirdPerson: CustomCharacterAnimationBase {
	
	public CustomCharacterAnimationThirdPersonSO customCharacterAnimationThirdPersonSo;
	public CustomCharacterThirdPerson customCharacterThirdPerson;
	[Range(0.1f,3f)] [SerializeField] public float animSpeedMultiplier = 1; // How much the animation of the character will be multiplied by


	public Animator animator;
	private Vector3 lastForward;
	private const string groundedDirectional = "Grounded Directional", groundedStrafe = "Grounded Strafe";
	private float deltaAngle;

	public float defaultAnimatorSpeed = 1;
	
	protected override void Start() {
		base.Start();

		animator = GetComponent<Animator>();

		lastForward = transform.forward;

		SetDefaultAnimatorSpeed();
	}

	private void OnEnable()
	{
		SetDefaultAnimatorSpeed();
		ResetAnimatorSpeed();
	}

	private void SetDefaultAnimatorSpeed()
	{
		defaultAnimatorSpeed = customCharacterAnimationThirdPersonSo.animSpeedMultiplier;
	}

	public void ResetAnimatorSpeed()
	{
		animSpeedMultiplier = defaultAnimatorSpeed;
	}
	
	public override Vector3 GetPivotPoint() {
		return animator.pivotPosition;
	}
	
	// Is the Animator playing the grounded animations?
	public override bool animationGrounded {
		get {
			return animator.GetCurrentAnimatorStateInfo(0).IsName(groundedDirectional) || animator.GetCurrentAnimatorStateInfo(0).IsName(groundedStrafe);
		}
	}

	// Update the Animator with the current state of the character controller
	protected virtual void Update() {
		
		if (Time.deltaTime == 0f) return;

		animatePhysics = animator.updateMode == AnimatorUpdateMode.AnimatePhysics;

		// Jumping
		if (customCharacterThirdPerson.animState.jump) {
			float runCycle = Mathf.Repeat (animator.GetCurrentAnimatorStateInfo (0).normalizedTime + customCharacterAnimationThirdPersonSo.runCycleLegOffset, 1);
			float jumpLeg = (runCycle < 0 ? 1 : -1) * customCharacterThirdPerson.animState.moveDirection.z;
			
			animator.SetFloat ("JumpLeg", jumpLeg);
		}
		
		// Calculate the angular delta in character rotation
		float angle = -GetAngleFromForward(lastForward) - deltaAngle;
		deltaAngle = 0f;
		lastForward = transform.forward;
		angle *= customCharacterAnimationThirdPersonSo.turnSensitivity * 0.01f;
		angle = Mathf.Clamp(angle / Time.deltaTime, -1f, 1f);
		
		// Update Animator params
		animator.SetFloat("Turn", Mathf.Lerp(animator.GetFloat("Turn"), angle, Time.deltaTime * customCharacterAnimationThirdPersonSo.turnSpeed));
		animator.SetFloat("Forward", customCharacterThirdPerson.animState.moveDirection.z);
		animator.SetFloat("Right", customCharacterThirdPerson.animState.moveDirection.x);
		animator.SetBool("Crouch", customCharacterThirdPerson.animState.crouch);
		animator.SetBool("OnGround", customCharacterThirdPerson.animState.onGround);
		animator.SetBool("IsStrafing", customCharacterThirdPerson.animState.isStrafing);
		
		if (!customCharacterThirdPerson.animState.onGround) {
			animator.SetFloat ("Jump", customCharacterThirdPerson.animState.yVelocity);
		}

		if (customCharacterThirdPerson.customCharacterThirdPersonData.doubleJumpEnabled) animator.SetBool("DoubleJump", customCharacterThirdPerson.animState.doubleJump);
		customCharacterThirdPerson.animState.doubleJump = false;
		
		// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector
		if (customCharacterThirdPerson.animState.onGround && customCharacterThirdPerson.animState.moveDirection.z > 0f) {
			animator.speed = animSpeedMultiplier;
		} else {
			// but we don't want to use that while airborne
			animator.speed = 1;
		}
	}

	// Call OnAnimatorMove manually on the character controller because it doesn't have the Animator component
	void OnAnimatorMove() {
		// For not using root rotation in Turn value calculation 
		Vector3 f = animator.deltaRotation * Vector3.forward;
		deltaAngle += Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;

        customCharacterThirdPerson.Move(animator.deltaPosition, animator.deltaRotation);
	}
}
