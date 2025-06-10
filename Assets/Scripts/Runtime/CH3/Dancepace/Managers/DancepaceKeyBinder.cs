using UnityEngine;
using UnityEngine.InputSystem;
using Runtime.Common.View;
using Runtime.ETC;
using Runtime.Input;
using System.Collections.Generic;

namespace Runtime.CH3.Dancepace
{
    public class DancepaceKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private DPRapley _dPRapley;

        [Header("Key Bindings (초기값)")]
        public Key upKey = Key.W;
        public Key downKey = Key.S;
        public Key leftKey = Key.A;
        public Key rightKey = Key.D;

        private Dictionary<string, Key> poseKeyBindings;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
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

            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _dPRapley?.OnMove(context);
        }

        public void OnInteraction()
        {
            // 상호작용 키 입력 처리
            // TODO: 필요한 경우 구현
        }

        public bool IsPoseKeyPressed(string poseId)
        {
            if (poseKeyBindings != null && poseKeyBindings.TryGetValue(poseId, out var key))
                return Keyboard.current[key].wasPressedThisFrame;
            return false;
        }
    }
} 