using Runtime.Common.View;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace Runtime.CH1.Pacmom
{
    public class PMKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [field:SerializeField] public LineView LineView { get; private set; }
        [field: SerializeField] public Rapley Rapley { get; private set; }
        private bool _canRestart = false;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.PMKeyBinding(this, _settingsUIView);
        }

        public void ActiveRestart()
        {
            _canRestart = true;
        }

        public void RestartPacmom()
        {
            // 가망이 없을 때만 가능
            if (!_canRestart)
                return;

            Managers.Sound.StopAllSound();

            Managers.Data.IsPacmomPlayed = true;
            Managers.Data.IsPacmomCleared = false;
            Managers.Data.SaveGame();

            SceneManager.LoadScene("Pacmom");
        }
    }
}