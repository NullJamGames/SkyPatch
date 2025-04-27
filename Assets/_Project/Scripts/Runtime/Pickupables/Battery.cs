using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Pickupables
{
    public class Battery : PickupableItem
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
    }
}
