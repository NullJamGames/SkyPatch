using DistantLands.Cozy;
using NJG.Runtime.Signals;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.WeatherSystem
{
    public class CozyEvents : MonoBehaviour
    {
        private SignalBus _signalBus;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Start()
        {
            float currentTime = CozyWeather.instance.timeModule.currentTime;
            bool isDaytime = currentTime is >= 0.25f and < 0.75f;
            if (isDaytime)
                TriggerDayTimeEvent();
            else
                TriggerNightTimeEvent();
        }

        public void TriggerDayTimeEvent()
        {
            _signalBus.Fire(new DayTimeChangeSignal(true));
        }

        public void TriggerNightTimeEvent()
        {
            _signalBus.Fire(new DayTimeChangeSignal(false));
        }
    }
}