using System;
using FMOD;
using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NJG.Runtime.UI
{
    public enum VolumeType{ Master, Music, SFX }
    public class VolumeSettings : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField] 
        private SVolumeSlider[] _volumeSliders;
        
        private void Start()
        {
            foreach (SVolumeSlider volumeSlider in _volumeSliders)
            {
                GetBus(volumeSlider.VolumeType).getVolume(out float volume);
                volumeSlider.Slider.SetValueWithoutNotify(volume);
                    
                volumeSlider.Slider.onValueChanged.AddListener((val) => OnSliderValueChanged(val, volumeSlider.VolumeType));
            }
        }

        private void OnSliderValueChanged(float val, VolumeType volType)
        {
            GetBus(volType).setVolume(val);
        }

        private Bus GetBus(VolumeType volType)
        {
            string path = GetVolumePath(volType);
            return FMODUnity.RuntimeManager.GetBus(path);
        }

        private string GetVolumePath(VolumeType volumeType)
        {
            return volumeType switch
            {
                VolumeType.Master => "bus:/",
                VolumeType.Music => "bus:/Music",
                VolumeType.SFX => "bus:/SFX",
                _ => "#Errorrr"
            };
        }
    }
    
    [System.Serializable]
    public struct SVolumeSlider
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private VolumeType _volumeType;
        
        public Slider Slider => _slider;
        public VolumeType VolumeType => _volumeType;
    }
}