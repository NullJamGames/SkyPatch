using System;
using System.Collections.Generic;
using KBCore.Refs;
using NJG.Runtime.Input;
using NJG.Runtime.Interactables;
using NJG.Runtime.Interfaces;
using NJG.Runtime.Managers;
using NJG.Utilities.PredicateStateMachines;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public class PlayerController : ValidatedMonoBehaviour, IResetable
    {
        [FoldoutGroup("References"), SerializeField, Self]
        private Rigidbody _rigidBody;
        [FoldoutGroup("References"), SerializeField, Self]
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

        [FoldoutGroup("Jump Settings"), SerializeField]
        private float _jumpForce = 10f;
        [FoldoutGroup("Jump Settings"), SerializeField]
        private float _jumpDuration = 0.5f;
        [FoldoutGroup("Jump Settings"), SerializeField]
        private float _jumpCooldown = 0f;
        [FoldoutGroup("Jump Settings"), SerializeField]
        private float _gravityMultiplier = 3f;

        [FoldoutGroup("Dash Settings"), SerializeField]
        private float _dashForce = 10f;
        [FoldoutGroup("Dash Settings"), SerializeField]
        private float _dashDuration = 1f;
        [FoldoutGroup("Dash Settings"), SerializeField]
        private float _dashCooldown = 2f;
        
        [FoldoutGroup("Interact Settings"), SerializeField]
        private float _interactCooldown = 0.5f;
        [FoldoutGroup("Interact Settings"), SerializeField]
        private float _interactDistance = 1f;
        [FoldoutGroup("Interact Settings"), SerializeField]
        private LayerMask _interactableLayers;
        
        private Camera _mainCamera;
        private StateMachine _stateMachine;

        private float _currentSpeed;
        private float _velocity;
        private float _jumpVelocity;
        private float _dashVelocity = 1f;
        private Vector3 _movement;
        private List<Timer> _timers;
        private CountdownTimer _jumpTimer;
        private CountdownTimer _jumpCooldownTimer;
        private CountdownTimer _dashTimer;
        private CountdownTimer _dashCooldownTimer;
        private CountdownTimer _interactTimer;

        private const float ZERO_F = 0f;
        
        // Animator Params
        private static readonly int _speedHash = Animator.StringToHash("Speed");
        
        public Vector3 StartPosition { get; private set; }

        private void Awake()
        {
            StartPosition = transform.position;
            _mainCamera = Camera.main;
            _virtualCamera.Follow = transform;
            _virtualCamera.LookAt = transform;
            _virtualCamera.OnTargetObjectWarped(transform, transform.position - _virtualCamera.transform.position - Vector3.forward);
            
            _rigidBody.freezeRotation = true;
            
            SetupTimers();
            SetupStateMachine();
        }

        private void SetupStateMachine()
        {
            // State Machine
            _stateMachine = new StateMachine();
            
            // Declare States
            LocomotionState _locomotionState = new LocomotionState(this, _animator);
            JumpState _jumpState = new JumpState(this, _animator);
            DashState _dashState = new DashState(this, _animator);
            InteractState _interactState = new InteractState(this, _animator);
            
            // Define transitions
            At(_locomotionState, _jumpState, new FuncPredicate(() => _jumpTimer.IsRunning));
            At(_locomotionState, _dashState, new FuncPredicate(() => _dashTimer.IsRunning));
            At(_locomotionState, _interactState, new FuncPredicate(() => _interactTimer.IsRunning));
            At(_interactState, _locomotionState, new FuncPredicate(() => !_interactTimer.IsRunning));
            Any(_locomotionState, new FuncPredicate(ReturnToLocomotionState));
            
            // Set initial state
            _stateMachine.SetState(_locomotionState);
        }

        private bool ReturnToLocomotionState()
        {
            return _groundChecker.IsGrounded 
                   && !_interactTimer.IsRunning 
                   && !_jumpTimer.IsRunning 
                   && !_dashTimer.IsRunning;
        }

        private void SetupTimers()
        {
            // Setup timers
            _jumpTimer = new CountdownTimer(_jumpDuration);
            _jumpCooldownTimer = new CountdownTimer(_jumpCooldown);
            

            _jumpTimer.OnTimerStart += () => _jumpVelocity = _jumpForce;
            _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();

            _dashTimer = new CountdownTimer(_dashDuration);
            _dashCooldownTimer = new CountdownTimer(_dashCooldown);
            _dashTimer.OnTimerStart += () => _dashVelocity = _dashForce;
            _dashTimer.OnTimerStop += () =>
            {
                _dashVelocity = 1f;
                _dashCooldownTimer.Start();
            };
            
            _interactTimer = new CountdownTimer(_interactCooldown);
                
            _timers = new List<Timer>(5) { _jumpTimer, _jumpCooldownTimer, _dashTimer, _dashCooldownTimer, _interactTimer };
        }

        private void OnEnable()
        {
            _input.JumpEvent += OnJump;
            _input.DashEvent += OnDash;
            _input.InteractEvent += OnInteract;
            _input.PickupEvent += OnPickup;
        }

        private void Start()
        {
            _input.EnablePlayerActions();
        }

        private void Update()
        {
            _movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
            _stateMachine.Update();

            HandleTimers();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        private void OnDisable()
        {
            _input.JumpEvent -= OnJump;
            _input.DashEvent -= OnDash;
            _input.InteractEvent -= OnInteract;
            _input.PickupEvent -= OnPickup;
            
            if (_input != null)
                _input.DisablePlayerActions();
        }

        private void At(IState from, IState to, IPredicate condition) =>
            _stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void HandleTimers()
        {
            foreach (Timer timer in _timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        
        private void OnInteract()
        {
            if (!_interactTimer.IsRunning)
                _interactTimer.Start();
        }

        public void Interact()
        {
            Collider[] hitColliders = new Collider[10];
            int hits = Physics.OverlapSphereNonAlloc(transform.position, _interactDistance, hitColliders, _interactableLayers);
            
            if (hits < 1)
                return;

            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;
            foreach (Collider hit in hitColliders)
            {
                if (hit == null)
                    break;
                
                if (!hit.gameObject.TryGetComponent(out IInteractable interactable))
                    continue;
                
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }

            closestInteractable?.Interact(_inventory);
        }

        private void OnJump(bool performed)
        {
            if (performed && !_jumpTimer.IsRunning && !_jumpCooldownTimer.IsRunning && _groundChecker.IsGrounded)
            {
                _jumpTimer.Start();
            }
            else if (!performed && _jumpTimer.IsRunning)
            {
                _jumpTimer.Stop();
            }
        }

        private void OnDash(bool performed)
        {
            if (performed && !_dashTimer.IsRunning && !_dashCooldownTimer.IsRunning)
            {
                _dashTimer.Start();
            }
            else if (!performed && _dashTimer.IsRunning)
            {
                _dashTimer.Stop();
            }
        }

        private void OnPickup()
        {
            _inventory.Pickup();
        }

        public void HandleJump()
        {
            if (!_jumpTimer.IsRunning && _groundChecker.IsGrounded)
            {
                _jumpVelocity = ZERO_F;
                _jumpTimer.Stop();
                return;
            }
            
            if (!_jumpTimer.IsRunning)
            {
                // Gravity takes over
                _jumpVelocity += Physics.gravity.y * _gravityMultiplier * Time.fixedDeltaTime;
            }
            
            // Apply velocity
            _rigidBody.linearVelocity = new Vector3(_rigidBody.linearVelocity.x, _jumpVelocity, _rigidBody.linearVelocity.z);
        }

        public void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            Vector3 adjustedDirection = Quaternion.AngleAxis(_mainCamera.transform.eulerAngles.y, Vector3.up) * _movement;

            if (adjustedDirection.magnitude > ZERO_F)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZERO_F);
                
                // Reset horizontal velocity for a snappy stop
                _rigidBody.linearVelocity = new Vector3(ZERO_F, _rigidBody.linearVelocity.y, ZERO_F);
            }
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement direction
            Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move the player
            Vector3 velocity = adjustedDirection * (_moveSpeed * _dashVelocity * Time.deltaTime);
            _rigidBody.linearVelocity = new Vector3(velocity.x, _rigidBody.linearVelocity.y, velocity.z);
        }

        private void SmoothSpeed(float value)
        {
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, _smoothTime);
        }

        private void UpdateAnimator()
        {
            _animator.SetFloat(_speedHash, _currentSpeed);
        }

        

        public void ResetState()
        {
            _rigidBody.transform.position = StartPosition;
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
    }
}