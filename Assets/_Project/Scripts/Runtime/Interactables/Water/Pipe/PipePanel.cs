using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NJG.Runtime.Interactables
{
    public class PipePanel : BatteryInteractable
    {
        [FoldoutGroup("References"), SerializeField]
        private Pipe _pipe;

        [FoldoutGroup("Settings"), SerializeField]
        private float _drainPerInterval = 1f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _drainInterval = 1f;

        private CountdownTimer _drainTimer;

        private void Awake()
        {
            _drainTimer = new CountdownTimer(_drainInterval);
            _drainTimer.OnTimerStop += OnDrainBattery;
        }

        private void OnEnable() => TimerManager.RegisterTimer(_drainTimer);

        private void OnDisable() => TimerManager.DeregisterTimer(_drainTimer);

        protected override void OnBatteryInserted()
        {
            if (_battery.CurrentCharge <= 0f)
                return;

            _pipe.Activate();
            _drainTimer.Start();
        }

        protected override void OnBatteryRemoved()
        {
            _pipe.Deactivate();
            _drainTimer.Stop();
        }

        private void OnDrainBattery()
        {
            if (_battery == null)
                return;

            _battery.RemoveCharge(_drainPerInterval);
            if (_battery.CurrentCharge <= 0f)
            {
                _pipe.Deactivate();
                return;
            }

            _drainTimer.Start();
        }
    }
}