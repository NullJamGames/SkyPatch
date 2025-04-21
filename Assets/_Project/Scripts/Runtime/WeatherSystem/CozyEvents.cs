using DistantLands.Cozy;
using NJG.Runtime.Events;
using UnityEngine;

namespace NJG.Runtime.WeatherSystem
{
    public class CozyEvents : MonoBehaviour
    {
        [SerializeField]
        private EventChannel _dayTimeEvent;
        [SerializeField]
        private EventChannel _nightTimeEvent;

        private readonly Empty _empty = new();

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
            Debug.Log("Day time event");
            _dayTimeEvent?.Invoke(_empty);
        }

        public void TriggerNightTimeEvent()
        {
            Debug.Log("Night time event");
            _nightTimeEvent?.Invoke(_empty);
        }
    }
}