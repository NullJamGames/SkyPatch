using System;
using DG.Tweening;
using KBCore.Refs;
using KinematicCharacterController;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public struct MovingPlatformState
    {
        public PhysicsMoverState MoverState;
    }
    
    public class MovingPlatform : BatteryPowered, IMoverController
    {
        [FoldoutGroup("References"), SerializeField, Self] 
        private PhysicsMover _mover;
        
        [FoldoutGroup("Settings"), SerializeField]
        private Transform[] _waypoints;
        [FoldoutGroup("Settings"), SerializeField]
        private float _platformSpeed = 1f;
        [FoldoutGroup("Settings"), SerializeField]
        [Tooltip("If true, the platform will return to the starting position when its deactivated.")]
        private bool _returnToStart = false;

        private Vector3[] _pathPoints;
        private float _totalPathLength;
        private float _pathTime;
        private float _elapsedTime;

        private Tween _tween;

        private bool _isMoving;
        private bool _isReturning;
        private bool _queuedActivation;

        private float _returnElapsed;
        private float _returnDuration;
        private Vector3 _returnStart;
        
        private const float _epsilon = 0.01f;
        
        public override bool IsActive => _isMoving;
        
        private void Start()
        {
            _mover.MoverController = this;
            SetupPath();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out PickupableItem pickupable))
            {
                pickupable.AttachTo(transform);
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.TryGetComponent(out PickupableItem pickupable))
            {
                pickupable.Unattach();
            }
        }

        private void SetupPath()
        {
            _pathPoints = new Vector3[_waypoints.Length];
            _totalPathLength = 0f;

            for (int i = 0; i < _waypoints.Length; i++)
            {
                _pathPoints[i] = _waypoints[i].position;

                if (i > 0)
                    _totalPathLength += Vector3.Distance(_pathPoints[i - 1], _pathPoints[i]);
            }

            _pathTime = _totalPathLength / _platformSpeed;

            _tween = transform.DOPath(_pathPoints, _pathTime, PathType.CatmullRom)
                              .SetOptions(true)
                              .SetEase(Ease.Linear)
                              .SetLoops(-1, LoopType.Yoyo)
                              .SetAutoKill(false)
                              .Pause();
        }

        public override void Activate()
        {
            if (_isReturning)
            {
                _queuedActivation = true;
                return;
            }

            _isMoving = true;
        }

        public override void Deactivate()
        {
            if (_returnToStart)
            {
                _isReturning = true;
                _isMoving = false;
                _elapsedTime = 0f;

                _returnStart = transform.position;
                _returnDuration = Vector3.Distance(_returnStart, _pathPoints[0]) / _platformSpeed;
                _returnElapsed = 0f;
            }
            else
            {
                _isMoving = false;
            }
        }

        // PhysicsMover calls this every FixedUpdate
        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            // Remember pose before animation
            Vector3 prePos = transform.position;
            Quaternion preRot = transform.rotation;

            // Evaluate next position
            EvaluateNextPosition(deltaTime);

            // Set our platform's goal pose to the animation's
            goalPosition = transform.position;
            goalRotation = transform.rotation;

            // Reset the actual transform pose to where it was before evaluating. 
            // This is so that the real movement can be handled by the physics mover; not the animation
            transform.position = prePos;
            transform.rotation = preRot;
        }

        private void EvaluateNextPosition(float deltaTime)
        {
            if (_isReturning)
            {
                _returnElapsed += deltaTime;

                float t = Mathf.Clamp01(_returnElapsed / _returnDuration);
                transform.position = Vector3.Lerp(_returnStart, _pathPoints[0], t);

                if (Vector3.Distance(transform.position, _pathPoints[0]) < _epsilon)
                {
                    transform.position = _pathPoints[0];
                    _isReturning = false;

                    if (_queuedActivation)
                    {
                        _queuedActivation = false;
                        _isMoving = true;
                    }
                }

                return;
            }

            if (!_isMoving || _tween == null)
                return;

            _elapsedTime += deltaTime;
            _tween.Goto(_elapsedTime, andPlay: false);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_waypoints == null || _waypoints.Length < 2)
                return;

            Gizmos.color = Color.blue;

            for (int i = 0; i < _waypoints.Length; i++)
            {
                if (_waypoints[i] == null)
                    continue;

                Gizmos.DrawSphere(_waypoints[i].position, 0.15f);
                
                if (i < _waypoints.Length - 1 && _waypoints[i + 1] != null)
                    Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
            }
        }
#endif

    }
}