using System.Collections.Generic;
using KBCore.Refs;
using KinematicCharacterController;
using NJG.Runtime.Interactables;
using NJG.Runtime.Interfaces;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace NJG.Runtime.Entities
{
    public enum CharacterState { Default, Climbing }

    public enum ClimbingState { Anchoring, Climbing, DeAnchoring }

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
        public bool Interact;
    }

    public struct AIInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }

    public class CharacterController : ValidatedMonoBehaviour, ICharacterController, ILaunchable, IResetable
    {
        #region Serialized Fields

        [field: FoldoutGroup("References"), SerializeField, Self]
        public KinematicCharacterMotor Motor { get; private set; }
        [field: FoldoutGroup("References"), SerializeField, Child]
        public Animator Animator { get; private set; }

        [FoldoutGroup("References/VFX"), SerializeField]
        private GameObject _landVFX;

        [field: FoldoutGroup("Stable Movement"), SerializeField]
        public float MaxStableMoveSpeed { get; private set; } = 10f;
        [field: FoldoutGroup("Stable Movement"), SerializeField]
        public float StableMovementSharpness { get; private set; } = 15f;
        [field: FoldoutGroup("Stable Movement"), SerializeField]
        public float OrientationSharpness { get; private set; } = 10f;
        [field: FoldoutGroup("Stable Movement"), SerializeField]
        public OrientationMethod OrientationMethod { get; private set; } = OrientationMethod.TowardsCamera;

        [field: FoldoutGroup("Air Movement"), SerializeField]
        public float MaxAirMoveSpeed { get; private set; } = 15f;
        [field: FoldoutGroup("Air Movement"), SerializeField]
        public float AirAccelerationSpeed { get; private set; } = 15f;
        [field: FoldoutGroup("Air Movement"), SerializeField]
        public float Drag = 0.1f;

        [field: FoldoutGroup("Jumping"), SerializeField]
        public bool AllowJumping { get; private set; }
        [field: FoldoutGroup("Jumping"), SerializeField]
        public bool AllowJumpingWhenSliding { get; private set; }
        [field: FoldoutGroup("Jumping"), SerializeField]
        public bool AllowDoubleJump { get; private set; }
        [field: FoldoutGroup("Jumping"), SerializeField]
        public bool AllowWallJump { get; private set; }
        [field: FoldoutGroup("Jumping"), SerializeField]
        public float JumpSpeed { get; private set; } = 10f;
        [field: FoldoutGroup("Jumping"), SerializeField]
        public float JumpScalableForwardSpeed { get; private set; } = 10f;
        [field: FoldoutGroup("Jumping"), SerializeField]
        public float JumpPreGroundingGraceTime { get; private set; }
        [field: FoldoutGroup("Jumping"), SerializeField]
        public float JumpPostGroundingGraceTime { get; private set; }

        [field: FoldoutGroup("Ladder Climbing"), SerializeField]
        public float ClimbingSpeed { get; private set; } = 4f;
        [field: FoldoutGroup("Ladder Climbing"), SerializeField]
        public float AnchoringDuration { get; private set; } = 0.25f;
        [field: FoldoutGroup("Ladder Climbing"), SerializeField]
        public LayerMask LadderLayer { get; private set; }

        [field: FoldoutGroup("Misc"), SerializeField]
        public List<Collider> IgnoredColliders { get; private set; } = new();
        [field: FoldoutGroup("Misc"), SerializeField]
        public BonusOrientationMethod BonusOrientationMethod { get; private set; } = BonusOrientationMethod.None;
        [field: FoldoutGroup("Misc"), SerializeField]
        public float BonusOrientationSharpness { get; private set; } = 10f;
        [field: FoldoutGroup("Misc"), SerializeField]
        public Vector3 Gravity { get; private set; } = new(0f, -30f, 0f);
        [field: FoldoutGroup("Misc"), SerializeField]
        public Transform MeshRoot { get; private set; }
        [field: FoldoutGroup("Misc"), SerializeField]
        public Transform CameraFollowPoint { get; private set; }
        [field: FoldoutGroup("Misc"), SerializeField]
        public float CrouchedCapsuleHeight { get; private set; } = 0.25f;

        #endregion

        #region Private Fields

        private readonly Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _jumpRequested;
        private bool _jumpConsumed;
        private bool _doubleJumpConsumed;
        private bool _jumpedThisFrame;
        private bool _canWallJump;
        private Vector3 _wallJumpNormal;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching;
        private bool _isCrouching;

        private Vector3 _lastInnerNormal = Vector3.zero;
        private Vector3 _lastOuterNormal = Vector3.zero;

        private Quaternion _tmpTransientRot;
        private Player _player;

        // Ladder vars
        private float _ladderUpDownInput;
        private Ladder _activeLadder { get; set; }
        private ClimbingState _internalClimbingState;
        private ClimbingState _climbingState
        {
            get => _internalClimbingState;
            set
            {
                _internalClimbingState = value;
                _anchoringTimer = 0f;
                _anchoringStartPosition = Motor.TransientPosition;
                _anchoringStartRotation = Motor.TransientRotation;
            }
        }
        private Vector3 _ladderTargetPosition;
        private Quaternion _ladderTargetRotation;
        private float _onLadderSegmentState;
        private float _anchoringTimer;
        private Vector3 _anchoringStartPosition = Vector3.zero;
        private Quaternion _anchoringStartRotation = Quaternion.identity;
        private Quaternion _rotationBeforeClimbing = Quaternion.identity;

        // Animator Params
        private static readonly int _speedHash = Animator.StringToHash("Speed");
        private static readonly int _ySpeedHash = Animator.StringToHash("ySpeed");
        private static readonly int _climbAnimationHash = Animator.StringToHash("Climb");
        private static readonly int _locomotionAnimationHash = Animator.StringToHash("Locomotion");
        private static readonly int _airLocomotionAnimationHash = Animator.StringToHash("AirLocomotion");

        #endregion

        #region Other Properties

        public CharacterState CurrentCharacterState { get; private set; }
        public Vector3 StartPosition { get; private set; }
        public Quaternion StartRotation { get; private set; }

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            StartPosition = transform.position;
            StartRotation = transform.rotation;
        }

        private void Start()
        {
            Motor.CharacterController = this;
            TransitionToState(CharacterState.Default);
        }

        private void Update() => UpdateAnimator();

        #endregion

        #region State Transitions

        /// <summary>
        ///     Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        /// <summary>
        ///     Event when entering a state
        /// </summary>
        private void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                {
                    break;
                }
                case CharacterState.Climbing:
                {
                    _rotationBeforeClimbing = Motor.TransientRotation;

                    Motor.SetMovementCollisionsSolvingActivation(false);
                    Motor.SetGroundSolvingActivation(false);
                    _climbingState = ClimbingState.Anchoring;
                    Animator.Play(_climbAnimationHash);

                    // Store the target position and rotation to snap to
                    _ladderTargetPosition =
                        _activeLadder.ClosestPointOnLadderSegment(Motor.TransientPosition, out _onLadderSegmentState);
                    _ladderTargetRotation = _activeLadder.transform.rotation;
                    break;
                }
            }
        }

        /// <summary>
        ///     Event when exiting a state
        /// </summary>
        private void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                {
                    break;
                }
                case CharacterState.Climbing:
                {
                    Motor.SetMovementCollisionsSolvingActivation(true);
                    Motor.SetGroundSolvingActivation(true);
                    Animator.Play(_locomotionAnimationHash);
                    break;
                }
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(Player player) => _player = player;

        /// <summary>
        ///     This is called every frame by Player.cs in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            // Handle ladder transitions
            _ladderUpDownInput = inputs.MoveAxisForward;
            // TODO: !_player.Interactor.HasInteractable
            if (inputs.Interact)
                if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders,
                        LadderLayer, QueryTriggerInteraction.Collide) > 0)
                    if (_probedColliders[0] != null)
                        // Handle ladders
                        if (_probedColliders[0].gameObject.TryGetComponent(out Ladder ladder))
                        {
                            // Transition to ladder climbing state
                            if (CurrentCharacterState == CharacterState.Default)
                            {
                                _activeLadder = ladder;
                                TransitionToState(CharacterState.Climbing);
                            }
                            // Transition back to default movement state
                            else if (CurrentCharacterState == CharacterState.Climbing)
                            {
                                _climbingState = ClimbingState.DeAnchoring;
                                _ladderTargetPosition = Motor.TransientPosition;
                                _ladderTargetRotation = _rotationBeforeClimbing;
                            }
                        }

            //Clamp inputs
            Vector3 moveInputVector =
                Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection =
                Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp)
                                               .normalized;
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    // Move and look inputs
                    _moveInputVector = cameraPlanarRotation * moveInputVector;

                    _lookInputVector = OrientationMethod switch
                    {
                        OrientationMethod.TowardsCamera => cameraPlanarDirection,
                        OrientationMethod.TowardsMovement => _moveInputVector.normalized,
                        _ => _lookInputVector
                    };

                    // Jumping input
                    if (AllowJumping && inputs.JumpDown)
                    {
                        _timeSinceJumpRequested = 0f;
                        _jumpRequested = true;
                    }

                    // Crouching input
                    if (inputs.CrouchDown)
                    {
                        _shouldBeCrouching = true;

                        if (!_isCrouching)
                        {
                            _isCrouching = true;
                            Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                            MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                        }
                    }
                    else if (inputs.CrouchUp)
                    {
                        _shouldBeCrouching = false;
                    }

                    break;
                }
            }
        }

        public void HandleInputs(ref AIInputs inputs)
        {
            _moveInputVector = inputs.MoveVector;
            _lookInputVector = inputs.LookVector;
        }

        public void AddForce(Vector3 force) => AddVelocity(force);

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    Motor.ForceUnground();
                    _internalVelocityAdd += velocity;
                    break;
                }
            }
        }

        public void SetLocalScale(Vector3 localScale)
        {
            if (MeshRoot != null)
                MeshRoot.localScale = localScale;
        }

        #endregion

        #region Kinematic Character Controller Callbacks

        /// <summary>
        ///     (Called by KinematicCharacterMotor during its update cycle)
        ///     This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime) { }

        /// <summary>
        ///     (Called by KinematicCharacterMotor during its update cycle)
        ///     This is where you tell your character what its rotation should be right now.
        ///     This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
                    {
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector,
                            1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }

                    Vector3 currentUp = currentRotation * Vector3.up;
                    if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
                    {
                        // Rotate from current up to invert gravity
                        Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized,
                            1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                    }
                    else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
                    {
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            Vector3 initialCharacterBottomHemiCenter =
                                Motor.TransientPosition + currentUp * Motor.Capsule.radius;

                            Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp,
                                Motor.GroundingStatus.GroundNormal,
                                1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) *
                                              currentRotation;

                            // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                            Motor.SetTransientPosition(initialCharacterBottomHemiCenter +
                                                       currentRotation * Vector3.down * Motor.Capsule.radius);
                        }
                        else
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized,
                                1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) *
                                              currentRotation;
                        }
                    }
                    else
                    {
                        Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up,
                            1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                        currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                    }

                    break;
                }
                case CharacterState.Climbing:
                {
                    currentRotation = _climbingState switch
                    {
                        ClimbingState.Climbing => _activeLadder.transform.rotation,
                        ClimbingState.Anchoring or ClimbingState.DeAnchoring => Quaternion.Slerp(
                            _anchoringStartRotation, _ladderTargetRotation, _anchoringTimer / AnchoringDuration),
                        _ => currentRotation
                    };
                    break;
                }
            }
        }

        /// <summary>
        ///     (Called by KinematicCharacterMotor during its update cycle)
        ///     This is where you tell your character what its velocity should be right now.
        ///     This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    Vector3 targetMovementVelocity = Vector3.zero;
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        // Reorient velocity on slope
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity,
                            Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                        // Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        Vector3 reorientedInput =
                            Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized *
                            _moveInputVector.magnitude;
                        targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                        // Smooth movement Velocity
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
                            1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                    }
                    else
                    {
                        // Add move input
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                            // Prevent climbing on un-stable slopes with air movement
                            if (Motor.GroundingStatus.FoundAnyGround)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3
                                                                         .Cross(
                                                                             Vector3.Cross(Motor.CharacterUp,
                                                                                 Motor.GroundingStatus.GroundNormal),
                                                                             Motor.CharacterUp).normalized;
                                targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity,
                                    perpenticularObstructionNormal);
                            }

                            Vector3 velocityDiff =
                                Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                            currentVelocity += velocityDiff * (AirAccelerationSpeed * deltaTime);
                        }

                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= 1f / (1f + Drag * deltaTime);
                    }

                    // Handle jumping
                    {
                        _jumpedThisFrame = false;
                        _timeSinceJumpRequested += deltaTime;
                        if (_jumpRequested)
                        {
                            // Handle double jump
                            if (AllowDoubleJump)
                                if (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding
                                        ? !Motor.GroundingStatus.FoundAnyGround
                                        : !Motor.GroundingStatus.IsStableOnGround))
                                {
                                    Motor.ForceUnground();

                                    // Add to the return velocity and reset jump state
                                    currentVelocity += Motor.CharacterUp * JumpSpeed - Vector3.Project(currentVelocity,
                                        Motor.CharacterUp);
                                    _jumpRequested = false;
                                    _doubleJumpConsumed = true;
                                    _jumpedThisFrame = true;
                                }

                            // See if we actually are allowed to jump
                            if (_canWallJump || (!_jumpConsumed &&
                                                 ((AllowJumpingWhenSliding
                                                      ? Motor.GroundingStatus.FoundAnyGround
                                                      : Motor.GroundingStatus.IsStableOnGround) ||
                                                  _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)))
                            {
                                // Calculate jump direction before ungrounding
                                Vector3 jumpDirection = Motor.CharacterUp;
                                if (_canWallJump)
                                    jumpDirection = _wallJumpNormal;
                                else if (Motor.GroundingStatus.FoundAnyGround &&
                                         !Motor.GroundingStatus.IsStableOnGround)
                                    jumpDirection = Motor.GroundingStatus.GroundNormal;

                                // Makes the character skip ground probing/snapping on its next update. 
                                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                Motor.ForceUnground();

                                // Add to the return velocity and reset jump state
                                currentVelocity += jumpDirection * JumpSpeed - Vector3.Project(currentVelocity,
                                    Motor.CharacterUp);
                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;
                            }
                        }

                        // Reset wall jump
                        _canWallJump = false;
                    }

                    // Take into account additive velocity
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }

                    break;
                }
                case CharacterState.Climbing:
                {
                    currentVelocity = Vector3.zero;

                    switch (_climbingState)
                    {
                        case ClimbingState.Climbing:
                            currentVelocity = (_ladderUpDownInput * _activeLadder.transform.up).normalized *
                                              ClimbingSpeed;
                            break;
                        case ClimbingState.Anchoring:
                        case ClimbingState.DeAnchoring:
                            Vector3 tmpPosition = Vector3.Lerp(_anchoringStartPosition, _ladderTargetPosition,
                                _anchoringTimer / AnchoringDuration);
                            currentVelocity =
                                Motor.GetVelocityForMovePosition(Motor.TransientPosition, tmpPosition, deltaTime);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///     (Called by KinematicCharacterMotor during its update cycle)
        ///     This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    // Handle jump-related values
                    {
                        // Handle jumping pre-ground grace period
                        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                            _jumpRequested = false;

                        if (AllowJumpingWhenSliding
                                ? Motor.GroundingStatus.FoundAnyGround
                                : Motor.GroundingStatus.IsStableOnGround)
                        {
                            // If we're on a ground surface, reset jumping values
                            if (!_jumpedThisFrame)
                            {
                                _doubleJumpConsumed = false;
                                _jumpConsumed = false;
                            }

                            _timeSinceLastAbleToJump = 0f;
                        }
                        else
                        {
                            // Keep track of time since we were last able to jump (for grace period)
                            _timeSinceLastAbleToJump += deltaTime;
                        }
                    }

                    // Handle uncrouching
                    if (_isCrouching && !_shouldBeCrouching)
                    {
                        // Do an overlap test with the character's standing height to see if there are any obstructions
                        Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                        if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders,
                                Motor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
                        {
                            // If obstructions, just stick to crouching dimensions
                            Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                        }
                        else
                        {
                            // If no obstructions, uncrouch
                            MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                            _isCrouching = false;
                        }
                    }

                    break;
                }
                case CharacterState.Climbing:
                {
                    switch (_climbingState)
                    {
                        case ClimbingState.Climbing:
                            // Detect getting off ladder during climbing
                            _activeLadder.ClosestPointOnLadderSegment(Motor.TransientPosition,
                                out _onLadderSegmentState);
                            if (Mathf.Abs(_onLadderSegmentState) > 0.05f)
                            {
                                _climbingState = ClimbingState.DeAnchoring;

                                // If we're higher than the ladder top point
                                if (_onLadderSegmentState > 0)
                                {
                                    _ladderTargetPosition = _activeLadder.TopReleasePoint.position;
                                    _ladderTargetRotation = _activeLadder.TopReleasePoint.rotation;
                                }
                                // If we're lower than the ladder bottom point
                                else if (_onLadderSegmentState < 0)
                                {
                                    _ladderTargetPosition = _activeLadder.BottomReleasePoint.position;
                                    _ladderTargetRotation = _activeLadder.BottomReleasePoint.rotation;
                                }
                            }

                            break;
                        case ClimbingState.Anchoring:
                        case ClimbingState.DeAnchoring:
                            // Detect transitioning out from anchoring states
                            if (_anchoringTimer >= AnchoringDuration)
                            {
                                if (_climbingState == ClimbingState.Anchoring)
                                    _climbingState = ClimbingState.Climbing;
                                else if (_climbingState == ClimbingState.DeAnchoring)
                                    TransitionToState(CharacterState.Default);
                            }

                            // Keep track of time since we started anchoring
                            _anchoringTimer += deltaTime;
                            break;
                    }

                    break;
                }
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
                OnLanded();
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
                OnLeaveStableGround();
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
                return true;

            return !IgnoredColliders.Contains(coll);
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport
        ) { }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport
        )
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                {
                    // We can wall jump only if we are not stable on ground and are moving against an obstruction
                    if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                    {
                        _canWallJump = true;
                        _wallJumpNormal = hitNormal;
                    }

                    break;
                }
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport
        ) { }

        public void OnDiscreteCollisionDetected(Collider hitCollider) { }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Called when the character lands on a stable ground
        /// </summary>
        private void OnLanded()
        {
            Animator.Play(_locomotionAnimationHash);
            if (_landVFX)
                Instantiate(_landVFX, transform.position, quaternion.identity);
        }

        /// <summary>
        ///     Called when the character leaves a stable ground
        /// </summary>
        private void OnLeaveStableGround()
        {
            Animator.Play(_airLocomotionAnimationHash);
        }

        private void UpdateAnimator()
        {
            float animSpeedValue = Motor.GroundingStatus.IsStableOnGround ? Motor.BaseVelocity.magnitude : 0f;
            Animator.SetFloat(_speedHash, animSpeedValue);

            Animator.SetFloat(_ySpeedHash, Motor.Velocity.y);
        }

        public void ResetState() => Motor.SetPositionAndRotation(StartPosition, StartRotation);

        #endregion
    }
}