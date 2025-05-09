using System;
using KBCore.Refs;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class WaterReservoir : MonoBehaviour, IInteractable, IWaterSource
    {
        [FoldoutGroup("References"), SerializeField, Anywhere]
        protected GameObject _waterVisual;

        [FoldoutGroup("Settings"), SerializeField]
        private bool _unlimitedWater = true;
        [FoldoutGroup("Settings"), SerializeField, HideIf(nameof(_unlimitedWater))]
        private int _numberOfUses = 3;

        private Collider _collider;
        private int _remainingUses;

        public bool ContainsWater { get; protected set; } = true;

        public Transform Transform => transform;

        public event Action<string> OnTooltipTextChanged;

        private void Awake()
        {
            _collider = GetComponent<Collider>();

            _remainingUses = _numberOfUses;
            if (!_unlimitedWater && _remainingUses <= 0)
                SetNoWater();
        }

        public void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
            OnTooltipTextChanged?.Invoke(GetTooltipText(playerInventory));
        }

        public string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetWaterSourceTooltip(playerInventory);
            return $"WATER SOURCE\n{tooltipText}";
        }

        public void FillWater(WaterContainer waterContainer)
        {
            if (!ContainsWater)
                return;

            if (waterContainer.TryFillWater())
                _remainingUses--;

            if (!_unlimitedWater && _remainingUses <= 0)
                SetNoWater();
        }

        public void SetUnlimitedWater()
        {
            _unlimitedWater = true;
            ContainsWater = true;
            _waterVisual.SetActive(true);
            _collider.enabled = true;
        }

        public void SetNoWater()
        {
            _unlimitedWater = false;
            ContainsWater = false;
            _waterVisual.SetActive(false);
            _collider.enabled = false;
        }
    }
}