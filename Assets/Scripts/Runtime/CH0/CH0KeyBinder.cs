using Runtime.CH1.Main.Player;
using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH0
{
    public class CH0KeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private TopDownPlayer _player;
        [SerializeField] private LineView _luckyDialogue;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();

            Managers.Data.InGameKeyBinder.CH1PlayerKeyBinding(_player);
            Managers.Data.InGameKeyBinder.CH0UIKeyBinding(_settingsUIView, _luckyDialogue);

            /*
            _ch1DialogueController.OnDialogueStart.AddListener(() => {
                Managers.Data.InGameKeyBinder.PlayerInputDisable();
                _player.PlayerIdle();
            });
            _ch1DialogueController.OnDialogueEnd.AddListener(() => Managers.Data.InGameKeyBinder.PlayerInputEnable());
            */

            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }
    }
}