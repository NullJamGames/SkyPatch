using System;
using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public abstract class BatteryInteractable : MonoBehaviour, IInteractable, IGivableInteractable, IBatteryReceiver
    {
        [FoldoutGroup("References"), SerializeField]
        private Transform _batteryHolder;

        [FoldoutGroup("General"), SerializeField]
        private string _name = "NAME";

        protected Battery _battery;

        public event Action<string> OnTooltipTextChanged;

        public bool HasBattery => _battery != null;
        public Transform Transform => transform;

        public virtual bool TryInsertBattery(Battery battery, PlayerInventory playerInventory)
        {
            if (_battery != null)
                return false;

            playerInventory.DetachPickupable();
            _battery = battery;
            _battery.Transform.SetParent(_batteryHolder);
            _battery.Transform.position = _batteryHolder.position;
            _battery.Transform.rotation = _batteryHolder.rotation;
            OnBatteryInserted();
            OnTooltipTextChanged?.Invoke(GetTooltipText(playerInventory));

            return true;
        }

        public virtual bool TryGivePickupable(PlayerInventory playerInventory)
        {
            if (_battery == null || !playerInventory.CanPickup())
                return false;

            playerInventory.AttachPickupable(_battery);
            _battery = null;
            OnBatteryRemoved();
            OnTooltipTextChanged?.Invoke(GetTooltipText(playerInventory));

            return true;
        }

        public virtual void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }

        public virtual string GetTooltipText(PlayerInventory playerInventory)
        {
            string tooltipText = InteractionHelper.GetBatteryInteractableTooltip(playerInventory, this);
            return $"{_name}\n{tooltipText}";
        }

        protected abstract void OnBatteryInserted();

        protected abstract void OnBatteryRemoved();
    }
}