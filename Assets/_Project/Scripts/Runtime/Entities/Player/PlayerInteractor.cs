using System;
using KBCore.Refs;
using ModelShark;
using NJG.Runtime.Entity;
using NJG.Runtime.Interactables;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Entities
{
    [RequireComponent(typeof(PlayerInventory))]
    public class PlayerInteractor : ValidatedMonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField, Self]
        private PlayerInventory _playerInventory;
        [FoldoutGroup("References"), SerializeField, Child]
        private TooltipTrigger _tooltip;
        
        [FoldoutGroup("Settings"), SerializeField]
        private float _interactDistance = 2f;
        [FoldoutGroup("Settings"), SerializeField]
        private LayerMask _interactableLayers;
        [FoldoutGroup("Settings"), SerializeField, Tooltip("We want to keep this number above 0 for performance.")]
        private float _interactableCheckInterval = 0.1f;
        [FoldoutGroup("Settings"), SerializeField, Tooltip("Maximum number of interactables that can be caught in radius at once.")]
        private int _maxInteractablesAtOnce = 10;

        private CountdownTimer _checkForInteractableTimer;
        private IInteractable _currentInteractable;
        
        private readonly Collider[] _hitColliders = new Collider[10];
        
        public bool HasInteractable => _currentInteractable != null;

        private void Awake()
        {
            _checkForInteractableTimer = new CountdownTimer(_interactableCheckInterval);
            _checkForInteractableTimer.OnTimerStop += OnCheckForInteractableTimerTick;
        }

        private void OnEnable()
        {
            TimerManager.RegisterTimer(_checkForInteractableTimer);
            _checkForInteractableTimer.Start();
        }

        private void OnDisable()
        {
            if (_checkForInteractableTimer == null)
                return;
            
            _checkForInteractableTimer.Stop();
            _checkForInteractableTimer.OnTimerStop -= OnCheckForInteractableTimerTick;
            TimerManager.DeregisterTimer(_checkForInteractableTimer);
        }

        private void OnDestroy() => UnregisterInteractable();

        public void Interact()
        {
            if (!HasInteractable)
                return;
            
            _currentInteractable.Interact(_playerInventory);
        }

        private void SetClosestInteractable()
        {
            Collider[] hitColliders = new Collider[_maxInteractablesAtOnce];
            int hits = Physics.OverlapSphereNonAlloc(transform.position, _interactDistance, hitColliders, _interactableLayers);
            
            if (hits < 1)
            {
                if (_currentInteractable == null)
                    return;
                
                UnregisterInteractable();
                _currentInteractable = null;
                OnInteractableChanged();
                return;
            }

            IPickupable closestPickupable = null;
            IInteractable closestInteractable = null;
            float closestPickupableDistance = float.MaxValue;
            float closestInteractableDistance = float.MaxValue;

            for (int i = 0; i < hits; i++)
            {
                Collider hit = hitColliders[i];
                if (hit == null)
                    break;
                
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (hit.TryGetComponent(out IPickupable pickupable))
                {
                    if (distance < closestPickupableDistance)
                    {
                        closestPickupableDistance = distance;
                        closestPickupable = pickupable;
                    }

                    continue;
                }

                if (hit.TryGetComponent(out IInteractable interactable))
                {
                    if (distance < closestInteractableDistance)
                    {
                        closestInteractableDistance = distance;
                        closestInteractable = interactable;
                    }
                }
            }
            
            if (closestPickupable != null)
            {
                if (_playerInventory.CanPickup() && IsSameInteractable(closestPickupable))
                    return;
                
                UnregisterInteractable();
                _currentInteractable = closestPickupable;
                RegisterInteractable();
                OnInteractableChanged();
            }
            else if (closestInteractable != null)
            {
                if (IsSameInteractable(closestInteractable))
                    return;
                
                UnregisterInteractable();
                _currentInteractable = closestInteractable;
                RegisterInteractable();
                OnInteractableChanged();
            }
        }

        private bool IsSameInteractable(IInteractable interactable)
        {
            return _currentInteractable?.Transform == interactable?.Transform;
        }

        private void RegisterInteractable()
        {
            if (_currentInteractable == null)
                return;
            
            _currentInteractable.OnTooltipTextChanged += OnUpdateTooltip;
        }

        private void UnregisterInteractable()
        {
            if (_currentInteractable == null)
                return;
            
            _currentInteractable.OnTooltipTextChanged -= OnUpdateTooltip;
        }

        private void OnInteractableChanged()
        {
            if (_currentInteractable == null)
                HideTooltip();
            else
                UpdateTooltipText(_currentInteractable.GetTooltipText(_playerInventory));
        }

        private void OnUpdateTooltip(string text)
        {
            UpdateTooltipText(text);
        }
        
        private void OnCheckForInteractableTimerTick()
        {
            SetClosestInteractable();
            _checkForInteractableTimer.Start();
        }
        
        private void UpdateTooltipText(string text)
        {
            // Reset tooltip just in case.
            _tooltip.ForceHideTooltip();
            
            _tooltip.SetText("BodyText", text);
            // Popup the tooltip (Note: duration doesn't matter, since StaysOpen is True)
            _tooltip.Popup(1f, gameObject);
        }

        private void HideTooltip() => _tooltip.ForceHideTooltip();
    }
}