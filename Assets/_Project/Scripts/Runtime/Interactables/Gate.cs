using System;
using DG.Tweening;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Gate : BatteryPowered, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private Transform _closedPoint;
        [FoldoutGroup("References"), SerializeField]
        private Transform _openPoint;

        [FoldoutGroup("Settings"), SerializeField]
        private float _speed = 1f;
        
        private Vector3 _closedPosition;
        private Vector3 _openPosition;
        private Tween _currentTween;
        private bool _isActive;
        
        public event Action<string> OnTooltipTextChanged;
        public Transform Transform => transform;

        private void Start()
        {
            _closedPosition = _closedPoint.position;
            _openPosition = _openPoint.position;
        }

        public string GetTooltipText(PlayerInventory playerInventory) => "GATE";

        public void Interact(PlayerInventory playerInventory) { }

        public override bool IsActive => _isActive;

        public override void Activate() => OpenGate();

        public override void Deactivate() => CloseGate();

        private void OpenGate()
        {
            if (_currentTween != null && _currentTween.IsActive())
                _currentTween.Kill();
            
            _isActive = true;
            float distance = Vector3.Distance(transform.position, _openPosition);
            _currentTween = transform.DOMove(_openPosition, distance / _speed)
                .SetEase(Ease.OutSine)
                .OnComplete(OnGateOpened);
        }

        private void OnGateOpened()
        {
            _currentTween = null;
            _isActive = false;
        }

        private void CloseGate()
        {
            if (_currentTween != null && _currentTween.IsActive())
                _currentTween.Kill();

            _isActive = true;
            float distance = Vector3.Distance(transform.position, _closedPosition);
            _currentTween = transform.DOMove(_closedPosition, distance / _speed)
                .SetEase(Ease.OutSine)
                .OnComplete(OnGateClosed);
        }

        private void OnGateClosed()
        {
            _currentTween = null;
            _isActive = false;
        }
    }
}