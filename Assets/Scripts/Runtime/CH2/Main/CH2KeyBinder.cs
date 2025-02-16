using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public class CH2KeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private CH2KeySetting _keySetting;
        [SerializeField] private LineView _luckyDialogue;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.CH2KeyBinding(_settingsUIView, _keySetting, _luckyDialogue);
        }
    }
}