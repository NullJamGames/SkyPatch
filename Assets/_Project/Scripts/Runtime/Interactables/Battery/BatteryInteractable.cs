using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public abstract class BatteryInteractable : MonoBehaviour, IInteractable, IGivableInteractable, IBatteryReceiver
    {
        [FoldoutGroup("References"), SerializeField]
        private Transform _batteryHolder;
        
        protected Battery _battery;
        
        public virtual void Interact(PlayerInventory playerInventory)
        {
            InteractionHelper.TryInteract(playerInventory, this);
        }
        
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
            
            return true;
        }
        
        public virtual bool TryGivePickupable(PlayerInventory playerInventory)
        {
            if (_battery == null || !playerInventory.CanPickup())
                return false;
            
            playerInventory.AttachPickupable(_battery);
            _battery = null;
            OnBatteryRemoved();
            
            return true;
        }

        protected abstract void OnBatteryInserted();
        protected abstract void OnBatteryRemoved();
    }
}