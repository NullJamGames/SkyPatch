using UnityEngine;
using Zenject;

namespace NJG.Runtime.Audio
{
    public class WaterfallAudio : MonoBehaviour
    {
        private AudioManager _audioManager;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        private void OnEnable() => _audioManager.PlayPersistent(_audioManager.AudioData.WaterfallHeavy, gameObject);

        private void OnDisable() => _audioManager.StopPersistent(_audioManager.AudioData.WaterfallHeavy);
    }
}