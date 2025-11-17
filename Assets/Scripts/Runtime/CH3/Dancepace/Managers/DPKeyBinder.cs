using UnityEngine;
using UnityEngine.InputSystem;
using Runtime.Common.View;
using Runtime.ETC;
using Runtime.Input;
using System.Collections.Generic;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace
{
    public class DPKeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private DPRapley _dPRapley;
        [SerializeField] private UIManager _uiManager;

        [Header("Key Bindings (초기값)")]
        public Key upKey = Key.W;
        public Key downKey = Key.S;
        public Key leftKey = Key.A;
        public Key rightKey = Key.D;

        private Dictionary<string, Key> poseKeyBindings;
        private bool isInputEnabled = true;
        private Vector2 lastMoveDirection;

        // 입력 없을 때 타이머 및 기본자세 체크
        private float noInputTimer = 0f;
        private bool isInDefaultPose = false;

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

            Managers.Data.InGameKeyBinder.PlayerInputDisable();
            Managers.Data.InGameKeyBinder.DancepaceKeyBinding(this, _settingsUIView);
            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();

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
            if (!isInputEnabled) 
            {
                return;
            }
            
            // InGameKeyBinder의 Player 입력 상태도 확인
            if (!Managers.Data.InGameKeyBinder.IsPlayerInputEnabled()) 
            {
                return;
            }

            if (context.performed)
            {
                lastMoveDirection = context.ReadValue<Vector2>();
                _dPRapley?.OnMove(context);

                string poseId = GetPoseIdFromVector(lastMoveDirection);
                _uiManager?.UpdateKeyGuide(poseId);

                // 입력이 들어오면 타이머 리셋 및 기본자세 플래그 해제
                noInputTimer = 0f;
                isInDefaultPose = false;
            }
            else if (context.canceled)
            {
                // 입력이 없을 때 마지막 방향을 유지
                var lastContext = new InputAction.CallbackContext();
                lastContext.ReadValue<Vector2>().Set(lastMoveDirection.x, lastMoveDirection.y);
                _dPRapley?.OnMove(lastContext);
                _uiManager?.UpdateKeyGuide(null);

                // 입력 해제 시 타이머 리셋 및 기본자세 플래그 해제
                noInputTimer = 0f;
                isInDefaultPose = false;
            }
        }

        private string GetPoseIdFromVector(Vector2 dir)
        {
            if (dir == Vector2.up) return "Up";
            if (dir == Vector2.down) return "Down";
            if (dir == Vector2.left) return "Left";
            if (dir == Vector2.right) return "Right";
            return null;
        }

        public void OnInteraction()
        {
            if (!isInputEnabled) return;
            
            // InGameKeyBinder의 Player 입력 상태도 확인
            if (!Managers.Data.InGameKeyBinder.IsPlayerInputEnabled()) return;
            
            // 상호작용 키 입력 처리
        }

        public bool IsPoseKeyPressed(string poseId)
        {
            if (!isInputEnabled) return false;
            
            // InGameKeyBinder의 Player 입력 상태도 확인
            if (!Managers.Data.InGameKeyBinder.IsPlayerInputEnabled()) return false;

            if (poseKeyBindings != null && poseKeyBindings.TryGetValue(poseId, out var key))
                return Keyboard.current[key].wasPressedThisFrame;
            return false;
        }

        public bool IsAnyOtherPoseKeyPressed(string exceptPoseId)
        {
            if (!isInputEnabled || poseKeyBindings == null)
                return false;
                
            // InGameKeyBinder의 Player 입력 상태도 확인
            if (!Managers.Data.InGameKeyBinder.IsPlayerInputEnabled()) return false;
            
            foreach (var kvp in poseKeyBindings)
            {
                if (kvp.Key == exceptPoseId) continue;
                if (Keyboard.current[kvp.Value].wasPressedThisFrame)
                    return true;
            }
            return false;
        }

        public bool IsPoseKeyHeld(string poseId)
        {
            if (!isInputEnabled) return false;
            
            // InGameKeyBinder의 Player 입력 상태도 확인
            if (!Managers.Data.InGameKeyBinder.IsPlayerInputEnabled()) return false;
            
            if (poseKeyBindings != null && poseKeyBindings.TryGetValue(poseId, out var key))
                return Keyboard.current[key].isPressed;
            return false;
        }

        public void DisableInput()
        {
            isInputEnabled = false;
            Managers.Sound.PauseAllSound();
            Managers.Data.InGameKeyBinder.PlayerInputDisable();
        }

        public void EnableInput()
        {
            isInputEnabled = true;
            Managers.Sound.UnPauseAllSound();
            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        private void Update()
        {
            if (!isInputEnabled) return;
            
            // InGameKeyBinder의 Player 입력 상태도 확인
            if (!Managers.Data.InGameKeyBinder.IsPlayerInputEnabled()) return;

            // 입력이 없을 때 타이머 증가 (키 입력 상태 체크를 IsPoseKeyHeld로 변경)
            if (!IsPoseKeyHeld("Up") && !IsPoseKeyHeld("Down") &&
                !IsPoseKeyHeld("Left") && !IsPoseKeyHeld("Right"))
            {
                noInputTimer += Time.deltaTime;

                if (!isInDefaultPose && noInputTimer >= 1.5f)
                {
                    // 기본 자세로 복귀
                    _dPRapley?.ResetState();
                    _uiManager?.UpdateKeyGuide(null);
                    isInDefaultPose = true;
                }
            }
            else
            {
                // 키가 눌려있으면 타이머 리셋
                noInputTimer = 0f;
                isInDefaultPose = false;
            }
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