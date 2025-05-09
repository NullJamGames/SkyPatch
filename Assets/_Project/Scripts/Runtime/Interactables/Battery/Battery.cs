using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class Battery : PickupableItem, IInteractablePickupable
    {
        [FoldoutGroup("Shader Setup"), SerializeField]
        private int _materialIndex = 3;
        [FoldoutGroup("Shader Setup"), SerializeField]
        private string _chargeSliderRef = "_ChargeSlider";
        [FoldoutGroup("Shader Setup"), SerializeField]
        private string _chargeColorRef = "_ChargedColor";
        [FoldoutGroup("Shader Setup"), SerializeField]
        private Color _chargingColor = Color.yellow;
        [FoldoutGroup("Shader Setup"), SerializeField]
        private Color _chargedColor = Color.green;

        [FoldoutGroup("VFX"), SerializeField]
        private GameObject _particleEffect;
        
        [field: SerializeField, ReadOnly]
        public float CurrentCharge { get; private set; }
        
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
            UpdateShader();
        }

        public virtual void RemoveCharge(float amount)
        {
            CurrentCharge -= amount;
            CurrentCharge = Mathf.Clamp(CurrentCharge, 0f, _maxCharge);
            UpdateShader();
        }

        public void OnBatteryPlaced()
        {
            if(_particleEffect)
                Instantiate(_particleEffect, transform.position, Quaternion.identity);
        }

        private void UpdateShader()
        {
            if (_renderer.materials.Length <= _materialIndex)
                return;
            
            float convertedCharge = CurrentCharge / _maxCharge;
            _renderer.materials[_materialIndex]?.SetFloat(_chargeSliderRef, convertedCharge);
            _renderer.materials[_materialIndex]?.SetColor(_chargeColorRef, CurrentCharge < _maxCharge ? _chargingColor : _chargedColor);
        }
    }
}