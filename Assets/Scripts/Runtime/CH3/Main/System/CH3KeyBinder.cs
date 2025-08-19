using Runtime.CH2.Main;
using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.Main
{
    public class CH3KeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private LineView _luckyDialogue;
        [SerializeField] private QuaterViewPlayer _player;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();

            Managers.Data.InGameKeyBinder.CH3PlayerKeyBinding(_player);
            Managers.Data.InGameKeyBinder.CH3UIKeyBinding(_settingsUIView, _luckyDialogue);

            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }
    }
}