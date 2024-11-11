using Runtime.Common.View;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class SAKeyBinder : MonoBehaviour
    {
        //[field:SerializeField] public LineView LineView { get; private set; }
        [field:SerializeField] public Ario Ario { get; private set; }
        [SerializeField] private SettingsUIView _settingsUIView;
    
        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.SAKeyBinding(this, _settingsUIView);
        }

        public void PauseKeyInput()
        {
            Ario.PauseKeyInput();
        }
        
        public void RestartSuperArio()
        {
            ArioManager.instance.RestartSuperArio();
        }
    }
}
