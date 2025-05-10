using System;
using UnityEngine;
using Zenject;

namespace NJG.Runtime.Managers
{
    public class VisualSettingsManager : IInitializable
    {
        private bool _isFullScreen;
        private EResolution _resolution;
        private EGraphicsQuality _graphicsQuality;
        private float _brightness;
        private float _cameraSensitivity;
        private bool _bloom;
        
        const string _fullScreenSaveString = "VisualSettings_FullScreen";
        const string _resolutionSaveString = "VisualSettings_Resolution";
        const string _graphicsQualitySaveString = "VisualSettings_GraphicsQuality";
        const string _brightnessSaveString = "VisualSettings_Brightness";
        const string _cameraSensitivitySaveString = "VisualSettings_CameraSensitivity";
        const string _bloomSaveString = "VisualSettings_Bloom";

        public event Action Ev_BrightnessChanged;
        public event Action Ev_BloomChanged;
        
        
        public bool IsFullScreen => _isFullScreen;
        public int ResolutionIndex => (int)_resolution;
        public int GraphicsQualityIndex => (int)_graphicsQuality;
        public float Brightness => _brightness;
        public float CameraSensitivity => _cameraSensitivity;
        public bool IsBloom => _bloom;
        
        public void Initialize()
        {
            if (PlayerPrefs.HasKey(_fullScreenSaveString))
                LoadSettings();
            else
            {
                _isFullScreen = true;
                _resolution = EResolution.R_1920x1080;
                _graphicsQuality = EGraphicsQuality.Ultra;
                _brightness = 0f;
                _cameraSensitivity = 1.0f;
                _bloom = true;
                SaveSettings();
            }
            
            UpdateScreen();
        }

        public void SetFullScreen(bool isFullScreen)
        {
            if (_isFullScreen == isFullScreen)
                return;
            _isFullScreen = isFullScreen;
            UpdateScreen();
            SaveSettings();
        }

        public void SetResolution(int resolutionIndex)
        {
            EResolution resolution = (EResolution)resolutionIndex;
            if(resolution == _resolution)
                return;
            _resolution = resolution;
            UpdateScreen();
            SaveSettings();
        }

        public void SetGraphicsQuality(int graphicsQualityIndex)
        {
            EGraphicsQuality graphicsQuality = (EGraphicsQuality)graphicsQualityIndex;
            if(graphicsQuality == _graphicsQuality)
                return;
            _graphicsQuality = graphicsQuality;
            QualitySettings.SetQualityLevel((int)_graphicsQuality, true);
        }
        
        public void SetBrightness(float brightness)
        {
            if(_brightness == brightness)
                return;
            _brightness = brightness;
            Ev_BrightnessChanged?.Invoke();
            SaveSettings();
        }

        public void SetCameraSensitivity(float cameraSensitivity)
        {
            if(_cameraSensitivity == cameraSensitivity)
                return;
            _cameraSensitivity = cameraSensitivity;
            SaveSettings();
        }

        public void SetBloom(bool bloom)
        {
            if(_bloom == bloom)
                return;
            _bloom = bloom;
            Ev_BloomChanged?.Invoke();
            SaveSettings();
        }

        
        
        private void UpdateScreen()
        {
            int width = 1920;
            int height = 1080;

            switch (_resolution)
            {
                case EResolution.R_3840x2160:
                    width = 3840;
                    height = 2160;
                    break;
                case EResolution.R_2560x1440:
                    width = 2560;
                    height = 1440;
                    break;
                case EResolution.R_1920x1080:
                    width = 1920;
                    height = 1080;
                    break;
                case EResolution.R_1600x900:
                    width = 1600;
                    height = 900;
                    break;
                case EResolution.R_1366x768:
                    width = 1366;
                    height = 768;
                    break;
                case EResolution.R_1280x720:
                    width = 1280;
                    height = 720;
                    break;
                case EResolution.R_1024x768:
                    width = 1024;
                    height = 768;
                    break;
            }

            var mode = _isFullScreen
                ? FullScreenMode.ExclusiveFullScreen 
                : FullScreenMode.Windowed;

            Screen.SetResolution(width, height, mode);
        }

        private void UpdateGraphicQuality()
        {
            QualitySettings.SetQualityLevel((int)_graphicsQuality, true);
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt(_fullScreenSaveString, _isFullScreen ? 1 : 0);
            PlayerPrefs.SetInt(_resolutionSaveString, (int)_resolution);
            PlayerPrefs.SetInt(_graphicsQualitySaveString, (int)_graphicsQuality);
            PlayerPrefs.SetFloat(_brightnessSaveString, _brightness);
            PlayerPrefs.SetFloat(_cameraSensitivitySaveString, _cameraSensitivity);
            PlayerPrefs.SetInt(_bloomSaveString, _bloom ? 1 : 0);
        }

        private void LoadSettings()
        {
            _isFullScreen = PlayerPrefs.GetInt(_fullScreenSaveString) != 0;
            _resolution = (EResolution)PlayerPrefs.GetInt(_resolutionSaveString);
            _graphicsQuality = (EGraphicsQuality)PlayerPrefs.GetInt(_graphicsQualitySaveString);
            _brightness = PlayerPrefs.GetFloat(_brightnessSaveString);
            _cameraSensitivity = PlayerPrefs.GetFloat(_cameraSensitivitySaveString);
            _bloom = PlayerPrefs.GetInt(_bloomSaveString) != 0;
        }
    }
    
    public enum EResolution
    {
        R_3840x2160, // 4K UHD
        R_2560x1440, // QHD
        R_1920x1080, // Full HD
        R_1600x900,  // HD+
        R_1366x768,  // WXGA (common laptop)
        R_1280x720,  // HD
        R_1024x768,  // XGA (4:3)
    }
    
    public enum EGraphicsQuality
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh,
        Ultra
    }
}
