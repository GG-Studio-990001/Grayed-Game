using Runtime.Common.View;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH2.SuperArio
{
    public class SAKeyBinder : MonoBehaviour
    {
        //[field:SerializeField] public LineView LineView { get; private set; }
        [field:SerializeField] public Ario Ario { get; private set; }
        [field:SerializeField] public ArioStore ArioStore { get; private set; }
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
            ArioManager.instance.PauseKey();
        }

        public void ItemKeyInput()
        {
            Ario.UseInvincibleItem();
        }
        
        public void RestartSuperArio()
        {
            ArioManager.instance.RestartSuperArio();
        }

        public void EnterStoreKeyInput(InputAction.CallbackContext context)
        {
            if (ArioManager.instance.IsPlay || ArioManager.instance.IsPause || ArioManager.instance.IsStore || ArioManager.instance.IsReward)
                return;

            Vector2 moveInput = context.ReadValue<Vector2>();
            if (context.performed)
            { 
                if (moveInput.y < 0) // 아래 방향키
                {
                    ArioManager.instance.EnterStore();
                }
            }
        }
    }
}
