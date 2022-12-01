using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharedData/CustomCharacterThirdPersonData", order = 1)]
public class CustomCharacterThirdPersonData : ScriptableObject
{
    [Header("Movement")]
    public bool smoothPhysics = true; // If true, will use interpolation to smooth out the fixed time step.
	public float smoothAccelerationTime = 0.2f; // The smooth acceleration of the speed of the character (using Vector3.SmoothDamp)
	public float linearAccelerationSpeed = 3f; // The linear acceleration of the speed of the character (using Vector3.MoveTowards)
	public float platformFriction = 7f;					// the acceleration of adapting the velocities of moving platforms
	public float groundStickyEffect = 4f;				// power of 'stick to ground' effect - prevents bumping down slopes.
	public float maxVerticalVelocityOnGround = 3f;		// the maximum y velocity while the character is grounded
	public float velocityToGroundTangentWeight = 0f;	// the weight of rotating character velocity vector to the ground tangent

	[Header("Rotation")]
	public bool lookInCameraDirection; // should the character be looking in the same direction that the camera is facing
	public float turnSpeed = 5f;					// additional turn speed added when the player is moving (added to animation root rotation)
	public float stationaryTurnSpeedMlp = 1f;           // additional turn speed added when the player is stationary (added to animation root rotation)

    [Header("Jumping and Falling")]
    public bool smoothJump = true; // If true, adds jump force over a few fixed time steps, not in a single step
	public float airSpeed = 6f; // determines the max speed of the character while airborne
	public float airControl = 2f; // determines the response speed of controlling the character while airborne
	public float jumpPower = 12f; // determines the jump force applied when jumping (and therefore the jump height)
	public float jumpRepeatDelayTime = 0f;			// amount of time that must elapse between landing and being able to jump again
	public bool doubleJumpEnabled;
	public float doubleJumpPowerMlp = 1f;

	[Header("Wall Running")]

	public LayerMask wallRunLayers; // walkable vertical surfaces
	public float wallRunMaxLength = 1f;					// max duration of a wallrun
	public float wallRunMinMoveMag = 0.6f;				// the minumum magnitude of the user control input move vector
	public float wallRunMinVelocityY = -1f;				// the minimum vertical velocity of doing a wall run
	public float wallRunRotationSpeed = 1.5f;			// the speed of rotating the character to the wall normal
	public float wallRunMaxRotationAngle = 70f;			// max angle of character rotation
	public float wallRunWeightSpeed = 5f;				// the speed of blending in/out the wall running effect

	[Header("Crouching")]
	public float crouchCapsuleScaleMlp = 0.6f;			// the capsule collider scale multiplier while crouching
	
}