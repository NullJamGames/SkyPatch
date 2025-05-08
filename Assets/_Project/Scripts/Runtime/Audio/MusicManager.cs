using DistantLands.Cozy;
using NJG.Runtime.Signals;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Audio
{
    public class MusicManager : MonoBehaviour
    {
        private AudioManager _audioManager;

        [Inject]
        private void Construct(SignalBus signalBus, AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void Start()
        {
            _audioManager.PlayPersistent(_audioManager.AudioData.Music);
            DontDestroyOnLoad(gameObject);
        }
    }
}