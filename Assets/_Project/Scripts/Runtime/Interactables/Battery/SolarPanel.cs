using FMOD.Studio;
using NJG.Runtime.Audio;
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
        private AudioManager _audioManager;

        private CountdownTimer _intervalTimer;
        private bool _isDaytime;
        private SignalBus _signalBus;

        [Inject]
        private void Construct(SignalBus signalBus, AudioManager audioManager)
        {
            _signalBus = signalBus;
            _audioManager = audioManager;
        }

        private void Awake()
        {
            _intervalTimer = new CountdownTimer(_timerInterval);
            _intervalTimer.OnTimerStop += OnTimerTick;
        }

        private void OnEnable() => _signalBus.Subscribe<DayTimeChangeSignal>(OnDayTimeChanged);

        private void OnDisable() => _signalBus.Unsubscribe<DayTimeChangeSignal>(OnDayTimeChanged);

        protected override void OnBatteryInserted()
        {
            if (!_isDaytime || _battery.CurrentCharge >= 100f)
                return;

            _intervalTimer.Start();

            _audioManager.StartKeyedInstance(gameObject, _audioManager.AudioData.RechargingAlarm);
            _audioManager.SetKeyedInstanceParamater(gameObject, _audioManager.AudioData.RechargingAlarm,
                "IsBatteryReady", "NotReady");
        }

        protected override void OnBatteryRemoved()
        {
            _intervalTimer.Stop();
            _audioManager.StopKeyedInstance(gameObject, _audioManager.AudioData.RechargingAlarm,
                STOP_MODE.ALLOWFADEOUT);
        }

        private void OnTimerTick()
        {
            if (_battery == null)
                return;

            _battery.AddCharge(_energyPerInterval);
            _intervalTimer.Start();

            // TODO: I believe this will not prevent this from running again if the time of day changes...
            if (_battery.CurrentCharge >= 100f)
            {
                _intervalTimer.Pause();
                _audioManager.SetKeyedInstanceParamater(gameObject, _audioManager.AudioData.RechargingAlarm,
                    "IsBatteryReady", "Ready");
            }
        }

        private void OnDayTimeChanged(DayTimeChangeSignal signal)
        {
            _isDaytime = signal.IsDayTime;

            if (!_isDaytime)
            {
                _audioManager.StopPersistent(_audioManager.AudioData.SolarPanelStatic);
                _audioManager.StopKeyedInstance(gameObject, _audioManager.AudioData.RechargingAlarm,
                    STOP_MODE.ALLOWFADEOUT);
            }
            else
            {
                _audioManager.PlayPersistent(_audioManager.AudioData.SolarPanelStatic, gameObject);
            }

            if (_battery == null)
                return;

            if (_battery.CurrentCharge < 100f && _isDaytime)
                _audioManager.StartKeyedInstance(gameObject, _audioManager.AudioData.RechargingAlarm);

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