using NJG.Runtime.Interactables;
using NJG.Runtime.Interfaces;
using UnityEngine;

namespace NJG
{
    public class TestHarvestedPlant : MonoBehaviour, IPickupable, IResetable
    {
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
