using Runtime.Data;
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
        }
        
        public void InitializeSettings()
        {
            Addressables.LoadAssetAsync<SettingsData>("SettingsData").Completed += OnSettingsDataLoaded;
        }

        public void OnMusicVolumeChanged(float volume)
        {
            _data.SetMusicVolume(volume);
            _view.SetMusicVolume(_data.MusicVolume);
        }
        
        private void OnSettingsDataLoaded(AsyncOperationHandle<SettingsData> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _data = handle.Result;
            }
            else
            {
                Debug.LogError("데이터 로드 오류: " + handle.DebugName);
                // 이후에 서버, 예외처리로 변경
            }
        }
    }
}