﻿using System;
using KBCore.Refs;
using NJG.Runtime.Entity;
using NJG.Runtime.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public abstract class PickupableItem : ValidatedMonoBehaviour, IPickupable, IResetable
    {
        [FoldoutGroup("References"), SerializeField, Anywhere]
        protected MeshRenderer _renderer;

        [FoldoutGroup("General"), SerializeField]
        protected string _name = "NAME";

        protected Collider _collider;
        protected Rigidbody _rigidbody;

        public Transform Transform => transform;
        public Vector3 StartPosition { get; private set; }
        public Quaternion StartRotation { get; private set; }
        public bool IsPickedUp { get; private set; }

        public event Action<string> OnTooltipTextChanged;

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
            _rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        public virtual void OnDrop()
        {
            IsPickedUp = false;
            _collider.enabled = true;
            _rigidbody.isKinematic = false;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetPickupableTooltip(playerInventory, this);
            return $"{_name}\n{tooltipText}";
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

        public virtual void AttachTo(Transform t)
        {
            transform.SetParent(t);
            _rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        public virtual void Unattach()
        {
            if (IsPickedUp)
                return;

            transform.SetParent(null);
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}