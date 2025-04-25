using System;
using NJG.Runtime.Interactables;
using NJG.Runtime.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG
{
    public class TestBattery : MonoBehaviour, IPickupable, IResetable
    {
        [FoldoutGroup("References"), SerializeField]
        private Material _emptyMaterial;
        [FoldoutGroup("References"), SerializeField]
        private Material _notFullMaterial;
        [FoldoutGroup("References"), SerializeField]
        private Material _fullMaterial;
        
        [field: SerializeField, ReadOnly]
        public float CurrentCharge { get; private set; } = 0f;

        private const float _maxCharge = 100f;
        private Collider _collider;
        private Rigidbody _rigidbody;
        private MeshRenderer _renderer;
        
        public Transform Transform => transform;
        public Vector3 StartPosition { get; private set; }
        
        private void Awake()
        {
            StartPosition = transform.position;
            
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _renderer = GetComponent<MeshRenderer>();
        }

        public void OnPickup()
        {
            _collider.enabled = false;
            _rigidbody.isKinematic = true;
        }

        public void OnDrop()
        {
            _collider.enabled = true;
            _rigidbody.isKinematic = false;
        }

        public void AddCharge(float amount)
        {
            CurrentCharge += amount;
            CurrentCharge = Mathf.Clamp(CurrentCharge, 0f, _maxCharge);
            UpdateMaterial();
        }
        
        public void RemoveCharge(float amount)
        {
            CurrentCharge -= amount;
            CurrentCharge = Mathf.Clamp(CurrentCharge, 0f, _maxCharge);
            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            _renderer.material = CurrentCharge switch
            {
                <= 0f => _emptyMaterial,
                < _maxCharge => _notFullMaterial,
                _ => _fullMaterial
            };
        }

        public void ResetState()
        {
            transform.position = StartPosition;
            _collider.enabled = true;
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
