using NJG.Runtime.Entity;
using NJG.Runtime.Signals;
using NJG.Utilities.ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

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
        private SignalBus _signalBus;
        
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        private void Awake()
        {
            _intervalTimer = new CountdownTimer(_energyInterval);
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

        public void Interact(PlayerInventory playerInventory)
        {
            if (_battery != null)
            {
                TryRemoveBattery(playerInventory);
                return;
            }

            TryInsertBattery(playerInventory);
        }

        private void TryRemoveBattery(PlayerInventory playerInventory)
        {
            if (!playerInventory.TryGivePickupable(_battery))
                return;
            
            _battery = null;
            _intervalTimer.Stop();
        }

        private void TryInsertBattery(PlayerInventory playerInventory)
        {
            switch (playerInventory.Pickupable)
            {
                case null:
                    return;
                case TestBattery battery:
                {
                    if (!playerInventory.TryGetPickupable(_batteryHolder))
                        return;
                    
                    _battery = battery;
                
                    if (_isDaytime)
                        _intervalTimer.Start();

                    break;
                }
            }
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