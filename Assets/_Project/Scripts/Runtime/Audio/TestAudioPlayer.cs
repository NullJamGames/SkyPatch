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

        [FoldoutGroup("Persistant"), Button(ButtonSizes.Medium)]
        private void PlayPersistant() => _audioManager.PlayPersistent(_audioEvent);
        [FoldoutGroup("Persistant"), Button(ButtonSizes.Medium)]
        private void StopPersistant() => _audioManager.StopPersistent(_audioEvent);
        [FoldoutGroup("Persistant"), Button(ButtonSizes.Medium)]
        private void StopAllPersistantSounds() => _audioManager.StopAllPersistentSounds();
        
        [FoldoutGroup("One Shot Tracked"), Button(ButtonSizes.Medium)]
        private void PlayOneShotTracked() => _audioManager.PlayOneShotTracked(_audioEvent);
        [FoldoutGroup("One Shot Tracked"), Button(ButtonSizes.Medium)]
        private void StopAllTrackedOneShots() => _audioManager.StopAllTrackedOneShots();
        
        [FoldoutGroup("One Shot and Forget"), Button(ButtonSizes.Medium)]
        private void PlayOneShotAndForget() => _audioManager.PlayOneShotAndForget(_audioEvent);
        [FoldoutGroup("Keyed Instance"), Button(ButtonSizes.Medium)]
        private void StartKeyedInstance() => _audioManager.StartKeyedInstance(gameObject, _audioEvent);
        [FoldoutGroup("Keyed Instance"), SerializeField]
        private string _paramaterName;
        [FoldoutGroup("Keyed Instance"), SerializeField]
        private float _value;
        [FoldoutGroup("Keyed Instance"), SerializeField]
        private string _label;
        [FoldoutGroup("Keyed Instance"), Button(ButtonSizes.Medium)]
        private void SetParamaterByValue() => _audioManager.SetKeyedInstanceParamater(gameObject, _audioEvent, _paramaterName, _value);
        [FoldoutGroup("Keyed Instance"), Button(ButtonSizes.Medium)]
        private void SetParamaterByLabel() => _audioManager.SetKeyedInstanceParamater(gameObject, _audioEvent, _paramaterName, _label);
        [FoldoutGroup("Keyed Instance"), Button(ButtonSizes.Medium)]
        private void StopKeyedInstance() => _audioManager.DestroyKeyAndRemoveInstances(gameObject);
        
        [FoldoutGroup("Global Parameter"), SerializeField]
        private string _globalParameterName;
        [FoldoutGroup("Global Parameter"), SerializeField]
        private float _globalValue;
        [FoldoutGroup("Global Parameter"), SerializeField]
        private string _globalLabel;
        [FoldoutGroup("Global Parameter"), Button(ButtonSizes.Medium)]
        private void SetGlobalParameterByValue() => _audioManager.SetGlobalParameter(_globalParameterName, _globalValue);
        [FoldoutGroup("Global Parameter"), Button(ButtonSizes.Medium)]
        private void SetGlobalParameterByLabel() => _audioManager.SetGlobalParameter(_globalParameterName, _globalLabel);
    }
}