using Runtime.Common.Domain;
using Runtime.Common.View;

namespace Runtime.Common.Presentation
{
    public class SettingsUIPresenter
    {
        private readonly SettingsUIView _settingsUIView;
        private readonly SettingsData _settingsData;

        public SettingsUIPresenter(SettingsUIView settingsUIView, SettingsData settingsData)
        {
            _settingsUIView = settingsUIView;
            _settingsData = settingsData;
        }

        public void SetMusicVolume(float volume)
        {
            _settingsUIView.SetMusicVolume(volume);
            _settingsData.MusicVolume = volume;
        }
        
        public void SetSfxVolume(float volume)
        {
            _settingsUIView.SetSfxVolume(volume);
            _settingsData.SfxVolume = volume;
        }
    }
}