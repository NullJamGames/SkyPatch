using NJG.Runtime.Entity;
using NJG.Runtime.Interactables;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG
{
    public class TestLiftPanel : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private Transform _insertSlot;
        [FoldoutGroup("References"), SerializeField]
        private PlatformMover _movablePlatform;
        
        [FoldoutGroup("Settings"), SerializeField]
        private float _drainPerInterval = 1f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _drainInterval = 1f;
        
        private TestBattery _battery;
        private CountdownTimer _drainTimer;

        private void Awake()
        {
            _drainTimer = new CountdownTimer(_drainInterval);
            _drainTimer.OnTimerStop += () =>
            {
                if (_battery == null)
                    return;

                _battery.RemoveCharge(_drainPerInterval);
                if (_battery.CurrentCharge <= 0f)
                {
                    _movablePlatform.Deactivate();
                    return;
                }
                
                _drainTimer.Start();
            };
        }
        
        private void Update()
        {
            _drainTimer?.Tick(Time.deltaTime);
        }

        public void Interact(PlayerInventory playerInventory)
        {
            if (_battery != null)
            {
                if (playerInventory.TryGivePickupable(_battery))
                {
                    _movablePlatform.Deactivate();
                    _battery = null;
                    _drainTimer.Stop();
                }

                return;
            }
            
            if (playerInventory.Pickupable is null)
                return;

            if (playerInventory.Pickupable is TestBattery battery)
            {
                if (playerInventory.TryGetPickupable(_insertSlot))
                {
                    _battery = battery;

                    if (_battery.CurrentCharge > 0f)
                    {
                        _movablePlatform.Activate();
                        _drainTimer.Start();
                    }
                }
            }
        }
    }
}
