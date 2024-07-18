using Runtime.Common.View;
using UnityEngine;

namespace Runtime.Common.Presentation
{
    public class SettingsUIPresenter
    {
        private readonly SettingsUIView _settingsUIView;

        public SettingsUIPresenter(SettingsUIView settingsUIView)
        {
            _settingsUIView = settingsUIView;

            InitVolume();
            _settingsUIView.BgmVolumeSlider.onValueChanged.AddListener(SetBgmVolume);
            _settingsUIView.SfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
            
            _settingsUIView.GameExitButton.onClick.AddListener(OnGameExitButtonClicked);
            _settingsUIView.ExitButton.onClick.AddListener(_settingsUIView.GameSettingToggle);
        }
        
        private void InitVolume()
        {
            _settingsUIView.BgmVolumeSlider.value = Managers.Data.BgmVolume;
            _settingsUIView.SfxVolumeSlider.value = Managers.Data.SfxVolume;
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
        
        private void OnGameExitButtonClicked()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}