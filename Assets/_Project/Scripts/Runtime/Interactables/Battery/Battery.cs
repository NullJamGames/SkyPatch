using Mono.CSharp;
using NJG.Runtime.Audio;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Interactables
{
    public class Battery : PickupableItem, IInteractablePickupable
    {
        [FoldoutGroup("References"), SerializeField]
        private Material _emptyMaterial;
        [FoldoutGroup("References"), SerializeField]
        private Material _notFullMaterial;
        [FoldoutGroup("References"), SerializeField]
        private Material _fullMaterial;
        
        [field: SerializeField, ReadOnly]
        public float CurrentCharge { get; private set; } = 0f;

        protected const float _maxCharge = 100f;



        public void InteractWith(IInteractable interactable, PlayerInventory playerInventory)
        {
            if (interactable is IBatteryReceiver batteryReceiver)
                batteryReceiver.TryInsertBattery(this, playerInventory);
        }

        public void AddCharge(float amount)
        {
            CurrentCharge += amount;
            CurrentCharge = Mathf.Clamp(CurrentCharge, 0f, _maxCharge);
            UpdateMaterial();
        }
        
        public virtual void RemoveCharge(float amount)
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
    }
}
