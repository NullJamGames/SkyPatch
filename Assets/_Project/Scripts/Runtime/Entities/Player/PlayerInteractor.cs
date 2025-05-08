using System;
using KBCore.Refs;
using ModelShark;
using NJG.Runtime.Entity;
using NJG.Runtime.Interactables;
using NJG.Runtime.UI.Tooltips;
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
        private ITooltipProvider _nonInteractableTooltipProvider;
        private bool _isRegisteredNonInteractableTooltip;
        private string _currentTooltipText;
        
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
                if (_currentInteractable == null && _nonInteractableTooltipProvider == null)
                    return;
                
                UnregisterInteractable();
                UnRegisterNonInteractableTooltip();
                _currentInteractable = null;
                _nonInteractableTooltipProvider = null;
                UpdateToolTip();
                return;
            }

            IPickupable closestPickupable = null;
            IInteractable closestInteractable = null;
            ITooltipProvider closestNonInteractableTooltipProvider = null;
            float closestPickupableDistance = float.MaxValue;
            float closestInteractableDistance = float.MaxValue;
            float closestNonInteractableTooltipDistance = float.MaxValue;

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
                    
                    continue;
                }

                if (hit.TryGetComponent(out ITooltipProvider tooltipProvider))
                {
                    if (distance < closestNonInteractableTooltipDistance)
                    {
                        closestNonInteractableTooltipDistance = distance;
                        closestNonInteractableTooltipProvider = tooltipProvider;
                    }
                }
            }
            
            if (closestPickupable != null)
            {
                if (IsSameInteractable(closestPickupable))
                    return;

                if (_playerInventory.CanPickup())
                {
                    UnregisterInteractable();
                    UnRegisterNonInteractableTooltip();
                    _currentInteractable = closestPickupable;
                    RegisterInteractable();
                    UpdateToolTip();
                    return;
                }
            }

            if (closestInteractable == null)
            {
                _currentInteractable = null;
            }
            else
            {
                if (IsSameInteractable(closestInteractable))
                    return;
                
                UnregisterInteractable();
                UnRegisterNonInteractableTooltip();
                _currentInteractable = closestInteractable;
                RegisterInteractable();
                UpdateToolTip();
                return;
            }

            if (closestNonInteractableTooltipProvider == null)
            {
                UnRegisterNonInteractableTooltip();
                _nonInteractableTooltipProvider = null;
                HideTooltip();
            }
            else
            {
                UnRegisterNonInteractableTooltip();
                _nonInteractableTooltipProvider = closestNonInteractableTooltipProvider;
                RegisterNonInteractableTooltip();
                UpdateToolTip();
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

        private void RegisterNonInteractableTooltip()
        {
            if(_isRegisteredNonInteractableTooltip)
                return;
            if (_nonInteractableTooltipProvider == null)
                return;
            
            _isRegisteredNonInteractableTooltip = true;
            _nonInteractableTooltipProvider.OnTooltipTextChanged += OnUpdateTooltip;
        }

        private void UnRegisterNonInteractableTooltip()
        {
            if (_nonInteractableTooltipProvider != null)
                _nonInteractableTooltipProvider.OnTooltipTextChanged -= OnUpdateTooltip;
            _isRegisteredNonInteractableTooltip = false;
        }


        private void UpdateToolTip()
        {
            if (_currentInteractable != null)
                UpdateTooltipText(_currentInteractable.GetTooltipText(_playerInventory));
            else if (_nonInteractableTooltipProvider != null)
                UpdateTooltipText(_nonInteractableTooltipProvider.GetTooltipText(_playerInventory));
            else
                HideTooltip();
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
            if(_currentTooltipText == text)
                return;
            _currentTooltipText = text;
            // Reset tooltip just in case.
            _tooltip.ForceHideTooltip();
            
            _tooltip.SetText("BodyText", text);
            // Popup the tooltip (Note: duration doesn't matter, since StaysOpen is True)
            _tooltip.Popup(1f, gameObject);
        }

        private void HideTooltip()
        {
            _currentTooltipText = "";
            _tooltip.ForceHideTooltip();
        } 
            
    }
}