using System;
using UnityEngine;

namespace Runtime.UI.Settings
{
    public class SettingsUIView : MonoBehaviour
    {
        private SettingsUIPresenter _presenter;

        private void Start()
        {
            _presenter = PresenterFactory.CreateSettingsUIPresenter(this);
            
            _presenter.OnViewCreated();
        }

        public void SetMusicVolume(float volume)
        {
            
        }
    }
}