using Runtime.Common.View;
using Runtime.ETC;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

namespace Runtime.CH2.SuperArio
{
    public class SAKeyBinder : MonoBehaviour
    {
        [field: SerializeField] public LineView _lineView { get; private set; }
        [field: SerializeField] public Ario Ario { get; private set; }
        [field: SerializeField] public ArioStore ArioStore { get; private set; }
        [SerializeField] private SettingsUIView _settingsUIView;

        private bool _isInputDelay;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.SAKeyBinding(this, _settingsUIView, _lineView);

            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        public void PauseKeyInput()
        {
            ArioManager.Instance.PauseKey();
        }

        public void ItemKeyInput()
        {
            Ario.UseInvincibleItem();
        }

        public void RestartSuperArio()
        {
            if (_isInputDelay) return;
            ArioManager.Instance.RestartSuperArio();
        }

        public void CheatKeyInput(InputAction.CallbackContext context)
        {
            if (!ArioManager.Instance.IsPlay) return;
            if (context.performed)
            {
                ArioManager.Instance.AddCheatCoins();
            }
        }

        public void EnterStoreKeyInput(InputAction.CallbackContext context)
        {
            if (ArioManager.Instance.IsPlay || ArioManager.Instance.IsPause || ArioManager.Instance.IsStore ||
                ArioManager.Instance.IsReward)
                return;

            Vector2 moveInput = context.ReadValue<Vector2>();
            if (context.performed)
            {
                if (moveInput.y < 0) // 아래 방향키
                {
                    if (ArioManager.Instance.IsGameOver && !_isInputDelay)
                    {
                        StartCoroutine(InputDelay());
                    }
                }
            }
        }

        private IEnumerator InputDelay()
        {
            _isInputDelay = true;
            Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_14");
            ArioManager.Instance.EnterStoreAnimation();
            yield return new WaitForSeconds(0.75f);
            ArioManager.Instance.EnterStore();
            _isInputDelay = false;
        }

        public void ChangeScreenResolution()
        {
            if (Managers.Data.IsFullscreen)
            {
                // 창 모드로 전환하고 해상도 1280x720로 설정
                Screen.SetResolution(1280, 720, false);
            }
            else
            {
                // 전체 화면 모드로 전환하고 해상도 1920x1080으로 설정
                Screen.SetResolution(1920, 1080, true);
            }
        }
    }
}