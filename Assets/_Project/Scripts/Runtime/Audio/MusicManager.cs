using DistantLands.Cozy;
using FMODUnity;
using NJG.Runtime.Audio;
using NJG.Runtime.WeatherSystem;
using System.Collections;
using UnityEngine;
using Zenject;

namespace NJG
{

    public class MusicManager : MonoBehaviour
    {

        private AudioManager _audioManager;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;
        void Start()
        {
            _audioManager.PlayPersistent(_audioManager.AudioData.Music);
            StartCoroutine(SyncTimeWithFMOD());
        }

        IEnumerator SyncTimeWithFMOD()
        {
            while (true)
            {
                float time = CozyWeather.instance.timeModule.currentTime;
              _audioManager.SetGlobalParameter("Day_Night_Cycle", time);
                yield return new WaitForSeconds(5f);
            }
        }
    }
}
