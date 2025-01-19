using Runtime.Common.View;
using Runtime.InGameSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.CH1.Title
{
    public class TitleKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private SceneSystem _sceneSystem;
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

        public void ExitTitle()
        {
            if (Managers.Data.Chapter < 2)
            {
                Managers.Data.SaveGame();
                _timeline.SetActive(true);
            }
            else
            {
                _sceneSystem.LoadSceneWithFade($"CH{Managers.Data.Chapter}");
            }
        }

        public void GoToMain()
        {
            SceneManager.LoadScene("Main");
        }
    }
}