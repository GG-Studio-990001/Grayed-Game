using Runtime.Common.View;
using UnityEngine;

namespace Runtime.CH1.Title
{
    public class TitleKeyBinder : MonoBehaviour
    {
        [SerializeField]
        private SettingsUIView _settingsUIView;
        [SerializeField]
        private GameObject _timeline;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.TitleKeyBinding(this, _settingsUIView);
        }

        public void ActiveTimeline()
        {
            _timeline.SetActive(true);
        }
    }
}