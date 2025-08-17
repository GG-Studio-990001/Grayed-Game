using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.TRPG
{
    public class TrpgKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private TrpgDialogue _trpgDialogue;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.TrpgKeyBinding(_settingsUIView, _trpgDialogue);
        }
    }
}