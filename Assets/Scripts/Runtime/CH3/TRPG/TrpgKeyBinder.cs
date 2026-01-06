using Runtime.Common.View;
using UnityEngine;
using Runtime.ETC;

namespace Runtime.CH3.TRPG
{
    public class TrpgKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private TrpgDialogue _trpgDialogue;

        private void Start()
        {
            InitKeyBinding();
            Managers.Sound.Play(Sound.BGM, "CH3/CH3_Field_Main_BGM.wav");
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.TrpgKeyBinding(_settingsUIView, _trpgDialogue);
        }
    }
}