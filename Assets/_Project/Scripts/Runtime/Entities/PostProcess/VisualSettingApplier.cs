using NJG.Runtime.Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace NJG.Runtime.Entities
{
    public class VisualSettingApplier : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private Volume  _globalVolume;
        
        private ColorAdjustments _colorAdjustments;
        private Bloom _bloom;

        private VisualSettingsManager _visualSettingsManager;

        [Inject]
        void Construct(VisualSettingsManager visualSettingsManager)
        {
            _visualSettingsManager = visualSettingsManager;
        }

        private void OnEnable()
        {
            if (!_globalVolume.profile.TryGet(out _bloom))
                Debug.LogError("Cannot find Bloom profile");

            if (!_globalVolume.profile.TryGet(out _colorAdjustments))
                Debug.LogError("Cannot find Color Adjusments profile");
            
            ApplyBloomSetting();
            ApplyBrightnessSetting();
            
            _visualSettingsManager.Ev_BloomChanged += ApplyBloomSetting;
            _visualSettingsManager.Ev_BrightnessChanged += ApplyBrightnessSetting;
        }

        private void OnDisable()
        {
            _visualSettingsManager.Ev_BloomChanged -= ApplyBloomSetting;
            _visualSettingsManager.Ev_BrightnessChanged -= ApplyBrightnessSetting;
        }

        private void ApplyBloomSetting()
        {
            if(_bloom)
                _bloom.active =_visualSettingsManager.IsBloom;
        }

        private void ApplyBrightnessSetting()
        {
            if (!_colorAdjustments)
                return;
            
            _colorAdjustments.postExposure.value = _visualSettingsManager.Brightness;
            _colorAdjustments.postExposure.overrideState = true;
        }
    }
}