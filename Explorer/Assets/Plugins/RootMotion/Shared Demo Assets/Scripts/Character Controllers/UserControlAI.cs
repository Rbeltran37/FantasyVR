using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

namespace RootMotion.Demos {
	
	/// <summary>
	/// User input for an AI controlled character controller.
	/// </summary>
	public class UserControlAI : UserControlThirdPerson {

		public Transform moveTarget;
		public float stoppingDistance = 0.5f;
		public float stoppingThreshold = 1.5f;
        public Navigator navigator;
        
        [SerializeField] private CharacterPuppet characterPuppet;
        [SerializeField] private float topSpeed = 1;
        [SerializeField] private float walkSpeed = .5f;
        [SerializeField] private float strafeSpeed = .7f;
        [SerializeField] private float walkStopDistance = .5f;
        [SerializeField] private float accelerationSpeed = .1f;
        
        
        private bool _isWalking = false;
        private bool _isChasing = false;
        private bool _isStrafing = false;
        private bool _isStrafingLeft = false;
        private bool _isMovingBackward = false;
        
        protected override void Start()
        {
            base.Start();

            navigator.Initiate(transform);
        }

        protected override void Update () {
	        float moveSpeed = walkByDefault? walkSpeed: topSpeed;

            // If using Unity Navigation
            if (navigator.activeTargetSeeking)
            {
                navigator.Update(moveTarget.position);
                state.move = navigator.normalizedDeltaPosition * moveSpeed;
            }
            // No navigation, just move straight to the target
            else
            {
	            if (_isChasing)
	            {
		            MoveStraightToTarget(moveSpeed);
	            }
	            else if (_isWalking)
	            {
		            WalkToTarget(walkSpeed);
	            }
	            else if (_isStrafing)
	            {
		            Strafe(strafeSpeed, _isStrafingLeft);
	            }
	            else if (_isMovingBackward)
	            {
		            MoveAwayFromTarget(moveSpeed);
	            }
	            else
	            {
		            StopMoving();
	            }
            }
		}

        private void StopMoving()
        {
	        characterPuppet.moveMode = CharacterThirdPerson.MoveMode.Directional;
	        characterPuppet.IsMovingBackward = false;
	        
	        Vector3 direction = moveTarget.position - transform.position;
	        float distance = direction.magnitude;

	        Vector3 normal = transform.up;
	        Vector3.OrthoNormalize(ref normal, ref direction);

	        float sD = state.move != Vector3.zero ? stoppingDistance : stoppingDistance * stoppingThreshold;

	        state.move = distance > sD ? direction * 0 : Vector3.zero;
	        state.lookPos = moveTarget.position;
        }
        
        private void MoveStraightToTarget(float moveSpeed)
        {
	        characterPuppet.moveMode = CharacterThirdPerson.MoveMode.Directional;
	        characterPuppet.IsMovingBackward = false;
	        
	        Vector3 direction = moveTarget.position - transform.position;
	        float distance = direction.magnitude;

	        Vector3 normal = transform.up;
	        Vector3.OrthoNormalize(ref normal, ref direction);

	        float sD = state.move != Vector3.zero ? stoppingDistance : stoppingDistance * stoppingThreshold;

	        state.move = distance > sD ? direction * moveSpeed : Vector3.zero;
	        state.lookPos = moveTarget.position;
        }
        
        private void WalkToTarget(float moveSpeed)
        {
	        characterPuppet.moveMode = CharacterThirdPerson.MoveMode.Directional;
	        characterPuppet.IsMovingBackward = false;
	        
	        Vector3 direction = moveTarget.position - transform.position;
	        float distance = direction.magnitude;

	        Vector3 normal = transform.up;
	        Vector3.OrthoNormalize(ref normal, ref direction);

	        float sD = state.move != Vector3.zero ? stoppingDistance : stoppingDistance * stoppingThreshold;

	        state.move = distance > walkStopDistance ? direction * moveSpeed : Vector3.zero;
	        state.lookPos = moveTarget.position;
        }

        private void MoveAwayFromTarget(float moveSpeed)
        {
	        characterPuppet.moveMode = CharacterThirdPerson.MoveMode.Strafe;
	        characterPuppet.IsMovingBackward = true;
	        
	        Vector3 direction = moveTarget.position - transform.position;
	        direction *= -1;
	        float distance = direction.magnitude;

	        Vector3 normal = transform.up;
	        Vector3.OrthoNormalize(ref normal, ref direction);

	        float sD = state.move != Vector3.zero ? stoppingDistance : stoppingDistance * stoppingThreshold;

	        //state.move = distance > sD ? direction * moveSpeed : Vector3.zero;
	        state.move = direction * moveSpeed;
	        state.lookPos = moveTarget.position;
        }

        private void Strafe(float moveSpeed, bool isStrafingLeft)
        {
	        characterPuppet.moveMode = CharacterThirdPerson.MoveMode.Strafe;
	        characterPuppet.IsMovingBackward = false;
	        
	        Vector3 direction = moveTarget.position - transform.position;
	        if (isStrafingLeft)
	        {
		        direction = Quaternion.Euler(0, -90, 0) * direction;
	        }
	        else
	        {
		        direction = Quaternion.Euler(0, 90, 0) * direction;
	        }
	        
	        float distance = direction.magnitude;
	        Vector3 normal = transform.up;
	        Vector3.OrthoNormalize(ref normal, ref direction);

	        float sD = state.move != Vector3.zero ? stoppingDistance : stoppingDistance * stoppingThreshold;

	        state.move = distance > sD ? direction * moveSpeed : Vector3.zero;
	        state.move = direction * moveSpeed;
	        state.lookPos = moveTarget.position;
        }

        // Visualize the navigator
        void OnDrawGizmos()
        {
            if (navigator.activeTargetSeeking) navigator.Visualize();
        }
        
        [Button]
        public void Stop()
        {
	        _isChasing = false;
	        _isWalking = false;
	        _isStrafing = false;
	        _isMovingBackward = false;
        }

        [Button]
        public void Chase()
        {
	        _isChasing = true;
	        _isWalking = false;
	        _isStrafing = false;
	        _isMovingBackward = false;
        }
        
        [Button]
        public void Walk()
        {
	        _isChasing = false;
	        _isWalking = true;
	        _isStrafing = false;
	        _isMovingBackward = false;
        }

        [Button]
        public void StrafeRandom()
        {
	        _isChasing = false;
	        _isWalking = false;
	        _isStrafing = true;
	        _isMovingBackward = false;
	        
	        int random = Random.Range(0, 2);
	        if (random == 1)
	        {
		        _isStrafingLeft = true;
	        }
	        else
	        {
		        _isStrafingLeft = false;
	        }
        }
        
        [Button]
        public void StrafeLeft()
        {
	        _isChasing = false;
	        _isWalking = false;
	        _isStrafing = true;
	        _isMovingBackward = false;
	        
	        _isStrafingLeft = true;
        }

        [Button]
        public void StrafeRight()
        {
	        _isChasing = false;
	        _isWalking = false;
	        _isStrafing = true;
	        _isMovingBackward = false;
	        
	        _isStrafingLeft = false;
        }
        
        [Button]
        public void MoveBackwards()
        {
	        _isChasing = false;
	        _isWalking = false;
	        _isStrafing = false;
	        _isMovingBackward = true;
        }

        //TODO Optimize
        public float GetDistance()
        {
	        Vector3 direction = moveTarget.position - transform.position;
	        float distance = direction.magnitude;
	        return distance;
        }
	}
}

