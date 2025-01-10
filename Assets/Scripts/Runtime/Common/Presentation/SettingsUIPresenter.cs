using Runtime.Common.View;
using Runtime.ETC;
using Runtime.InGameSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Common.Presentation
{
    public class SettingsUIPresenter
    {
        private readonly SettingsUIView _settingsUIView;

        public SettingsUIPresenter(SettingsUIView settingsUIView)
        {
            _settingsUIView = settingsUIView;
            
            InitVolume();
            InitScreenMode();
            
            _settingsUIView.BgmVolumeSlider.onValueChanged.AddListener(SetBgmVolume);
            _settingsUIView.SfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
            
            _settingsUIView.FullScreenToggle.onValueChanged.AddListener(SetFullScreenMode);
            _settingsUIView.WindowScreenToggle.onValueChanged.AddListener(SetWindowScreenMode);
            
            _settingsUIView.GameExitButton.onClick.AddListener(OnGameExitButtonClicked);
            _settingsUIView.ExitButton.onClick.AddListener(_settingsUIView.GameSettingToggle);
        }
        
        private void InitVolume()
        {
            _settingsUIView.BgmVolumeSlider.value = Managers.Data.BgmVolume;
            _settingsUIView.SfxVolumeSlider.value = Managers.Data.SfxVolume;
        }

        private void InitScreenMode()
        {
            if (Managers.Data.IsFullscreen)
            {
                _settingsUIView.FullScreenToggle.SetIsOnWithoutNotify(true);
                _settingsUIView.FullScreenToggle.interactable = false;

                _settingsUIView.WindowScreenToggle.SetIsOnWithoutNotify(false);
                _settingsUIView.WindowScreenToggle.interactable = true;
                
                Managers.Data.IsFullscreen = true;
                Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
            }
            else
            {
                _settingsUIView.FullScreenToggle.SetIsOnWithoutNotify(false);
                _settingsUIView.FullScreenToggle.interactable = true;

                _settingsUIView.WindowScreenToggle.SetIsOnWithoutNotify(true);
                _settingsUIView.WindowScreenToggle.interactable = false;
                
                Managers.Data.IsFullscreen = false;
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            }
        }

        private void SetBgmVolume(float volume)
        {
            Managers.Data.BgmVolume = volume;
            Managers.Sound.BGM.volume = volume;
            Managers.Sound.LuckyBGM.volume = volume;
        }

        private void SetSfxVolume(float volume)
        {
            Managers.Data.SfxVolume = volume;
            Managers.Sound.SFX.volume = volume;
        }

        private void SetFullScreenMode(bool isOn)
        {
            if (isOn)
            {
                Managers.Sound.Play(Sound.SFX, "Setting/SFX_Setting_UI_Window");
                _settingsUIView.FullScreenToggle.SetIsOnWithoutNotify(true);
                _settingsUIView.FullScreenToggle.interactable = false;

                _settingsUIView.WindowScreenToggle.SetIsOnWithoutNotify(false);
                _settingsUIView.WindowScreenToggle.interactable = true;
                
                Managers.Data.IsFullscreen = true;
                Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
            }
        }
        
        private void SetWindowScreenMode(bool isOn)
        {
            if (isOn)
            {
                Managers.Sound.Play(Sound.SFX, "Setting/SFX_Setting_UI_Window");
                _settingsUIView.FullScreenToggle.SetIsOnWithoutNotify(false);
                _settingsUIView.FullScreenToggle.interactable = true;

                _settingsUIView.WindowScreenToggle.SetIsOnWithoutNotify(true);
                _settingsUIView.WindowScreenToggle.interactable = false;
                
                Managers.Data.IsFullscreen = false;
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            }
        }
        
        private void OnGameExitButtonClicked()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}