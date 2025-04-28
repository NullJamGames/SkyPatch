using System;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NJG.Runtime.Interactables
{
    public class Switch : MonoBehaviour, IInteractable
    {
        [BoxGroup("Arm"), SerializeField]
        private Transform _arm;
        
        [SerializeField]
        public UnityEvent OnSwitchActivated;

        private bool _isActive;
        private Vector3 _offPosition = new(-54f, 0f, 0f);
        private Vector3 _onPosition = new(54f, 0f, 0f);
        
        public Transform Transform => transform;
        
        public event Action<string> OnTooltipTextChanged;
        
        public void Interact(PlayerInventory playerInventory)
        {
            if (_isActive)
                return;
            
            _isActive = true;
            _arm.localPosition = _onPosition;
            OnSwitchActivated?.Invoke();
        }

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            return "[Switch]\nTOOLTIP NOT IMPLEMENTED";
        }
    }
}
