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
        private bool _returnToStartOnDeactivate = false;
        [FoldoutGroup("Settings"), SerializeField]
        private bool _infiniteLoops = true;
        [FoldoutGroup("Settings"), SerializeField, HideIf(nameof(_infiniteLoops))]
        private int _loops = 3;
        [FoldoutGroup("Settings"), SerializeField]
        private float _stopDuration = 1f;

        private Vector3[] _pathPoints;
        private float[] _segmentDurations;

        private bool _isMoving;
        private bool _isWaiting;
        private bool _isReturning;
        private bool _queuedActivation;
        private bool _loopingBackToStart;

        private float _waitTimer;
        private float _elapsedSegmentTime;
        private int _currentSegmentIndex;
        private int _completedLoops;

        private Vector3 _returnStart;
        private float _returnElapsed;
        private float _returnDuration;

        private const float _epsilon = 0.01f;

        public override bool IsActive => _isMoving;

        private void Start()
        {
            _mover.MoverController = this;
            SetupPath();
        }

        private void SetupPath()
        {
            if (_waypoints == null || _waypoints.Length < 2)
                throw new Exception("MovingPlatform requires at least 2 waypoints.");

            _pathPoints = new Vector3[_waypoints.Length];
            _segmentDurations = new float[_waypoints.Length];

            for (int i = 0; i < _waypoints.Length; i++)
                _pathPoints[i] = _waypoints[i].position;

            for (int i = 0; i < _waypoints.Length - 1; i++)
            {
                float dist = Vector3.Distance(_pathPoints[i], _pathPoints[i + 1]);
                _segmentDurations[i] = dist / _platformSpeed;
            }

            float returnDist = Vector3.Distance(_pathPoints[^1], _pathPoints[0]);
            _segmentDurations[^1] = returnDist / _platformSpeed;

            ResetMovement();
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
            if (_returnToStartOnDeactivate)
            {
                _isReturning = true;
                _isMoving = false;
                _returnStart = transform.position;
                _returnElapsed = 0f;
                _returnDuration = Vector3.Distance(_returnStart, _pathPoints[0]) / _platformSpeed;
            }
            else
            {
                _isMoving = false;
            }
        }

        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            Vector3 prePos = transform.position;
            Quaternion preRot = transform.rotation;

            EvaluateNextPosition(deltaTime);

            goalPosition = transform.position;
            goalRotation = transform.rotation;

            transform.position = prePos;
            transform.rotation = preRot;
        }

        private void EvaluateNextPosition(float deltaTime)
        {
            if (_isReturning)
            {
                HandleReturnToStart(deltaTime);
                return;
            }

            if (!_isMoving || _waypoints.Length < 2)
                return;

            if (_isWaiting)
            {
                HandleWaitAtWaypoint(deltaTime);
                return;
            }

            if (_currentSegmentIndex >= _pathPoints.Length - 1 && !_loopingBackToStart)
            {
                if (_infiniteLoops || _completedLoops < _loops - 1)
                {
                    _loopingBackToStart = true;
                    _elapsedSegmentTime = 0f;
                }
                else
                {
                    _isMoving = false;
                    return;
                }
            }

            Vector3 from = _loopingBackToStart ? _pathPoints[^1] : _pathPoints[_currentSegmentIndex];
            Vector3 to = _loopingBackToStart ? _pathPoints[0] : _pathPoints[_currentSegmentIndex + 1];
            float duration = _loopingBackToStart ? _segmentDurations[^1] : _segmentDurations[_currentSegmentIndex];

            _elapsedSegmentTime += deltaTime;
            float t = Mathf.Clamp01(_elapsedSegmentTime / duration);
            transform.position = Vector3.Lerp(from, to, t);

            if (t >= 1f)
            {
                _isWaiting = true;
                _waitTimer = 0f;

                if (_loopingBackToStart)
                {
                    _loopingBackToStart = false;
                    _completedLoops++;
                    _currentSegmentIndex = 0;
                }
                else
                {
                    _currentSegmentIndex++;
                }
            }
        }

        private void HandleReturnToStart(float deltaTime)
        {
            _returnElapsed += deltaTime;
            float t = Mathf.Clamp01(_returnElapsed / _returnDuration);
            transform.position = Vector3.Lerp(_returnStart, _pathPoints[0], t);

            if (Vector3.Distance(transform.position, _pathPoints[0]) < _epsilon)
            {
                _isReturning = false;
                ResetMovement();

                if (_queuedActivation)
                {
                    _queuedActivation = false;
                    _isMoving = true;
                }
            }
        }

        private void HandleWaitAtWaypoint(float deltaTime)
        {
            _waitTimer += deltaTime;
            if (_waitTimer >= _stopDuration)
            {
                _waitTimer = 0f;
                _isWaiting = false;
                _elapsedSegmentTime = 0f;
            }
        }

        private void ResetMovement()
        {
            _currentSegmentIndex = 0;
            _elapsedSegmentTime = 0f;
            _completedLoops = 0;
            _loopingBackToStart = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_waypoints == null || _waypoints.Length < 2)
                return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < _waypoints.Length; i++)
            {
                if (_waypoints[i] != null)
                    Gizmos.DrawSphere(_waypoints[i].position, 0.1f);

                if (i < _waypoints.Length - 1 && _waypoints[i] != null && _waypoints[i + 1] != null)
                    Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
            }

            // Loop line from end to start
            if (_infiniteLoops && _waypoints[^1] != null && _waypoints[0] != null)
                Gizmos.DrawLine(_waypoints[^1].position, _waypoints[0].position);
        }
#endif
    }
}
