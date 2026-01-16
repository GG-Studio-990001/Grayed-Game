using Runtime.Common.View;
using UnityEngine;
using Runtime.ETC;

namespace Runtime.CH4
{
    public class CH4KeyBinder : MonoBehaviour
    {
        [SerializeField] private PlatformerPlayer _player;
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private int _chapterNum;

        private void Start()
        {
            InitKeyBinding();

            if (_chapterNum == 1)
            {
                Managers.Sound.Play(Sound.BGM, "CH4/CH4_BGM_Floyard's Crying");
            }
            else if (_chapterNum == 2)
            {
                Managers.Sound.Play(Sound.BGM, "CH4/CH4_BGM_CH2_Main");
            }
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.CH4KeyBinding(_player, _settingsUIView);
        }
    }
}