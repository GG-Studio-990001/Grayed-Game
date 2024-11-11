using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.SuperArio
{
    public class SAKeyBinder : MonoBehaviour
    {
        [field:SerializeField] public LineView LineView { get; private set; }
        [field:SerializeField] public Ario Ario { get; private set; }
        [SerializeField] private SettingsUIView _settingsUIView;
        
        private bool _canRestart;
    
        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            //Managers.Data.InGameKeyBinder.SAKeyBinding(this, _settingsUIView);
        }
    
        public void ActiveRestart()
        {
            _canRestart = true;
        }
    
        public void RestartSuperArio()
        {
            // 가망이 없을 때만 가능
            if (!_canRestart)
                return;

            // Managers.Sound.StopAllSound();
            //
            // Managers.Data.CH1.IsPacmomPlayed = true;
            // Managers.Data.CH1.IsPacmomCleared = false;
            // Managers.Data.SaveGame();
            //
            // SceneManager.LoadScene("Pacmom");
        }
    }
}
