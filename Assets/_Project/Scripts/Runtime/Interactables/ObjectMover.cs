using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class ObjectMover : MonoBehaviour, IActivatable
    {
        [FoldoutGroup("Settings"), SerializeField]
        private Vector3 _direction = Vector3.up;
        [FoldoutGroup("Settings"), SerializeField, Range(0.01f, 100f)]
        private float _speed = 10f;
        [FoldoutGroup("Settings"), SerializeField, Range(0f, 100f)]
        private float _duration = 5f;
        [FoldoutGroup("Settings"), SerializeField, Range(0f, 100f)]
        private bool _destroyOnCompletion = true;

        private Tween _tween;
        
        [Button(ButtonSizes.Large)]
        public void Activate() => Move();

        [Button(ButtonSizes.Large)]
        public void Deactivate() => _tween?.Pause();

        private void Move()
        {
            if (_direction == Vector3.zero)
            {
                Debug.Log("[ObjectMover] Direction was 0,0,0, this will not work.. setting back to default.");
                _direction = Vector3.up;
            }
            
            _tween?.Kill();

            Vector3 worldDirection = transform.TransformDirection(_direction);
            float distance = worldDirection.magnitude;
            float time = distance / _speed;
            
            _tween = transform.DOMove(transform.position + _direction, time)
                                .SetEase(Ease.Linear)
                                .SetLoops(-1, LoopType.Incremental);
            
            //if (_destroyOnCompletion)
        }

        private void DestroySelf() => Destroy(gameObject);
    }
}