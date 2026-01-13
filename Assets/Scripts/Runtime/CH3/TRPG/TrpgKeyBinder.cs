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
            Managers.Sound.Play(Sound.BGM, "CH3/CoC/CH3_CoC_Main_BGM");
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.TrpgKeyBinding(_settingsUIView, _trpgDialogue);
        }
    }
}