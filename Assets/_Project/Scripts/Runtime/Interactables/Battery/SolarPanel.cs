using NJG.Runtime.Signals;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Interactables
{
    public class SolarPanel : BatteryInteractable
    {
        [FoldoutGroup("Settings"), SerializeField]
        private float _energyPerInterval = 10f;
        [FoldoutGroup("Settings"), SerializeField]
        private float _timerInterval = 1f;
        
        private CountdownTimer _intervalTimer;
        private bool _isDaytime;
        private SignalBus _signalBus;
        
        [Inject]
        private void Construct(SignalBus signalBus) => _signalBus = signalBus;

        private void Awake()
        {
            _intervalTimer = new CountdownTimer(_timerInterval);
            _intervalTimer.OnTimerStop += OnTimerTick;
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<DayTimeChangeSignal>(OnDayTimeChanged);
            
            TimerManager.RegisterTimer(_intervalTimer);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<DayTimeChangeSignal>(OnDayTimeChanged);
            
            TimerManager.DeregisterTimer(_intervalTimer);
        }

        protected override void OnBatteryInserted()
        {
            if (_isDaytime)
                _intervalTimer.Start();
        }

        protected override void OnBatteryRemoved()
        {
            _intervalTimer.Stop();
        }

        private void OnTimerTick()
        {
            if (_battery == null)
                return;
                
            _battery.AddCharge(_energyPerInterval);
            _intervalTimer.Start();
        }

        private void OnDayTimeChanged(DayTimeChangeSignal signal)
        {
            _isDaytime = signal.IsDayTime;
            
            if (_battery == null)
                return;

            if (!_isDaytime)
            {
                _intervalTimer.Pause();
                return;
            }
            
            if (_intervalTimer.IsPaused)
                _intervalTimer.Resume();
            else
                _intervalTimer.Start();
        }
    }
}