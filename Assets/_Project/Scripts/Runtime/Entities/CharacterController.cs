using System;
using System.Collections.Generic;
using KBCore.Refs;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entities
{
    public enum CharacterState { Default }
    public enum OrientationMethod { TowardsCamera, TowardsMovement }
    public enum BonusOrientationMethod { None, TowardsGravity, TowardsGroundSlopeAndGravity }

    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool CrouchDown;
        public bool CrouchUp;
    }

    public struct AICharacterInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }
    
    public class CharacterController : ValidatedMonoBehaviour, ICharacterController
    {
        [SerializeField, Self]
        private KinematicCharacterMotor _motor;

        [FoldoutGroup("Stable Movement"), SerializeField]
        private float _maxStableMoveSpeed = 10f;
        [FoldoutGroup("Stable Movement"), SerializeField]
        private float _stableMovementSharpness = 15f;
        [FoldoutGroup("Stable Movement"), SerializeField]
        private float _orientationSharpness = 10f;
        [FoldoutGroup("Stable Movement"), SerializeField]
        private OrientationMethod _orientationMethod = OrientationMethod.TowardsCamera;

        [FoldoutGroup("Air Movement"), SerializeField]
        private float _maxAirMoveSpeed = 15f;
        [FoldoutGroup("Air Movement"), SerializeField]
        private float _airAccelerationSpeed = 15f;
        [FoldoutGroup("Air Movement"), SerializeField]
        private float _drag = 0.1f;
        
        [FoldoutGroup("Jumping"), SerializeField]
        private bool _allowJumpingWhenSliding = false;
        [FoldoutGroup("Jumping"), SerializeField]
        private float _jumpUpSpeed = 10f;
        [FoldoutGroup("Jumping"), SerializeField]
        private float _jumpScalableForwardSpeed = 10f;
        [FoldoutGroup("Jumping"), SerializeField]
        private float _jumpPreGroundingGraceTime = 0f;
        [FoldoutGroup("Jumping"), SerializeField]
        private float _jumpPostGroundingGraceTime = 0f;

        [FoldoutGroup("Misc"), SerializeField]
        private List<Collider> _ignoreColliders = new();
        [FoldoutGroup("Misc"), SerializeField]
        private BonusOrientationMethod _bonusOrientationMethod = BonusOrientationMethod.None;
        [FoldoutGroup("Misc"), SerializeField]
        private float _bonusOrientationSharpness = 10f;
        [FoldoutGroup("Misc"), SerializeField]
        private Vector3 _gravity = new (0f, -30f, 0f);
        [FoldoutGroup("Misc"), SerializeField]
        private Transform _meshRoot;
        [field: FoldoutGroup("Misc"), SerializeField]
        public Transform CameraFollowPoint { get; private set; }
        [FoldoutGroup("Misc"), SerializeField]
        private float _crouchedCapsuleHeight = 0.25f;
        
        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;

        private Vector3 _lastInnerNormal = Vector3.zero;
        private Vector3 _lastOuterNormal = Vector3.zero;
        
        public CharacterState CurrentCharacterState { get; private set; }

        private void Awake()
        {
            TransitionToState(CharacterState.Default);
        }

        private void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialState = CurrentCharacterState;
            //OnStateExit(tmpInitialState, newState);
            // TODO: State transitions....
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public bool IsColliderValidForCollisions(Collider coll) => throw new System.NotImplementedException();

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            throw new System.NotImplementedException();
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport
        )
        {
            throw new System.NotImplementedException();
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport
        )
        {
            throw new System.NotImplementedException();
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            throw new System.NotImplementedException();
        }
    }
}