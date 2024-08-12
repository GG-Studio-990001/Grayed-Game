using Runtime.Common.View;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.CH1.Title
{
    public class TitleKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private GameObject _timeline;

        private void Start()
        {
            Managers.Data.LoadGame();
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.TitleKeyBinding(this, _settingsUIView);
        }

        public void ActiveTimeline()
        {
            Managers.Data.SaveGame();
            _timeline.SetActive(true);
        }

        public void GoToMain()
        {
            SceneManager.LoadScene("Main");
        }
    }
}