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
                Managers.Sound.Play(Sound.BGM, "CH1/Main(Cave)_BGM");
            }
            else if (_chapterNum == 2)
            {
                Managers.Sound.Play(Sound.BGM, "CH2/BGM_06_Micael's Riddle");
            }
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.CH4KeyBinding(_player, _settingsUIView);
        }
    }
}