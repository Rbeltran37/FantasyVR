using UnityEngine;
using System.Collections;

/// <summary>
/// The base abstract class for all character controllers, provides common functionality.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public abstract class CustomCharacterBase: MonoBehaviour {

	[Header("Base Parameters")] 
	public CustomCharacterBaseData customCharacterBaseData;

	[Tooltip("If specified, will use the direction from the character to this Transform as the gravity vector instead of Physics.gravity. Physics.gravity.magnitude will be used as the magnitude of the gravity vector.")]
    public Transform gravityTarget;

    private PhysicMaterial zeroFrictionMaterial;
	private PhysicMaterial highFrictionMaterial;
	protected Rigidbody r;
	protected const float half = 0.5f;
	protected float originalHeight;
	protected Vector3 originalCenter;
	protected CapsuleCollider capsule;

	public abstract void Move(Vector3 deltaPosition, Quaternion deltaRotation);

	protected Vector3 GetGravity() {
		if (gravityTarget != null) {
			return (gravityTarget.position - transform.position).normalized * Physics.gravity.magnitude;
		}

		return Physics.gravity;
	}

	protected virtual void Start() {
		capsule = GetComponent<Collider>() as CapsuleCollider;
		r = GetComponent<Rigidbody>();

		// Store the collider volume
		originalHeight = capsule.height;
		originalCenter = capsule.center;

		// Physics materials
		zeroFrictionMaterial = new PhysicMaterial();
		zeroFrictionMaterial.dynamicFriction = 0f;
		zeroFrictionMaterial.staticFriction = 0f;
		zeroFrictionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
		zeroFrictionMaterial.bounciness = 0f;
		zeroFrictionMaterial.bounceCombine = PhysicMaterialCombine.Minimum;

		highFrictionMaterial = new PhysicMaterial();

		// Making sure rigidbody rotation is fixed
		r.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}

	// Spherecast from the root to find ground height
	protected virtual RaycastHit GetSpherecastHit() {
		Vector3 up = transform.up;
		Ray ray = new Ray (r.position + up * customCharacterBaseData.airborneThreshold, -up);
		RaycastHit h = new RaycastHit();
		h.point = transform.position - transform.transform.up * customCharacterBaseData.airborneThreshold;
		h.normal = transform.up;

		Physics.SphereCast(ray, customCharacterBaseData.spherecastRadius, out h, customCharacterBaseData.airborneThreshold * 2f, customCharacterBaseData.groundLayers);
		return h;
	}

	// Gets angle around y axis from a world space direction
	public float GetAngleFromForward(Vector3 worldDirection) {
		Vector3 local = transform.InverseTransformDirection(worldDirection);
		return Mathf.Atan2 (local.x, local.z) * Mathf.Rad2Deg;
	}

	// Rotate a rigidbody around a point and axis by angle
	protected void RigidbodyRotateAround(Vector3 point, Vector3 axis, float angle) {
		Quaternion rotation = Quaternion.AngleAxis(angle, axis);
		Vector3 d = transform.position - point;
		r.MovePosition(point + rotation * d);
		r.MoveRotation(rotation * transform.rotation);
	}

	// Scale the capsule collider to 'mlp' of the initial value
	protected void ScaleCapsule (float mlp) {
		if (capsule.height != originalHeight * mlp) {
			capsule.height = Mathf.MoveTowards (capsule.height, originalHeight * mlp, Time.deltaTime * 4);
			capsule.center = Vector3.MoveTowards (capsule.center, originalCenter * mlp, Time.deltaTime * 2);
		}
	}

	// Set the collider to high friction material
	protected void HighFriction() {
		capsule.material = highFrictionMaterial;
	}

	// Set the collider to zero friction material
	protected void ZeroFriction() {
		capsule.material = zeroFrictionMaterial;
	}

	// Get the damper of velocity on the slopes
	protected float GetSlopeDamper(Vector3 velocity, Vector3 groundNormal) {
		float angle = 90f - Vector3.Angle(velocity, groundNormal);
		angle -= customCharacterBaseData.slopeStartAngle;
		float range = customCharacterBaseData.slopeEndAngle - customCharacterBaseData.slopeStartAngle;
		return 1f - Mathf.Clamp(angle / range, 0f, 1f);
	}
}
