using System;
using System.Collections.Generic;
using DG.Tweening;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Switch : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("Arm"), SerializeField]
        private Transform _arm;
        [FoldoutGroup("Arm"), SerializeField]
        private float _xRotation = 30;
        [FoldoutGroup("Arm"), SerializeField]
        private float _rotateTime = 0.3f;
        [FoldoutGroup("Arm"), SerializeField]
        private Ease _ease = Ease.InOutQuad;

        [FoldoutGroup("Activatables"), SerializeField]
        private List<ActivatableField> _activatables = new();
        [FoldoutGroup("Activatables/Reverse"), SerializeField]
        private List<ActivatableField> _reverseActivatables = new();

        private bool _isActive;
        private Tween _tween;

        public Transform Transform => transform;
        public event Action<string> OnTooltipTextChanged;

        private void Start()
        {
            _arm.localRotation = Quaternion.Euler(-_xRotation, 0, 0);

            foreach (ActivatableField reverseActivatable in _reverseActivatables)
                reverseActivatable.Activatable?.Activate();
        }

        public void Interact(PlayerInventory playerInventory)
        {
            if (!_isActive)
                Activate();
            else
                Deactivate();
        }

        public string GetTooltipText(PlayerInventory playerInventory) => "[SWITCH]\nPress E to pull";

        protected virtual void Activate()
        {
            _isActive = true;
            RotateArm(_xRotation);
            foreach (ActivatableField activatable in _activatables)
                activatable.Activatable?.Activate();
            foreach (ActivatableField reverseActivatable in _reverseActivatables)
                reverseActivatable.Activatable?.Deactivate();
        }

        protected virtual void Deactivate()
        {
            _isActive = false;
            RotateArm(-_xRotation);
            foreach (ActivatableField activatable in _activatables)
                activatable.Activatable?.Deactivate();
            foreach (ActivatableField reverseActivatable in _reverseActivatables)
                reverseActivatable.Activatable?.Activate();
        }

        private void RotateArm(float desiredRot)
        {
            _tween?.Kill();
            _tween = _arm.DOLocalRotate(new Vector3(desiredRot, 0f, 0f), _rotateTime).SetEase(_ease);
        }
    }
}