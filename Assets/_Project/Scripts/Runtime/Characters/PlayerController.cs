using System;
using System.Collections.Generic;
using KBCore.Refs;
using NJG.Runtime.Characters.Player.States;
using NJG.Runtime.Input;
using NJG.Runtime.Managers;
using NJG.Runtime.StateMachine;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace NJG.Runtime.Characters
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Self]
        private Rigidbody _rigidBody;
        [BoxGroup("References"), SerializeField, Self]
        private GroundChecker _groundChecker;
        [BoxGroup("References"), SerializeField, Anywhere]
        private Animator _animator;
        [BoxGroup("References"), SerializeField, Anywhere]
        private CinemachineCamera _virtualCamera;
        [BoxGroup("References"), SerializeField, Anywhere]
        private InputReader _input;
        
        [BoxGroup("Movement Settings"), SerializeField]
        private float _moveSpeed = 6f;
        [BoxGroup("Movement Settings"), SerializeField]
        private float _rotationSpeed = 15f;
        [BoxGroup("Movement Settings"), SerializeField]
        private float _smoothTime = 0.2f;

        [BoxGroup("Jump Settings"), SerializeField]
        private float _jumpForce = 10f;
        [BoxGroup("Jump Settings"), SerializeField]
        private float _jumpDuration = 0.5f;
        [BoxGroup("Jump Settings"), SerializeField]
        private float _jumpCooldown = 0f;
        [BoxGroup("Jump Settings"), SerializeField]
        private float _gravityMultiplier = 3f;
        
        private Camera _mainCamera;
        private StateMachine.StateMachine _stateMachine;

        private float _currentSpeed;
        private float _velocity;
        private float _jumpVelocity;
        private Vector3 _movement;
        private List<Timer> _timers;
        private CountdownTimer _jumpTimer;
        private CountdownTimer _jumpCooldownTimer;

        private const float ZERO_F = 0f;
        
        // Animator Params
        private static readonly int _speedHash = Animator.StringToHash("Speed");

        private void Awake()
        {
            _mainCamera = Camera.main;
            _virtualCamera.Follow = transform;
            _virtualCamera.LookAt = transform;
            _virtualCamera.OnTargetObjectWarped(transform, transform.position - _virtualCamera.transform.position - Vector3.forward);
            
            _rigidBody.freezeRotation = true;
            
            // Setup timers
            _jumpTimer = new CountdownTimer(_jumpDuration);
            _jumpCooldownTimer = new CountdownTimer(_jumpCooldown);
            _timers = new List<Timer>(2) { _jumpTimer, _jumpCooldownTimer };

            _jumpTimer.OnTimerStart += () => _jumpVelocity = _jumpForce;
            _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();
            
            // State Machine
            _stateMachine = new StateMachine.StateMachine();
            
            // Declare States
            LocomotionState _locomotionState = new LocomotionState(this, _animator);
            JumpState _jumpState = new JumpState(this, _animator);
            
            // Define transitions
            At(_locomotionState, _jumpState, new FuncPredicate(() => _jumpTimer.IsRunning));
            At(_jumpState, _locomotionState, new FuncPredicate(() => _groundChecker.IsGrounded && !_jumpTimer.IsRunning));
            
            // Set initial state
            _stateMachine.SetState(_locomotionState);
        }

        private void OnEnable()
        {
            _input.JumpEvent += OnJump;
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

            // Vector2 moveInput = _input.Direction;
            //
            // Vector3 worldMovement = new (moveInput.x, 0f, moveInput.y);
            // if (worldMovement.sqrMagnitude > ZERO_F)
            // {
            //     HandleRotation(worldMovement);
            //     HandleCharacterController(worldMovement.normalized);
            //     SmoothSpeed(worldMovement.magnitude);
            // }


            //Vector3 movementDirection = new Vector3(_input.Direction.x, 0f, _input.Direction.y).normalized;
            // Rotate movement direction to match camera rotation
            //Vector3 adjustedDirection = Quaternion.AngleAxis(_mainCamera.transform.eulerAngles.y, Vector3.up) * movementDirection;
            // Vector3 adjustedDirection = _mainCamera.transform.TransformDirection(movementDirection);
            // adjustedDirection.y = 0f;
            // adjustedDirection.Normalize();

            // if (adjustedDirection.magnitude > ZERO_F)
            // {
            //     HandleRotation(adjustedDirection);
            //     HandleCharacterController(adjustedDirection);
            //     SmoothSpeed(adjustedDirection.magnitude);
            // }
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
            Vector3 velocity = adjustedDirection * (_moveSpeed * Time.deltaTime);
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
    }
}