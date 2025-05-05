using FMODUnity;
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
        
        private CountdownTimer _intervalTimer;
        private bool _isDaytime;
        private SignalBus _signalBus;
        private AudioManager _audioManager;


        [Inject]
        private void Construct(SignalBus signalBus) => _signalBus = signalBus;
        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager=audioManager;    

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
            {
                _intervalTimer.Start();
                FMOD.Studio.EventInstance istance;
                istance = FMODUnity.RuntimeManager.CreateInstance(_audioManager.AudioData.RechargingAlarm);
                _audioManager.PlayPersistent(_audioManager.AudioData.RechargingAlarm, gameObject);
            }
        }

        protected override void OnBatteryRemoved()
        {
            _intervalTimer.Stop();
            _audioManager.StopPersistent(_audioManager.AudioData.RechargingAlarm);
        }

        private void OnTimerTick()
        {
            if (_battery == null)
                return;
                
            _battery.AddCharge(_energyPerInterval);
            _intervalTimer.Start();

            //I am struggling to find a way to implement the battery recharged sound.
            // I have a Parameter local to the RechargingAlarm named IsBatteryReady that has two parameters
            // and is of type Label: NotReady/Ready
            // I want that when the battery is fully charged the state will change from NotReady to Ready
        }

        private void OnDayTimeChanged(DayTimeChangeSignal signal)
        {
            _isDaytime = signal.IsDayTime;

            if (!_isDaytime)
            {
                _audioManager.StopPersistent(_audioManager.AudioData.SolarPanelStatic);
                _audioManager.StopPersistent(_audioManager.AudioData.RechargingAlarm);
            }
            else
                _audioManager.PlayPersistent(_audioManager.AudioData.SolarPanelStatic, gameObject);

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