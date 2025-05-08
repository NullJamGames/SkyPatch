using DistantLands.Cozy;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Audio
{
    public class MusicManager : MonoBehaviour
    {
        private const float _danNightCheckInterval = 5f;
        private const string _dayNightCycleParam = "Day_Night_Cycle";
        private AudioManager _audioManager;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        private void Start()
        {
            _audioManager.PlayPersistent(_audioManager.AudioData.Music);
            InvokeRepeating(nameof(SyncCozyTime), 0f, _danNightCheckInterval);
        }

        private void SyncCozyTime()
        {
            float time = CozyWeather.instance.timeModule.currentTime;
            _audioManager.SetGlobalParameter(_dayNightCycleParam, time);
        }
    }
}