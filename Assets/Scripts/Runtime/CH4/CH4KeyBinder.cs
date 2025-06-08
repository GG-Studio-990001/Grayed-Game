using Runtime.Common.View;
using UnityEngine;

namespace Runtime.CH4
{
    public class CH4KeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.CH4KeyBinding(_settingsUIView);
        }
    }
}