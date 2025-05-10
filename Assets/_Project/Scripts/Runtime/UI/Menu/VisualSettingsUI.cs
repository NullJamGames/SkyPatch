using NJG.Runtime.Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace NJG.Runtime.UI
{
    public class VisualSettingsUI : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        private Toggle _fullScreenToggle;
        [FoldoutGroup("References"), SerializeField]
        private TMP_Dropdown _resolutionDropdown;
        [FoldoutGroup("References"), SerializeField]
        private TMP_Dropdown _graphicsDropdown;
        [FoldoutGroup("References"), SerializeField]
        private Slider _brightnessSlider;
        [FoldoutGroup("References"), SerializeField] 
        private Slider _cameraSensitivitySlider;
        [FoldoutGroup("References"), SerializeField] 
        private Toggle _bloomToggle;
        
        private VisualSettingsManager _visualSettingsManager;

        
        [Inject]
        void Construct(VisualSettingsManager visualSettingsManager)
        {
            _visualSettingsManager = visualSettingsManager;
        }

        private void Start()
        {
            _fullScreenToggle.SetIsOnWithoutNotify(_visualSettingsManager.IsFullScreen);
            _fullScreenToggle.onValueChanged.AddListener(_visualSettingsManager.SetFullScreen);
            
            _resolutionDropdown.SetValueWithoutNotify(_visualSettingsManager.ResolutionIndex);
            _resolutionDropdown.onValueChanged.AddListener(_visualSettingsManager.SetResolution);
            
            _graphicsDropdown.SetValueWithoutNotify(_visualSettingsManager.GraphicsQualityIndex);
            _graphicsDropdown.onValueChanged.AddListener(_visualSettingsManager.SetGraphicsQuality);
            
            _brightnessSlider.SetValueWithoutNotify(_visualSettingsManager.Brightness);
            _brightnessSlider.onValueChanged.AddListener(_visualSettingsManager.SetBrightness);
            
            _cameraSensitivitySlider.SetValueWithoutNotify(_visualSettingsManager.CameraSensitivity);
            _cameraSensitivitySlider.onValueChanged.AddListener(_visualSettingsManager.SetCameraSensitivity);
            
            _bloomToggle.SetIsOnWithoutNotify(_visualSettingsManager.IsBloom);
            _bloomToggle.onValueChanged.AddListener(_visualSettingsManager.SetBloom);
        }
        
    }
}