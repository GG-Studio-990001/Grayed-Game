using UnityEngine;
using UnityEngine.InputSystem;
using Runtime.Common.View;
using Runtime.ETC;
using Runtime.Input;

namespace Runtime.CH3.Dancepace
{
    public class DancepaceKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private DPRapley _dPRapley;

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
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
    }
} 