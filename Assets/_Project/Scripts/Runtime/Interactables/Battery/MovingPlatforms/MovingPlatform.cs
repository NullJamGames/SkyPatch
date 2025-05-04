using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class MovingPlatform : MonoBehaviour, IActivatable
    {
        [FoldoutGroup("Settings"), SerializeField]
        private Vector3 _moveTo = Vector3.zero;
        [FoldoutGroup("Settings"), SerializeField]
        private float _moveTime = 1f;
        [FoldoutGroup("Settings"), SerializeField]
        private Ease _ease = Ease.InOutQuad;
        
        public bool IsWorking { get;  private set; }
        
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        
        private Vector3 _targetPosition;

        private Vector3 _currentPosition;
        private Vector3 _previousPosition;
        
        private CoroutineHandle _moveCoroutine;

        private bool _hasBattery;

        private int _obstacleCount;
        

        private void Start()
        {
            _startPosition = transform.position;
            _endPosition = transform.position + _moveTo;
            
            transform.position = _startPosition;
            _targetPosition = _endPosition;
        }

        private void Update()
        {
            _previousPosition = _currentPosition;
            _currentPosition = transform.position;
        }

        public void Activate()
        {
            _hasBattery = true;
            TryStartMovement();
        }
        
        public void Deactivate()
        {
            _hasBattery = false;
            StopMovement();
        }

        public void AddObstacle()
        {
            _obstacleCount++;
            StopMovement();
        }

        public void RemoveObstacle()
        {
            _obstacleCount--;
            TryStartMovement();
        }

        private IEnumerator<float> MoveToTargetPosition(float moveTime)
        {
            Vector3 startPosition = transform.position;
            float elapsed = 0f;

            while (elapsed < moveTime)
            {
                float t = elapsed / moveTime; 
                float easedT = DOVirtual.EasedValue(0f, 1f, t, _ease); 
                
                transform.position = Vector3.Lerp(startPosition, _targetPosition, easedT); 

                elapsed += Timing.DeltaTime; 
                yield return Timing.WaitForOneFrame; 
            }

            transform.position = _targetPosition;
            
            ChangeDirection();
            StartMovement();
        }

        private void ChangeDirection()
        {
            if(_targetPosition == _startPosition)
                _targetPosition = _endPosition;
            else 
                _targetPosition = _startPosition;
        }
        
        
        private void StopMovement()
        {
            if (_moveCoroutine.IsValid)
            {
                Timing.KillCoroutines(_moveCoroutine);
                _moveCoroutine = default;
            }
            IsWorking = false;
        }

        private void TryStartMovement()
        {
            if(_hasBattery && _obstacleCount == 0)
                StartMovement();
        }

        private void StartMovement()
        {
            if(_moveCoroutine.IsValid)
                Timing.KillCoroutines(_moveCoroutine);
            
            _moveCoroutine = Timing.RunCoroutine(MoveToTargetPosition(_moveTime));
            IsWorking = true;
        }
        

        public Vector3 GetVelocity()
        {
            return (_currentPosition - _previousPosition) / Time.deltaTime;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _moveTo);
            Gizmos.DrawSphere(transform.position + _moveTo, 0.1f);
        }
        #endif
    }
}