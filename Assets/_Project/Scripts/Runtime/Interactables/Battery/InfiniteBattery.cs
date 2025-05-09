using NJG.Runtime.Entity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class InfiniteBattery : Battery
    {
        [FoldoutGroup("Test"), SerializeField, PropertyTooltip("attaches the battery at start (if not null)")]
        private BatteryInteractable _batteryInteractable;

        private void Start()
        {
            AddCharge(_maxCharge);
            TryAttachAtStart();
        }

        public override void RemoveCharge(float amount) { }

        private void TryAttachAtStart()
        {
            PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();
            _batteryInteractable?.TryInsertBattery(this, playerInventory);
        }
    }
}