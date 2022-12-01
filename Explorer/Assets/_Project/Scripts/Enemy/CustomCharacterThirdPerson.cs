using UnityEngine;
using System.Collections;
using RootMotion;
using RootMotion.Demos;
using RootMotion.Dynamics;

/// <summary>
/// Third person character controller. This class is based on the ThirdPersonCharacter.cs of the Unity Exmaple Assets.
/// </summary>
public class CustomCharacterThirdPerson : CustomCharacterBase {

	// Is the character always rotating to face the move direction or is he strafing?
	[System.Serializable]
	public enum MoveMode {
		Directional,
		Strafe
	}

	// Animation state
	public struct AnimState {
		public Vector3 moveDirection; // the forward speed
		public bool jump; // should the character be jumping?
		public bool crouch; // should the character be crouching?
		public bool onGround; // is the character grounded
		public bool isStrafing; // should the character always rotate to face the move direction or strafe?
		public float yVelocity; // y velocity of the character
		public bool doubleJump;
	}

	[Header("References")] 
	public BehaviourPuppet behaviourPuppet;
	
	public CustomCharacterThirdPersonData customCharacterThirdPersonData;
	public CustomCharacterAnimationBase customCharacterAnimationBase; // the animation controller
	public CustomUserControlThirdPerson customUserControlThirdPerson; // user input
	public CameraController cameraController; // Camera controller (optional). If assigned will update the camera in LateUpdate only if character moves

	
	public MoveMode moveMode; // Is the character always rotating to face the move direction or is he strafing?

	
	[Header("Custom")]
	public bool IsMovingBackward = false;
	public bool IsAttacking = false;

	public bool onGround { get; private set; }
	public AnimState animState = new AnimState();

	protected Vector3 moveDirection; // The current move direction of the character in Strafe move mode
	private Animator animator;
	private Vector3 normal, platformVelocity, platformAngularVelocity;
	private RaycastHit hit;
	private float jumpLeg, jumpEndTime, forwardMlp, groundDistance, lastAirTime, stickyForce;
	private Vector3 wallNormal = Vector3.up;
	private Vector3 moveDirectionVelocity;
	private float wallRunWeight;
	private float lastWallRunWeight;
	private Vector3 fixedDeltaPosition;
	private Quaternion fixedDeltaRotation = Quaternion.identity;
	private bool fixedFrame;
	private float wallRunEndTime;
	private Vector3 gravity;
	private Vector3 verticalVelocity;
	private float velocityY;
	private bool doubleJumped;
	private bool jumpReleased;


	// Use this for initialization
	protected override void Start () {
		base.Start();

		animator = GetComponent<Animator>();
		if (animator == null) animator = customCharacterAnimationBase.GetComponent<Animator>();

		wallNormal = -gravity.normalized;
		onGround = true;
		animState.onGround = true;

		if (cameraController != null) cameraController.enabled = false;
	}

	void OnAnimatorMove() {
		Move (animator.deltaPosition, animator.deltaRotation);
	}

	// When the Animator moves
	public override void Move(Vector3 deltaPosition, Quaternion deltaRotation) {
		// Accumulate delta position, update in FixedUpdate to maintain consitency
		if (IsMovingBackward)
		{
			deltaPosition *= -1;
		}
		fixedDeltaPosition += deltaPosition;
		fixedDeltaRotation *= deltaRotation;
	}

	void FixedUpdate()
	{
		if (IsAttacking) return;
		
        gravity = GetGravity();

		verticalVelocity = V3Tools.ExtractVertical(r.velocity, gravity, 1f);
		velocityY = verticalVelocity.magnitude;
		if (Vector3.Dot(verticalVelocity, gravity) > 0f) velocityY = -velocityY;

		/*
		if (animator != null && animator.updateMode == AnimatorUpdateMode.AnimatePhysics) {
			smoothPhysics = false;
			characterAnimation.smoothFollow = false;
		}
		*/

		// Smoothing out the fixed time step
		r.interpolation = customCharacterThirdPersonData.smoothPhysics? RigidbodyInterpolation.Interpolate: RigidbodyInterpolation.None;
		customCharacterAnimationBase.smoothFollow = customCharacterThirdPersonData.smoothPhysics;

        // Move
        if (IsMovingBackward)
        {
            fixedDeltaPosition *= -1;
        } 
        MoveFixed(fixedDeltaPosition);
		fixedDeltaPosition = Vector3.zero;

		r.MoveRotation(transform.rotation * fixedDeltaRotation);
		fixedDeltaRotation = Quaternion.identity;

		Rotate();

		GroundCheck (); // detect and stick to ground

		// Friction
		if (customUserControlThirdPerson.state.move == Vector3.zero && groundDistance < customCharacterBaseData.airborneThreshold * 0.5f) HighFriction();
		else ZeroFriction();

		bool stopSlide = onGround && customUserControlThirdPerson.state.move == Vector3.zero && r.velocity.magnitude < 0.5f && groundDistance < customCharacterBaseData.airborneThreshold * 0.5f;

		// Individual gravity
		if (gravityTarget != null) {
			r.useGravity = false;

			if (!stopSlide) r.AddForce(gravity);
		}

		if (stopSlide) {
			r.useGravity = false;
			r.velocity = Vector3.zero;
		} else if (gravityTarget == null) r.useGravity = true;

		if (onGround) {
			// Jumping
			animState.jump = Jump();
			jumpReleased = false;
			doubleJumped = false;
		} else {
			if (!customUserControlThirdPerson.state.jump) jumpReleased = true;

			//r.AddForce(gravity * gravityMultiplier);
			if (jumpReleased && customUserControlThirdPerson.state.jump && !doubleJumped && customCharacterThirdPersonData.doubleJumpEnabled) {
				jumpEndTime = Time.time + 0.1f;
				animState.doubleJump = true;

				Vector3 jumpVelocity = customUserControlThirdPerson.state.move * customCharacterThirdPersonData.airSpeed;
				r.velocity = jumpVelocity;
				r.velocity += transform.up * customCharacterThirdPersonData.jumpPower * customCharacterThirdPersonData.doubleJumpPowerMlp;
				doubleJumped = true;
			}
		}

		// Scale the capsule colllider while crouching
		ScaleCapsule(customUserControlThirdPerson.state.crouch? customCharacterThirdPersonData.crouchCapsuleScaleMlp: 1f);

		fixedFrame = true;
    }

    protected virtual void Update() {
		// Fill in animState
		animState.onGround = onGround;
		animState.moveDirection = GetMoveDirection();
		if (IsMovingBackward)
		{
			animState.moveDirection *= -1;
		}
		animState.yVelocity = Mathf.Lerp(animState.yVelocity, velocityY, Time.deltaTime * 10f);
		animState.crouch = customUserControlThirdPerson.state.crouch;
		animState.isStrafing = moveMode == MoveMode.Strafe;
	}

	protected virtual void LateUpdate() {
		if (cameraController == null) return;
		
		cameraController.UpdateInput();
		
		if (!fixedFrame && r.interpolation == RigidbodyInterpolation.None) return;
		
		// Update camera only if character moves
		cameraController.UpdateTransform(r.interpolation == RigidbodyInterpolation.None? Time.fixedDeltaTime: Time.deltaTime);
		
		fixedFrame = false;
	}

	private void MoveFixed(Vector3 deltaPosition) {
        // Process horizontal wall-running
        WallRun();

        Vector3 velocity = deltaPosition / Time.deltaTime;
		
		// Add velocity of the rigidbody the character is standing on
		velocity += V3Tools.ExtractHorizontal(platformVelocity, gravity, 1f);
		
		if (onGround) {
			// Rotate velocity to ground tangent
			if (customCharacterThirdPersonData.velocityToGroundTangentWeight > 0f) {
				Quaternion rotation = Quaternion.FromToRotation(transform.up, normal);
				velocity = Quaternion.Lerp(Quaternion.identity, rotation, customCharacterThirdPersonData.velocityToGroundTangentWeight) * velocity;
			}
		} else {
			// Air move
			//Vector3 airMove = new Vector3 (userControl.state.move.x * airSpeed, 0f, userControl.state.move.z * airSpeed);
			Vector3 airMove = V3Tools.ExtractHorizontal(customUserControlThirdPerson.state.move * customCharacterThirdPersonData.airSpeed, gravity, 1f);
			velocity = Vector3.Lerp(r.velocity, airMove, Time.deltaTime * customCharacterThirdPersonData.airControl);
		}

		if (onGround && Time.time > jumpEndTime) {
			r.velocity = r.velocity - transform.up * stickyForce * Time.deltaTime;
		}
		
		// Vertical velocity
		Vector3 verticalVelocity = V3Tools.ExtractVertical(r.velocity, gravity, 1f);
		Vector3 horizontalVelocity = V3Tools.ExtractHorizontal(velocity, gravity, 1f);

		if (onGround) {
			if (Vector3.Dot(verticalVelocity, gravity) < 0f) {
				verticalVelocity = Vector3.ClampMagnitude(verticalVelocity, customCharacterThirdPersonData.maxVerticalVelocityOnGround);
			}
		}

		r.velocity = horizontalVelocity + verticalVelocity;

        // Dampering forward speed on the slopes (Not working since Unity 2017.2)
        //float slopeDamper = !onGround? 1f: GetSlopeDamper(-deltaPosition / Time.deltaTime, normal);
        //forwardMlp = Mathf.Lerp(forwardMlp, slopeDamper, Time.deltaTime * 5f);
        forwardMlp = 1f;
	}

	// Processing horizontal wall running
	private void WallRun() {
		bool canWallRun = CanWallRun();

		// Remove flickering in and out of wall-running
		if (wallRunWeight > 0f && !canWallRun) wallRunEndTime = Time.time;
		if (Time.time < wallRunEndTime + 0.5f) canWallRun = false;

		wallRunWeight = Mathf.MoveTowards(wallRunWeight, (canWallRun? 1f: 0f), Time.deltaTime * customCharacterThirdPersonData.wallRunWeightSpeed);
		
		if (wallRunWeight <= 0f) {
			// Reset
			if (lastWallRunWeight > 0f) {
				Vector3 frw = V3Tools.ExtractHorizontal(transform.forward, gravity, 1f);
				transform.rotation = Quaternion.LookRotation(frw, -gravity);
				wallNormal = -gravity.normalized;
			}
		}

		lastWallRunWeight = wallRunWeight;
		
		if (wallRunWeight <= 0f) return;

		// Make sure the character won't fall down
		if (onGround && velocityY < 0f) r.velocity = V3Tools.ExtractHorizontal(r.velocity, gravity, 1f);
		
		// transform.forward flattened
		Vector3 f = V3Tools.ExtractHorizontal(transform.forward, gravity, 1f);

		// Raycasting to find a walkable wall
		RaycastHit velocityHit = new RaycastHit();
		velocityHit.normal = -gravity.normalized;
		Physics.Raycast(onGround? transform.position: capsule.bounds.center, f, out velocityHit, 3f, customCharacterThirdPersonData.wallRunLayers);
		
		// Finding the normal to rotate to
		wallNormal = Vector3.Lerp(wallNormal, velocityHit.normal, Time.deltaTime * customCharacterThirdPersonData.wallRunRotationSpeed);

		// Clamping wall normal to max rotation angle
		wallNormal = Vector3.RotateTowards(-gravity.normalized, wallNormal, customCharacterThirdPersonData.wallRunMaxRotationAngle * Mathf.Deg2Rad, 0f);

		// Get transform.forward ortho-normalized to the wall normal
		Vector3 fW = transform.forward;
		Vector3 nW = wallNormal;
		Vector3.OrthoNormalize(ref nW, ref fW);

		// Rotate from upright to wall normal
		transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(f, -gravity), Quaternion.LookRotation(fW, wallNormal), wallRunWeight);
	}

	// Should the character be enabled to do a wall run?
	private bool CanWallRun() {
		if (Time.time < jumpEndTime - 0.1f) return false;
		if (Time.time > jumpEndTime - 0.1f + customCharacterThirdPersonData.wallRunMaxLength) return false;
		if (velocityY < customCharacterThirdPersonData.wallRunMinVelocityY) return false;
		if (customUserControlThirdPerson.state.move.magnitude < customCharacterThirdPersonData.wallRunMinMoveMag) return false;
		return true;
	}

	// Get the move direction of the character relative to the character rotation
	private Vector3 GetMoveDirection() {
		switch(moveMode) {
		case MoveMode.Directional:
			moveDirection = Vector3.SmoothDamp(moveDirection, new Vector3(0f, 0f, customUserControlThirdPerson.state.move.magnitude), ref moveDirectionVelocity, customCharacterThirdPersonData.smoothAccelerationTime);
			moveDirection = Vector3.MoveTowards(moveDirection, new Vector3(0f, 0f, customUserControlThirdPerson.state.move.magnitude), Time.deltaTime * customCharacterThirdPersonData.linearAccelerationSpeed);
			return moveDirection * forwardMlp;
		case MoveMode.Strafe:
			moveDirection = Vector3.SmoothDamp(moveDirection, customUserControlThirdPerson.state.move, ref moveDirectionVelocity, customCharacterThirdPersonData.smoothAccelerationTime);
			moveDirection = Vector3.MoveTowards(moveDirection, customUserControlThirdPerson.state.move, Time.deltaTime * customCharacterThirdPersonData.linearAccelerationSpeed);
			return transform.InverseTransformDirection(moveDirection);
		}

		return Vector3.zero;
	}

	// Rotate the character
	protected virtual void Rotate() {
		if (gravityTarget != null) r.MoveRotation (Quaternion.FromToRotation(transform.up, transform.position - gravityTarget.position) * transform.rotation);
		if (platformAngularVelocity != Vector3.zero) r.MoveRotation (Quaternion.Euler(platformAngularVelocity) * transform.rotation);

		float angle = GetAngleFromForward(GetForwardDirection());
		
		if (customUserControlThirdPerson.state.move == Vector3.zero) angle *= (1.01f - (Mathf.Abs(angle) / 180f)) * customCharacterThirdPersonData.stationaryTurnSpeedMlp;

		// Rotating the character
		//RigidbodyRotateAround(characterAnimation.GetPivotPoint(), transform.up, angle * Time.deltaTime * turnSpeed);
		r.MoveRotation(Quaternion.AngleAxis(angle * Time.deltaTime * customCharacterThirdPersonData.turnSpeed, transform.up) * r.rotation);
	}

	// Which way to look at?
	private Vector3 GetForwardDirection() {
		bool isMoving = customUserControlThirdPerson.state.move != Vector3.zero;

		switch (moveMode) {
		case MoveMode.Directional:
			if (isMoving && !IsMovingBackward) return customUserControlThirdPerson.state.move;
			return customCharacterThirdPersonData.lookInCameraDirection? customUserControlThirdPerson.state.lookPos - r.position: transform.forward;
		case MoveMode.Strafe:
			if (isMoving) return customUserControlThirdPerson.state.lookPos - r.position;
			return customCharacterThirdPersonData.lookInCameraDirection? customUserControlThirdPerson.state.lookPos - r.position: transform.forward;
		}

		return Vector3.zero;
	}

	protected virtual bool Jump() {
		// check whether conditions are right to allow a jump:
		if (!customUserControlThirdPerson.state.jump) return false;
		if (customUserControlThirdPerson.state.crouch) return false;
		if (!customCharacterAnimationBase.animationGrounded) return false;
		if (Time.time < lastAirTime + customCharacterThirdPersonData.jumpRepeatDelayTime) return false;

		// Jump
		onGround = false;
		jumpEndTime = Time.time + 0.1f;

        Vector3 jumpVelocity = customUserControlThirdPerson.state.move * customCharacterThirdPersonData.airSpeed;
        jumpVelocity += transform.up * customCharacterThirdPersonData.jumpPower;

        if (customCharacterThirdPersonData.smoothJump)
        {
            StopAllCoroutines();
            StartCoroutine(JumpSmooth(jumpVelocity - r.velocity));
        } else
        {
            r.velocity = jumpVelocity;
        }

        return true;
	}

    // Add jump velocity smoothly to avoid puppets launching to space when unpinned during jump acceleration
    private IEnumerator JumpSmooth(Vector3 jumpVelocity)
    {
        int steps = 0;
        int stepsToTake = 3;
        while (steps < stepsToTake)
        {
            r.AddForce((jumpVelocity) / stepsToTake, ForceMode.VelocityChange);
            steps++;
            yield return new WaitForFixedUpdate();
        }
    }

	// Is the character grounded?
	private void GroundCheck () {
		Vector3 platformVelocityTarget = Vector3.zero;
		platformAngularVelocity = Vector3.zero;
		float stickyForceTarget = 0f;

		// Spherecasting
		hit = GetSpherecastHit();

		//normal = hit.normal;
		normal = transform.up;
		//groundDistance = r.position.y - hit.point.y;
		groundDistance = Vector3.Project(r.position - hit.point, transform.up).magnitude;

		// if not jumping...
		bool findGround = Time.time > jumpEndTime && velocityY < customCharacterThirdPersonData.jumpPower * 0.5f;

		if (findGround) {
			bool g = onGround;
			onGround = false;

			// The distance of considering the character grounded
			float groundHeight = !g? customCharacterBaseData.airborneThreshold * 0.5f: customCharacterBaseData.airborneThreshold;

			//Vector3 horizontalVelocity = r.velocity;
			Vector3 horizontalVelocity = V3Tools.ExtractHorizontal(r.velocity, gravity, 1f);

			float velocityF = horizontalVelocity.magnitude;

			if (groundDistance < groundHeight &&
			    behaviourPuppet.state == BehaviourPuppet.State.Puppet) {	//Added check to see puppet is pinned
				// Force the character on the ground
				stickyForceTarget = customCharacterThirdPersonData.groundStickyEffect * velocityF * groundHeight;

				// On moving platforms
				if (hit.rigidbody != null) {
					platformVelocityTarget = hit.rigidbody.GetPointVelocity(hit.point);
					platformAngularVelocity = Vector3.Project(hit.rigidbody.angularVelocity, transform.up);
				}

				// Flag the character grounded
				onGround = true;
			}
		}

		// Interpolate the additive velocity of the platform the character might be standing on
		platformVelocity = Vector3.Lerp(platformVelocity, platformVelocityTarget, Time.deltaTime * customCharacterThirdPersonData.platformFriction);

		stickyForce = stickyForceTarget;//Mathf.Lerp(stickyForce, stickyForceTarget, Time.deltaTime * 5f);

		// remember when we were last in air, for jump delay
		if (!onGround) lastAirTime = Time.time;
	}
}
