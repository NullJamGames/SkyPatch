using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class BatteryPanel : BatteryInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private BatteryPowered _batteryPowered;
        
        [FoldoutGroup("Settings"), SerializeField]
        private float _activationDelay = 0.5f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _drainPerInterval = 1f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _drainInterval = 1f;
        
        private CountdownTimer _drainTimer;
        private CountdownTimer _delayTimer;

        private void Awake()
        {
            _drainTimer = new CountdownTimer(_drainInterval);
            _drainTimer.OnTimerStop += OnDrainBattery;
            
            _delayTimer = new CountdownTimer(_activationDelay);
            _delayTimer.OnTimerStop += ActivatePlatform;
        }

        protected override void OnBatteryInserted()
        {
            if (_battery.CurrentCharge <= 0f)
                return;
            
            if (!_delayTimer.IsRunning)
                _delayTimer.Start();
        }

        private void ActivatePlatform()
        {
            _batteryPowered.Activate();
            _drainTimer.Start();
        }

        protected override void OnBatteryRemoved()
        {
            if (_delayTimer.IsRunning)
                _delayTimer.Pause();
            
            _batteryPowered.Deactivate();
            _drainTimer.Stop();
        }

        private void OnDrainBattery()
        {
            if (_battery == null || !_batteryPowered.IsActive)
                return;

            _battery.RemoveCharge(_drainPerInterval);
            if (_battery.CurrentCharge <= 0f)
            {
                _batteryPowered.Deactivate();
                return;
            }
                
            _drainTimer.Start();
        }
    }
}
