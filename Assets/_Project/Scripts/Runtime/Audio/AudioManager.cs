using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Zenject;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace NJG.Runtime.Audio
{
    public enum VolumeType
    {
        Master,
        Music,
        SFX
    }

    public class AudioManager : IInitializable, ILateDisposable
    {
        private readonly Dictionary<EventReference, EventInstance> _activeEvents = new();
        private readonly Dictionary<GameObject, Dictionary<EventReference, EventInstance>> _keyedInstances = new();
        private readonly List<EventInstance> _oneShotInstances = new();

        private AudioManager(AudioDataSO audioData) => AudioData = audioData;

        public AudioDataSO AudioData { get; private set; }

        public void Initialize()
        {
            float masterVolume = LoadVolume(VolumeType.Master);
            float musicVolume = LoadVolume(VolumeType.Music);
            float sfxVolume = LoadVolume(VolumeType.SFX);
            SetCurrentVolume(VolumeType.Master, masterVolume);
            SetCurrentVolume(VolumeType.Music, musicVolume);
            SetCurrentVolume(VolumeType.SFX, sfxVolume);
        }

        public float GetEventLength(EventReference sound)
        {
            if (sound.IsNull)
                return 0f;

            EventDescription eventDescription = RuntimeManager.GetEventDescription(sound);
            if (!eventDescription.isValid())
                return 0f;

            eventDescription.getLength(out int lengthMs);
            return lengthMs / 1000f;
        }

        #region Volume Control

        public void SetCurrentVolume(VolumeType volumeType, float volume)
        {
            string path = GetVolumePath(volumeType);
            VCA vca = RuntimeManager.GetVCA(path);
            volume = Mathf.Clamp01(volume);
            vca.setVolume(volume);
            SaveVolume(volumeType, volume);
        }

        public float GetCurrentVolume(VolumeType volumeType)
        {
            string path = GetVolumePath(volumeType);
            VCA vca = RuntimeManager.GetVCA(path);
            vca.getVolume(out float volume);
            return volume;
        }

        private string GetVolumePath(VolumeType volumeType)
        {
            return volumeType switch
            {
                VolumeType.Master => "vca:/Master",
                VolumeType.Music => "vca:/Music",
                VolumeType.SFX => "vca:/SFX",
                _ => "vca:/Master"
            };
        }

        private void SaveVolume(VolumeType volumeType, float volume)
        {
            PlayerPrefs.SetFloat($"{volumeType}Volume", volume);
            PlayerPrefs.Save();
        }

        private float LoadVolume(VolumeType volumeType)
        {
            string volumeKey = $"{volumeType}Volume";
            return PlayerPrefs.HasKey(volumeKey) ? PlayerPrefs.GetFloat(volumeKey) : 0.5f;
        }

        #endregion

        #region Play Music/SFX

        public void PlayOneShotAndForget(EventReference sound, Vector3 position = default)
        {
            RuntimeManager.PlayOneShot(sound, position);
        }

        /// <summary>
        ///     Currently for one shot we have param:
        /// </summary>
        public void PlayOneShotAndForget(EventReference sound, string parameterName, string parameterValue,
            Vector3 position = default
        )
        {
            EventInstance instance = RuntimeManager.CreateInstance(sound);
            instance.setParameterByNameWithLabel(parameterName, parameterValue);
            instance.start();
            instance.release();
        }

        public void PlayOneShotTracked(EventReference sound, Vector3 position = default)
        {
            EventInstance instance = RuntimeManager.CreateInstance(sound);
            instance.start();
            _oneShotInstances.Add(instance);
        }

        public void PlayOneShotTracked(EventReference sound, string parameterName, string parameterValue,
            Vector3 position = default
        )
        {
            EventInstance instance = RuntimeManager.CreateInstance(sound);
            instance.setParameterByNameWithLabel(parameterName, parameterValue);
            instance.start();
            _oneShotInstances.Add(instance);
        }

        public void StopAllTrackedOneShots()
        {
            foreach (EventInstance instance in _oneShotInstances.Where(instance => instance.isValid()))
            {
                instance.stop(STOP_MODE.IMMEDIATE);
                instance.release();
            }

            _oneShotInstances.Clear();
        }

        public void SetGlobalParameter(string parameterName, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        }

        public void SetGlobalParameter(string parameterName, string label)
        {
            RuntimeManager.StudioSystem.setParameterByNameWithLabel(parameterName, label);
        }

        public void PlayPersistent(EventReference sound, GameObject sourceObject = null,
            Rigidbody sourceRigidbody = null
        )
        {
            if (_activeEvents.ContainsKey(sound))
                return;

            EventInstance instance = RuntimeManager.CreateInstance(sound);
            if (sourceObject != null)
            {
                Rigidbody rb = sourceRigidbody != null ? sourceRigidbody : sourceObject.GetComponent<Rigidbody>();
                RuntimeManager.AttachInstanceToGameObject(instance, sourceObject, rb);
            }

            instance.start();
            _activeEvents[sound] = instance;
        }

        /// <summary>
        ///     Currently for persistent params we have param:
        /// </summary>
        public void PlayPersistent(EventReference sound, string parameterName, string parameterValue,
            GameObject sourceObject = null, Rigidbody sourceRigidbody = null
        )
        {
            if (sound.IsNull || _activeEvents.ContainsKey(sound))
                return;

            EventInstance instance = RuntimeManager.CreateInstance(sound);
            instance.setParameterByNameWithLabel(parameterName, parameterValue);

            if (sourceObject != null)
            {
                Rigidbody rb = sourceRigidbody != null ? sourceRigidbody : sourceObject.GetComponent<Rigidbody>();
                RuntimeManager.AttachInstanceToGameObject(instance, sourceObject, rb);
            }

            instance.start();
            _activeEvents[sound] = instance;
        }

        public void StopPersistent(EventReference sound)
        {
            if (_activeEvents.TryGetValue(sound, out EventInstance instance))
            {
                instance.stop(STOP_MODE.ALLOWFADEOUT);
                instance.release();
                _activeEvents.Remove(sound);
            }
        }

        public void StopAllPersistentSounds()
        {
            foreach (EventInstance instance in _activeEvents.Values)
            {
                instance.stop(STOP_MODE.ALLOWFADEOUT);
                instance.release();
            }

            _activeEvents.Clear();
        }

        public void StartKeyedInstance(GameObject keyObject, EventReference sound, Rigidbody sourceRigidbody = null)
        {
            if (keyObject == null || sound.IsNull)
                return;

            if (!_keyedInstances.TryGetValue(keyObject, out Dictionary<EventReference, EventInstance> soundMap))
            {
                soundMap = new Dictionary<EventReference, EventInstance>();
                _keyedInstances[keyObject] = soundMap;
            }

            if (!soundMap.TryGetValue(sound, out EventInstance instance) || !instance.isValid())
            {
                instance = RuntimeManager.CreateInstance(sound);
                soundMap[sound] = instance;

                Rigidbody rb = sourceRigidbody != null ? sourceRigidbody : keyObject.GetComponent<Rigidbody>();
                RuntimeManager.AttachInstanceToGameObject(instance, keyObject, rb);
            }

            instance.start();
        }

        public void SetKeyedInstanceParamater(GameObject keyObject, EventReference sound, string parameterName,
            float parameterValue
        )
        {
            if (!_keyedInstances.ContainsKey(keyObject) || !_keyedInstances[keyObject].ContainsKey(sound))
                StartKeyedInstance(keyObject, sound);

            _keyedInstances[keyObject][sound].setParameterByName(parameterName, parameterValue);
        }

        public void SetKeyedInstanceParamater(GameObject keyObject, EventReference sound, string parameterName,
            string label
        )
        {
            if (!_keyedInstances.ContainsKey(keyObject) || !_keyedInstances[keyObject].ContainsKey(sound))
                StartKeyedInstance(keyObject, sound);

            _keyedInstances[keyObject][sound].setParameterByNameWithLabel(parameterName, label);
        }

        public void StopKeyedInstance(GameObject keyObject, EventReference sound,
            STOP_MODE stopMode = STOP_MODE.IMMEDIATE
        )
        {
            if (!_keyedInstances.ContainsKey(keyObject) || !_keyedInstances[keyObject].ContainsKey(sound))
                return;
            _keyedInstances[keyObject][sound].stop(stopMode);
            _keyedInstances[keyObject][sound].release();
            _keyedInstances[keyObject].Remove(sound);
        }

        public void DestroyKeyAndRemoveInstances(GameObject keyObject)
        {
            if (!_keyedInstances.ContainsKey(keyObject))
                return;
            foreach (EventInstance VARIABLE in _keyedInstances[keyObject].Values)
            {
                VARIABLE.stop(STOP_MODE.IMMEDIATE);
                VARIABLE.release();
            }

            _keyedInstances.Remove(keyObject);
        }

        private void StopAllKeyedInstances()
        {
            foreach (KeyValuePair<GameObject, Dictionary<EventReference, EventInstance>> VARIABLE in _keyedInstances)
                foreach (KeyValuePair<EventReference, EventInstance> VARIABLE2 in VARIABLE.Value)
                {
                    VARIABLE2.Value.stop(STOP_MODE.IMMEDIATE);
                    VARIABLE2.Value.release();
                }

            _keyedInstances.Clear();
        }

        public void LateDispose()
        {
            StopAllPersistentSounds();
            StopAllTrackedOneShots();
            StopAllKeyedInstances();
        }

        #endregion
    }
}