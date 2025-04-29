using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class MovingPlatform : MonoBehaviour
    {
        [FoldoutGroup("Settings"), SerializeField]
        private Vector3 _moveTo = Vector3.zero;
        [FoldoutGroup("Settings"), SerializeField]
        private float _moveTime = 1f;
        [FoldoutGroup("Settings"), SerializeField]
        private Ease _ease = Ease.InOutQuad;
        
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private Vector3 _currentPosition;
        private Vector3 _previousPosition;

        private void Start()
        {
            _startPosition = transform.position;
            _endPosition = transform.position + _moveTo;
        }

        private void Update()
        {
            _previousPosition = _currentPosition;
            _currentPosition = transform.position;
        }

        public void Activate()
        {
            Initialize();
        }
        
        public void Deactivate()
        {
            transform.DOKill();
        }

        private void Initialize()
        {
            if (transform.position != _startPosition)
            {
                transform.DOMove(_startPosition, _moveTime).OnComplete(() =>
                {
                    transform.position = _startPosition;
                    Move();
                });
                return;
            }

            Move();
        }

        private void Move()
        {
            transform.DOMove(_endPosition, _moveTime)
                     .SetEase(_ease)
                     .SetLoops(-1, LoopType.Yoyo);
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