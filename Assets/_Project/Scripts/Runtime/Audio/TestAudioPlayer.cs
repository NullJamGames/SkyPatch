using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Audio
{
    public class TestAudioPlayer : MonoBehaviour
    {
        [BoxGroup("Audio"), SerializeField]
        private EventReference _audioEvent;
        
        private AudioManager _audioManager;

        [Inject]
        private void Construct(AudioManager audioManager) => _audioManager = audioManager;

        private void Start()
        {
            // Use this to play sounds from the AudioData (how they will be played in game).
            //_audioManager.PlayPersistent(_audioManager.AudioData.Music);
        }

        [BoxGroup("Persistant"), Button(ButtonSizes.Medium)]
        private void PlayPersistant() => _audioManager.PlayPersistent(_audioEvent);
        [BoxGroup("Persistant"), Button(ButtonSizes.Medium)]
        private void StopPersistant() => _audioManager.StopPersistent(_audioEvent);
        [BoxGroup("Persistant"), Button(ButtonSizes.Medium)]
        private void StopAllPersistantSounds() => _audioManager.StopAllPersistentSounds();
        
        [BoxGroup("One Shot Tracked"), Button(ButtonSizes.Medium)]
        private void PlayOneShotTracked() => _audioManager.PlayOneShotTracked(_audioEvent);
        [BoxGroup("One Shot Tracked"), Button(ButtonSizes.Medium)]
        private void StopAllTrackedOneShots() => _audioManager.StopAllTrackedOneShots();
        
        [BoxGroup("One Shot and Forget"), Button(ButtonSizes.Medium)]
        private void PlayOneShotAndForget() => _audioManager.PlayOneShotAndForget(_audioEvent);
    }
}