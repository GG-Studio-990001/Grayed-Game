using Runtime.Common.View;
using UnityEngine;

namespace Runtime.CH2.Main
{
    public class CH2KeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private KeySetting _keySetting;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.CH2KeyBinding(_settingsUIView, _keySetting);
        }
    }
}