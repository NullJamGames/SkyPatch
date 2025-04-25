using NJG.Runtime.Entity;
using NJG.Runtime.Events;
using NJG.Runtime.Managers;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class SolarPanel : MonoBehaviour, IInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private Transform _batteryHolder;
        
        [FoldoutGroup("Settings"), SerializeField]
        private float _energyPerInterval = 10f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _energyInterval = 1f;

        private bool _isDaytime;
        private CountdownTimer _intervalTimer;
        private TestBattery _battery;
        
        private void Awake()
        {
            _intervalTimer = new CountdownTimer(_energyInterval);
            _intervalTimer.OnTimerStop += () =>
            {
                if (_battery == null)
                    return;
                
                _battery.AddCharge(_energyPerInterval);
                _intervalTimer.Start();
            };
        }

        private void Update()
        {
            _intervalTimer?.Tick(Time.deltaTime);
        }

        public void Interact(PlayerInventory playerInventory)
        {
            if (_battery != null)
            {
                if (playerInventory.TryGivePickupable(_battery))
                {
                    _battery = null;
                    _intervalTimer.Stop();
                }

                return;
            }
            
            if (playerInventory.Pickupable is null)
                return;

            if (playerInventory.Pickupable is TestBattery battery)
            {
                if (playerInventory.TryGetPickupable(_batteryHolder))
                {
                    _battery = battery;
                    
                    if (_isDaytime)
                        _intervalTimer.Start();
                }
            }
        }

        public void OnMorningTime()
        {
            _isDaytime = true;

            if (_battery == null)
                return;
            
            if (_intervalTimer.IsPaused)
                _intervalTimer.Resume();
            else
                _intervalTimer.Start();
        }

        public void OnEveningTime()
        {
            _isDaytime = false;
            
            if (_battery == null)
                return;
            
            _intervalTimer.Pause();
        }
    }
}