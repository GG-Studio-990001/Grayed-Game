using Runtime.Common.Presentation;
using Runtime.Event;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common.View
{
    public class SettingsUIView : MonoBehaviour
    {
        [field:SerializeField] public GameObject SettingUIObject { get; set; }
        [field:SerializeField] public Slider BgmVolumeSlider { get; set; }
        [field:SerializeField] public Slider SfxVolumeSlider { get; set; }
        [field:SerializeField] public Button ExitButton { get; set; }
        [field:SerializeField] public Button GameExitButton { get; set; }
        
        [field:SerializeField] public Toggle FullScreenToggle { get; set; }
        
        [field:SerializeField] public Toggle WindowScreenToggle { get; set; }

        
        private SettingsUIPresenter _presenter;

        public event Action OnSettingsOpen;
        public event Action OnSettingsClose;
        private Vector2 _lastResolution;
        
        private void Awake()
        {
            _lastResolution = new Vector2(Screen.width, Screen.height);
        }
        
        private void Start()
        {
            _presenter = PresenterFactory.CreateSettingsUIPresenter(this);
            UpdateScreenModeToggles();
        }
        
        private void Update()
        {
            // 해상도가 변경되었는지 체크
            if (_lastResolution.x != Screen.width || _lastResolution.y != Screen.height)
            {
                // 해상도가 변경됨
                if (SettingUIObject.activeSelf)
                {
                    ClosePopup();
                }

                // 토글 상태 업데이트
                UpdateScreenModeToggles();
        
                _lastResolution = new Vector2(Screen.width, Screen.height);
            }
        }

        private void UpdateScreenModeToggles()
        {
            if (FullScreenToggle != null && WindowScreenToggle != null)
            {
                bool isFullScreen = Screen.fullScreen;
        
                // SetIsOnWithoutNotify로 토글 상태만 변경
                FullScreenToggle.SetIsOnWithoutNotify(isFullScreen);
                WindowScreenToggle.SetIsOnWithoutNotify(!isFullScreen);
        
                // 상호작용 가능 상태 설정
                FullScreenToggle.interactable = !isFullScreen;
                WindowScreenToggle.interactable = isFullScreen;
        
                // Data Manager 업데이트
                Managers.Data.IsFullscreen = isFullScreen;
            }
        }
        
        public void ClosePopup()
        {
            if (!SettingUIObject.activeSelf) return;
    
            OnSettingsClose?.Invoke();
            Time.timeScale = 1;
            SettingsEvent.ToggleSettings(false);
            SettingUIObject.SetActive(false);
        }

        public void GameSettingToggle()
        {
            if (SettingUIObject.activeSelf)
            {
                ClosePopup();
            }
            else
            {
                OnSettingsOpen?.Invoke();
                Time.timeScale = 0;
                SettingsEvent.ToggleSettings(true);
                SettingUIObject.SetActive(true);
            }
        }
    }
}