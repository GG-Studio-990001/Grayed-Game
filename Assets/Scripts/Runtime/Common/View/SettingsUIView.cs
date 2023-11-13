using Runtime.Common.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Common.View
{
    public class SettingsUIView : MonoBehaviour
    {
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        
        private SettingsUIPresenter _presenter;
        
        private void Start()
        {
            _presenter = PresenterFactory.CreateSettingsUIPresenter(this);

            musicVolumeSlider.onValueChanged.AddListener(OnSliderMusicValueChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSliderSfxValueChanged);
        }

        public virtual void SetViewMusicVolume(float volume)
        {
            musicVolumeSlider.value = volume;
        }
        
        public virtual void SetViewSfxVolume(float volume)
        {
            sfxVolumeSlider.value = volume;
        }
        
        private void OnSliderMusicValueChanged(float value)
        {
            _presenter.SetMusicVolume(value);
        }
        
        private void OnSliderSfxValueChanged(float value)
        {
            _presenter.SetSfxVolume(value);
        }
    }
}