using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.Settings
{
    public class SettingsUIPresenter
    {
        private SettingsUIView _view;
        private SettingsData _data;

        public SettingsUIPresenter(SettingsUIView view)
        {
            _view = view;
            
            _data = Addressables.LoadAssetAsync<SettingsData>("SettingsData").Result;
        }
        
        public void OnViewCreated()
        {
            _view.SetMusicVolume(_data.MusicVolume);
        }

        public void OnMusicVolumeChanged(float volume)
        {
            _view.SetMusicVolume(volume);
        }
    }
}