using System;
using NJG.Runtime.Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NJG.Runtime.UI
{
    public class VolumeSettings : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private SVolumeSlider[] _volumeSliders;

        private AudioManager _audioManager;

        [Inject]
        private void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void Start()
        {
            foreach (SVolumeSlider volumeSlider in _volumeSliders)
            {
                volumeSlider.Slider.SetValueWithoutNotify(_audioManager.GetCurrentVolume(volumeSlider.VolumeType));

                volumeSlider.Slider.onValueChanged.AddListener(val =>
                    OnSliderValueChanged(val, volumeSlider.VolumeType));
            }
        }

        private void OnSliderValueChanged(float val, VolumeType volType)
        {
            _audioManager.SetCurrentVolume(volType, val);
        }
    }

    [Serializable]
    public struct SVolumeSlider
    {
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private VolumeType _volumeType;

        public Slider Slider => _slider;
        public VolumeType VolumeType => _volumeType;
    }
}