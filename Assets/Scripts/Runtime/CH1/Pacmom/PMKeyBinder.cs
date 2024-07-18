using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Pacmom
{
    public class PMKeyBinder : MonoBehaviour
    {
        [SerializeField]
        private SettingsUIView _settingsUIView;
        [SerializeField]
        private LineView _lineView;
        [SerializeField]
        private Rapley _rapley;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.PMKeyBinding(_settingsUIView, _lineView, _rapley);
        }
    }
}