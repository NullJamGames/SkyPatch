using NJG.Runtime.Interactables;
using NJG.Runtime.Interfaces;
using UnityEngine;

namespace NJG
{
    public class TestBucket : MonoBehaviour, IPickupable, IResetable
    {
        [SerializeField]
        private GameObject _waterVisual;
        
        private Collider _collider;
        private Rigidbody _rigidbody;
        private MeshRenderer _renderer;
        
        public Transform Transform => transform;
        public Vector3 StartPosition { get; private set; }
        public bool HasWater { get; private set; }
        
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

        public bool TryFillWater()
        {
            if (HasWater)
                return false;

            HasWater = true;
            _waterVisual.SetActive(true);
            return true;
        }

        public bool TryEmptyWater()
        {
            if (!HasWater)
                return false;

            HasWater = false;
            _waterVisual.SetActive(false);
            return true;
        }

        public void ResetState()
        {
            transform.position = StartPosition + Vector3.up;
            _collider.enabled = true;
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
