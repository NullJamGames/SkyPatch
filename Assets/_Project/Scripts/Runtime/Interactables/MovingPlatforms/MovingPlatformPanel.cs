using System;
using NJG.Runtime.Entity;
using NJG.Runtime.Pickupables;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class MovingPlatformPanel : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private Transform _insertSlot;
        [FoldoutGroup("References"), SerializeField]
        private MovingPlatform _movableMovingPlatform;
        
        [FoldoutGroup("Settings"), SerializeField]
        private float _drainPerInterval = 1f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _drainInterval = 1f;
        
        private Battery _battery;
        private CountdownTimer _drainTimer;

        private void Awake()
        {
            _drainTimer = new CountdownTimer(_drainInterval);
            _drainTimer.OnTimerStop += OnDrain;
        }

        private void OnEnable() => TimerManager.RegisterTimer(_drainTimer);
        private void OnDisable() => TimerManager.DeregisterTimer(_drainTimer);

        public void Interact(PlayerInventory playerInventory)
        {
            if (_battery != null)
            {
                if (playerInventory.TryGivePickupable(_battery))
                {
                    _movableMovingPlatform.Deactivate();
                    _battery = null;
                    _drainTimer.Stop();
                }

                return;
            }
            
            if (playerInventory.Pickupable is null)
                return;

            if (playerInventory.Pickupable is Battery battery)
            {
                if (playerInventory.TryGetPickupable(_insertSlot))
                {
                    _battery = battery;

                    if (_battery.CurrentCharge > 0f)
                    {
                        _movableMovingPlatform.Activate();
                        _drainTimer.Start();
                    }
                }
            }
        }

        private void OnDrain()
        {
            if (_battery == null)
                return;

            _battery.RemoveCharge(_drainPerInterval);
            if (_battery.CurrentCharge <= 0f)
            {
                _movableMovingPlatform.Deactivate();
                return;
            }
                
            _drainTimer.Start();
        }
    }
}
