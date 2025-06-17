using UnityEngine;
using UnityEngine.InputSystem;
using Runtime.Common.View;
using Runtime.ETC;
using Runtime.Input;
using System.Collections.Generic;

namespace Runtime.CH3.Dancepace
{
    public class DPKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private DPRapley _dPRapley;

        [Header("Key Bindings (초기값)")]
        public Key upKey = Key.W;
        public Key downKey = Key.S;
        public Key leftKey = Key.A;
        public Key rightKey = Key.D;

        private Dictionary<string, Key> poseKeyBindings;
        private bool isInputEnabled = true;
        private Vector2 lastMoveDirection;

        private void Start()
        {
            InitializeKeyBindings();
            SetupSettingsCallbacks();
        }

        private void InitializeKeyBindings()
        {
            poseKeyBindings = new Dictionary<string, Key>
            {
                { "Up", upKey },
                { "Down", downKey },
                { "Left", leftKey },
                { "Right", rightKey }
            };

            Managers.Data.InGameKeyBinder.GameControlReset();
            Managers.Data.InGameKeyBinder.DancepaceKeyBinding(this, _settingsUIView);

            if (_dPRapley == null)
            {
                Debug.LogError("DPRapley reference is missing!");
            }
        }

        private void SetupSettingsCallbacks()
        {
            _settingsUIView.OnSettingsOpen += DisableInput;
            _settingsUIView.OnSettingsClose += EnableInput;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!isInputEnabled) return;

            if (context.performed)
            {
                lastMoveDirection = context.ReadValue<Vector2>();
                _dPRapley?.OnMove(context);
            }
            else if (context.canceled)
            {
                // 입력이 없을 때 마지막 방향을 유지
                var lastContext = new InputAction.CallbackContext();
                lastContext.ReadValue<Vector2>().Set(lastMoveDirection.x, lastMoveDirection.y);
                _dPRapley?.OnMove(lastContext);
            }
        }

        public void OnInteraction()
        {
            if (!isInputEnabled) return;
            // 상호작용 키 입력 처리
        }

        public bool IsPoseKeyPressed(string poseId)
        {
            if (!isInputEnabled) return false;
            
            if (poseKeyBindings != null && poseKeyBindings.TryGetValue(poseId, out var key))
                return Keyboard.current[key].wasPressedThisFrame;
            return false;
        }

        public void DisableInput()
        {
            isInputEnabled = false;
            Managers.Data.InGameKeyBinder.PlayerInputDisable();
        }

        public void EnableInput()
        {
            isInputEnabled = true;
            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        private void OnDestroy()
        {
            if (_settingsUIView != null)
            {
                _settingsUIView.OnSettingsOpen -= DisableInput;
                _settingsUIView.OnSettingsClose -= EnableInput;
            }
        }
    }
} 