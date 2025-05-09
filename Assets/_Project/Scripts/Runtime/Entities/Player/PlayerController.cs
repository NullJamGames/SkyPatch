using System;
using System.Collections.Generic;
using KBCore.Refs;
using NJG.Runtime.Input;
using NJG.Runtime.Interactables;
using NJG.Utilities.ImprovedTimers;
using NJG.Utilities.PredicateStateMachines;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class PlayerController : ValidatedMonoBehaviour //, IResetable, IPlatformRider, ILaunchable, IPlatformStopper
    {
        private const float ZERO_F = 0f;

        // Animator Params
        private static readonly int _speedHash = Animator.StringToHash("Speed");
        private static readonly int _climbSpeedHash = Animator.StringToHash("climbSpeed");
        [FoldoutGroup("References"), SerializeField, Self]
        private Rigidbody _rigidBody;
        [FoldoutGroup("References"), SerializeField, Child]
        private GroundChecker _groundChecker;
        [FoldoutGroup("References"), SerializeField, Anywhere]
        private Animator _animator;
        [FoldoutGroup("References"), SerializeField, Anywhere]
        private CinemachineCamera _virtualCamera;
        [FoldoutGroup("References"), SerializeField, Anywhere]
        private InputReader _input;
        [FoldoutGroup("References"), SerializeField, Self]
        private PlayerInventory _inventory;

        [FoldoutGroup("Movement Settings"), SerializeField]
        private float _moveSpeed = 6f;
        [FoldoutGroup("Movement Settings"), SerializeField]
        private float _rotationSpeed = 15f;
        [FoldoutGroup("Movement Settings"), SerializeField]
        private float _smoothTime = 0.2f;
        [FoldoutGroup("Movement Settings"), SerializeField]
        private float _stopSmoothTime = 0.04f;
        [FoldoutGroup("Movement Settings"), SerializeField]
        private float _airRotationSpeed = 100f;
        [FoldoutGroup("Movement Settings"), SerializeField]
        private float _airSmoothTime = 1f;
        [FoldoutGroup("Movement Settings"), SerializeField]
        private float _airStopSmoothTime = 0.3f;
        [FoldoutGroup("Movement Settings"), SerializeField]
        private float _extraGravityForce = 20f;

        [FoldoutGroup("Jump Settings"), SerializeField]
        private float _jumpForce = 10f;
        [FoldoutGroup("Jump Settings"), SerializeField]
        private float _jumpDuration = 0.5f;
        [FoldoutGroup("Jump Settings"), SerializeField]
        private float _jumpCooldown;
        [FoldoutGroup("Jump Settings"), SerializeField]
        private float _gravityMultiplier = 3f;

        [FoldoutGroup("Dash Settings"), SerializeField]
        private float _dashForce = 10f;
        [FoldoutGroup("Dash Settings"), SerializeField]
        private float _dashDuration = 1f;
        [FoldoutGroup("Dash Settings"), SerializeField]
        private float _dashCooldown = 2f;

        [FoldoutGroup("Climb Settings"), SerializeField]
        private float _climbSpeed = 100f;
        [FoldoutGroup("Climb Settings"), SerializeField]
        private float _climbSmoothTime = 0.2f;
        [FoldoutGroup("Climb Settings"), SerializeField]
        private LayerMask _ladderLayer;
        [FoldoutGroup("Climb Settings"), SerializeField]
        private float _ladderDistance = 1;
        [FoldoutGroup("Climb Settings"), SerializeField]
        private float _climbCooldown = 0.8f;
        private float _climbVelocity;
        private float _currentClimbSpeed;
        private Vector2 _currentHorizontalSpeed;

        private float _currentSpeed;
        private CountdownTimer _dashCooldownTimer;
        private CountdownTimer _dashTimer;
        private float _dashVelocity = 1f;
        private Func<Vector3> _getPlatformerSpeed;

        private bool _isClimbing;

        private bool _isOnMovingPlatform;
        private CountdownTimer _jumpCooldownTimer;
        private CountdownTimer _jumpTimer;
        private float _jumpVelocity;

        private Camera _mainCamera;
        private Vector3 _movement;
        private OldLadder _oldLadder;

        private Vector3 _requestedForce = Vector3.zero;
        private StateMachine _stateMachine;
        private List<Timer> _timers;
        private Vector2 _velocity;

        // public Vector3 StartPosition { get; private set; }
        // public Quaternion StartRotation { get; private set; }
        //
        // public PlayerInteractor Interactor { get; private set; }
        //
        // public bool IsGrounded => _groundChecker.IsGrounded || _isOnMovingPlatform;
        // public Rigidbody Rigidbody => _rigidBody;
        // public bool HasLaunched { get; set; }
        //
        // private void Awake()
        // {
        //     Interactor = GetComponent<PlayerInteractor>();
        //     
        //     StartPosition = transform.position;
        //     StartRotation = transform.rotation;
        //     _mainCamera = Camera.main;
        //     _virtualCamera.Follow = transform;
        //     _virtualCamera.LookAt = transform;
        //     _virtualCamera.OnTargetObjectWarped(transform, transform.position - _virtualCamera.transform.position - Vector3.forward);
        //     
        //     _rigidBody.freezeRotation = true;
        //     
        //     SetupTimers();
        //     SetupStateMachine();
        // }
        //
        // private void SetupStateMachine()
        // {
        //     _stateMachine = new StateMachine();
        //
        //     // Declare States
        //     LocomotionState _locomotionState = new LocomotionState(this, _animator);
        //     JumpState _jumpState = new JumpState(this, _animator);
        //     DashState _dashState = new DashState(this, _animator);
        //     ClimbState _climbState = new ClimbState(this, _animator);
        //     LaunchedState _launchedState = new LaunchedState(this, _animator);
        //
        //     // Define transitions
        //     At(_locomotionState, _jumpState, new FuncPredicate(() => _jumpTimer.IsRunning));
        //     At(_locomotionState, _dashState, new FuncPredicate(() => _dashTimer.IsRunning));
        //     At(_climbState, _locomotionState, new FuncPredicate(() => !_isClimbing));
        //     At(_launchedState, _locomotionState, new FuncPredicate(() => !HasLaunched));
        //     Any(_launchedState, new FuncPredicate(() => HasLaunched));
        //     Any(_climbState, new FuncPredicate(() => _isClimbing));
        //     Any(_locomotionState, new FuncPredicate(ReturnToLocomotionState));
        //
        //     _stateMachine.SetState(_locomotionState);
        // }
        //
        // private bool ReturnToLocomotionState()
        // {
        //     return IsGrounded
        //            && !_jumpTimer.IsRunning
        //            && !_dashTimer.IsRunning
        //            && !_isClimbing;
        // }
        //
        // private void SetupTimers()
        // {
        //     // Setup timers
        //     _jumpTimer = new CountdownTimer(_jumpDuration);
        //     _jumpCooldownTimer = new CountdownTimer(_jumpCooldown);
        //
        //     _jumpTimer.OnTimerStart += JumpStart ;
        //     _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();
        //
        //     _dashTimer = new CountdownTimer(_dashDuration);
        //     _dashCooldownTimer = new CountdownTimer(_dashCooldown);
        //     _dashTimer.OnTimerStart += () => _dashVelocity = _dashForce;
        //     _dashTimer.OnTimerStop += () =>
        //     {
        //         _dashVelocity = 1f;
        //         _dashCooldownTimer.Start();
        //     };
        //     
        //     _timers = new List<Timer>(4) { _jumpTimer, _jumpCooldownTimer, _dashTimer, _dashCooldownTimer};
        // }
        //
        // private void OnEnable()
        // {
        //     _input.JumpEvent += OnJump;
        //     _input.DashEvent += OnDash;
        //     _input.InteractEvent += OnInteract;
        //     _input.PickupEvent += OnPickup;
        // }
        //
        // private void Start() => _input.EnablePlayerActions();
        //
        // private void Update()
        // {
        //     _movement = new Vector3(_input.MoveDirection.x, 0f, _input.MoveDirection.y);
        //     _stateMachine.Update();
        //
        //     HandleTimers();
        //     
        //     UpdateAnimator();
        // }
        //
        // private void FixedUpdate()
        // {
        //     // TODO: Hacky way for now... need better solution
        //     if (_requestedForce != Vector3.zero)
        //     {
        //         _rigidBody.AddForce(_requestedForce, ForceMode.Impulse);
        //         _requestedForce = Vector3.zero;
        //     }
        //
        //     _stateMachine.FixedUpdate();
        // }
        //
        // private void OnDisable()
        // {
        //     _input.JumpEvent -= OnJump;
        //     _input.DashEvent -= OnDash;
        //     _input.InteractEvent -= OnInteract;
        //     _input.PickupEvent -= OnPickup;
        //     
        //     if (_input != null)
        //         _input.DisablePlayerActions();
        // }
        //
        // private void At(IState from, IState to, IPredicate condition) =>
        //     _stateMachine.AddTransition(from, to, condition);
        // private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
        //
        // private void HandleTimers()
        // {
        //     foreach (Timer timer in _timers)
        //     {
        //         timer.Tick(Time.deltaTime);
        //     }
        // }
        //
        // private void OnInteract() => Interactor.Interact();
        //
        // private void CheckForClimbing()
        // {
        //     if (Physics.Raycast(transform.position, _rigidBody.linearVelocity.normalized, out RaycastHit hitInfo, _ladderDistance, _ladderLayer))
        //         if (hitInfo.collider.TryGetComponent(out Ladder ladder))
        //             EnterClimbState(ladder);
        // }
        //
        // private void EnterClimbState(Ladder ladder)
        // {
        //     if(_isClimbing)
        //         return;
        //     
        //     _ladder = ladder;
        //     if (ShouldExitImmediately())
        //         return;
        //     
        //     _isClimbing = true;
        //     _rigidBody.useGravity = false;
        //     ResetSpeed();
        //     SetClimbStartTransform();
        // }
        //
        // private void SetClimbStartTransform()
        // {
        //     Transform playerTransform = transform;
        //     
        //     Vector3 climbStartPosition = _ladder.GetClimbPosition();
        //     climbStartPosition.y =  playerTransform.position.y;
        //     
        //     playerTransform.position = climbStartPosition;
        //     playerTransform.rotation = _ladder.ClimbRotation;
        // }
        //
        // private bool ShouldExitImmediately()
        // {
        //     bool isAtTop = _ladder.IsCloserToTopPoint(transform.position.y);
        //     float desiredSpeed = 1;
        //     
        //     if (Vector3.Dot(_rigidBody.linearVelocity, _ladder.ClimbRotation * Vector3.forward) < 0)
        //         desiredSpeed *= -1;
        //
        //     if (isAtTop && desiredSpeed > 0)
        //         return true;
        //
        //     if (!isAtTop && desiredSpeed < 0)
        //         return true;
        //     return false;
        // }
        //
        // public void HandleClimb()
        // {
        //     HandleClimbMovement();
        //     HandleClimbExit();
        // }
        //
        // private void HandleClimbMovement()
        // {
        //     float desiredSpeed = _movement.z * _climbSpeed;
        //     
        //     Vector3 camForward = Quaternion.AngleAxis(_mainCamera.transform.eulerAngles.y, Vector3.up) * Vector3.forward;
        //     if (Vector3.Dot(_ladder.ClimbRotation * Vector3.forward, camForward) < 0)
        //         desiredSpeed *= -1;
        //     
        //     _currentClimbSpeed = Mathf.SmoothDamp(_currentClimbSpeed, desiredSpeed, ref _climbVelocity, _climbSmoothTime);
        //     _rigidBody.linearVelocity = new Vector3(0, _currentClimbSpeed, 0);
        // }
        //
        // private void HandleClimbExit()
        // {
        //     if (_currentClimbSpeed < 0)
        //     {
        //         if (_ladder.GetBottomHeight() > transform.position.y)
        //             ExitClimb();
        //     }
        //     else if(_currentClimbSpeed > 0)
        //         if (_ladder.GetTopHeight() < transform.position.y)
        //         {
        //             transform.position = _ladder.GetTopExitPos();
        //             ExitClimb();
        //         }
        // }
        //
        // private void ExitClimb()
        // {
        //     _isClimbing = false;
        //     _rigidBody.useGravity = true;
        // }
        //
        // private void OnJump(bool performed)
        // {
        //     // TODO: Testing if removing jump is a good idea
        //     return;
        //     
        //     if (performed && !_jumpTimer.IsRunning && !_jumpCooldownTimer.IsRunning && (IsGrounded))
        //     {
        //         _jumpTimer.Start();
        //     }
        //     else if (!performed && _jumpTimer.IsRunning)
        //     {
        //         _jumpTimer.Stop();
        //     }
        // }
        //
        // private void OnDash(bool performed)
        // {
        //     if (performed && !_dashTimer.IsRunning && !_dashCooldownTimer.IsRunning)
        //     {
        //         _dashTimer.Start();
        //     }
        //     else if (!performed && _dashTimer.IsRunning)
        //     {
        //         _dashTimer.Stop();
        //     }
        // }
        //
        // private void OnPickup() => _inventory.Drop();
        //
        // private void JumpStart()
        // {
        //     _jumpVelocity = _jumpForce;
        //     transform.SetParent(null);
        //     
        //     if(_getPlatformerSpeed == null)
        //         return;
        //
        //     float ySpeedOfPlatform = _getPlatformerSpeed().y;
        //     if(ySpeedOfPlatform > 0)
        //         _jumpVelocity += ySpeedOfPlatform;
        // }
        //
        // public void JumpBoostHorizontalSpeed()
        // {
        //     _currentHorizontalSpeed.x += _movement.x;
        //     _currentHorizontalSpeed.y += _movement.z;
        //     _currentHorizontalSpeed *= 0.5f;
        // }
        //
        // public void HandleJump()
        // {
        //     if (!_jumpTimer.IsRunning && IsGrounded)
        //     {
        //         _jumpVelocity = ZERO_F;
        //         _jumpTimer.Stop();
        //         return;
        //     }
        //     
        //     if (!_jumpTimer.IsRunning)
        //     {
        //         // Gravity takes over
        //         _jumpVelocity += Physics.gravity.y * _gravityMultiplier * Time.fixedDeltaTime;
        //     }
        //     
        //     // Apply velocity
        //     _rigidBody.linearVelocity = new Vector3(_rigidBody.linearVelocity.x, _jumpVelocity, _rigidBody.linearVelocity.z);
        // }
        //
        // public void HandleMovement()
        // {
        //     // Rotate movement direction to match camera rotation
        //     Vector3 adjustedDirection = Quaternion.AngleAxis(_mainCamera.transform.eulerAngles.y, Vector3.up) * _movement;
        //
        //     if (adjustedDirection.magnitude > ZERO_F)
        //     {
        //         HandleRotation(adjustedDirection);
        //         SmoothSpeed(adjustedDirection * _moveSpeed);
        //         HandleHorizontalMovement();
        //         HandleExtraGravity();
        //         CheckForClimbing();
        //     }
        //     else
        //     {
        //         SmoothSpeedZeroMovementInput();
        //         HandleHorizontalMovement();
        //         HandleExtraGravity();
        //     }
        // }
        //
        // private void HandleRotation(Vector3 adjustedDirection)
        // {
        //     // Adjust rotation to match movement direction
        //     Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
        //     float rotationSpeed = IsGrounded? _rotationSpeed : _airRotationSpeed;
        //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        // }
        //
        // private void HandleHorizontalMovement()
        // {
        //     Vector2 velocity = _currentHorizontalSpeed *  _dashVelocity;
        //     _rigidBody.linearVelocity = new Vector3(velocity.x, _rigidBody.linearVelocity.y, velocity.y);
        // }
        //
        // private void HandleExtraGravity()
        // {
        //     float verticalVelocity = _rigidBody.linearVelocity.y - _extraGravityForce * Time.deltaTime;
        //     SetRigidBodyVerticalVelocity(verticalVelocity);
        // }
        //
        // private void SmoothSpeed(Vector3 desiredSpeed)
        // {
        //     Vector2 desiredHorizontalSpeed = new Vector2(desiredSpeed.x, desiredSpeed.z);
        //     float smoothTime = IsGrounded? _smoothTime: _airSmoothTime;
        //     _currentHorizontalSpeed = Vector2.SmoothDamp(_currentHorizontalSpeed, desiredHorizontalSpeed, ref _velocity, smoothTime);
        // }
        //
        // private void SmoothSpeedZeroMovementInput()
        // {
        //     Vector2 desiredHorizontalSpeed = Vector2.zero;
        //     float smoothTime = IsGrounded? _stopSmoothTime: _airStopSmoothTime;
        //     _currentHorizontalSpeed = Vector2.SmoothDamp(_currentHorizontalSpeed, desiredHorizontalSpeed, ref _velocity, smoothTime);
        // }
        //
        // private void SetRigidBodyVerticalVelocity(float verticalVelocity)
        // {
        //     Vector3 velocity = _rigidBody.linearVelocity;
        //     velocity.y = verticalVelocity;
        //     _rigidBody.linearVelocity = velocity;
        // }
        //
        // private void UpdateAnimator()
        // {
        //     //Debug.Log(_rigidBody.linearVelocity.y);
        //     _animator.SetFloat(_climbSpeedHash, _currentClimbSpeed);
        //     
        //     float animSpeedValue = IsGrounded ? _currentHorizontalSpeed.magnitude : 0f;
        //     _animator.SetFloat(_speedHash, animSpeedValue);
        // }
        //
        // public void AttachToPlatform(Transform platform)
        // {
        //     _isOnMovingPlatform = true;
        //     transform.SetParent(platform);
        // }
        //
        // public void DetachFromPlatform()
        // {
        //     _isOnMovingPlatform = false;
        //     transform.SetParent(null);
        //     _getPlatformerSpeed = null;
        // }
        //
        // public void SetGetPlatformerSpeedDelegate(Func<Vector3> getPlatformerSpeedDelegate)
        // {
        //     _getPlatformerSpeed = getPlatformerSpeedDelegate;
        // }
        //
        // public void Launch(Vector3 force)
        // {
        //     _requestedForce = force;
        //     HasLaunched = true;
        //     ResetSpeed();
        // }
        //
        // public void ResetState()
        // {
        //     _rigidBody.transform.position = StartPosition;
        //     _rigidBody.linearVelocity = Vector3.zero;
        //     _rigidBody.angularVelocity = Vector3.zero;
        // }
        //
        // private void ResetSpeed()
        // {
        //     _rigidBody.linearVelocity = Vector3.zero;
        //     
        //     _currentHorizontalSpeed = Vector2.zero;
        //     _velocity = Vector2.zero;
        //     
        //     _currentClimbSpeed = 0;
        //     _climbVelocity = 0;
        // }
    }
}