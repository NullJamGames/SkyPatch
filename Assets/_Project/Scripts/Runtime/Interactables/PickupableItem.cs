using System;
using KBCore.Refs;
using NJG.Runtime.Entity;
using NJG.Runtime.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public abstract class PickupableItem : MonoBehaviour, IPickupable, IResetable
    {
        [FoldoutGroup("References"), SerializeField, Anywhere]
        protected MeshRenderer _renderer;
        
        protected Collider _collider;
        protected Rigidbody _rigidbody;
        
        public Transform Transform => transform;
        public Vector3 StartPosition { get; private set; }
        public Quaternion StartRotation { get; private set; }
        public bool IsPickedUp { get; private set; }

        public virtual void Awake()
        {
            StartPosition = transform.position;
            StartRotation = transform.rotation;
            
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public virtual void Interact(PlayerInventory playerInventory)
        {
            playerInventory.PickUp(this);
        }

        public virtual void OnPickup()
        {
            IsPickedUp = true;
            _collider.enabled = false;
            _rigidbody.isKinematic = true;
        }

        public virtual void OnDrop()
        {
            IsPickedUp = false;
            _collider.enabled = true;
            _rigidbody.isKinematic = false;
        }

        public virtual void ResetState()
        {
            transform.position = StartPosition + Vector3.up;
            transform.rotation = StartRotation;
            _collider.enabled = true;
            _rigidbody.isKinematic = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}