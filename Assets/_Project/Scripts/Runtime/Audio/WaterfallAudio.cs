using NJG.Runtime.Audio;
using UnityEngine;
using Zenject;

namespace NJG
{
    public class WaterfallAudio : MonoBehaviour
    {
        private AudioManager _audioManager;
        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;
        void OnEnable()
        {
            _audioManager.PlayPersistent(_audioManager.AudioData.WaterfallHeavy,gameObject);
        }
        private void OnDisable()
        {
            _audioManager.StopPersistent(_audioManager.AudioData.WaterfallHeavy);
        }
        private void OnDestroy()
        {
            _audioManager.StopPersistent(_audioManager.AudioData.WaterfallHeavy);
        }
    }
}
