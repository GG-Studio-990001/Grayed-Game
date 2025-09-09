using Runtime.CH0;
using Runtime.CH1.Main.Controller;
using Runtime.CH1.Main.Player;
using Runtime.CH1.Pacmom;
using Runtime.CH1.Title;
using Runtime.CH2.Main;
using Runtime.CH2.SuperArio;
using Runtime.CH3;
using Runtime.CH3.Dancepace;
using Runtime.CH3.TRPG;
using Runtime.CH4;
using Runtime.Common.View;
using Yarn.Unity;
using UnityEngine.InputSystem;

namespace Runtime.Input
{
    // 현재 플랫폼에 맞게 키 바인딩을 설정하는 클래스
    public class InGameKeyBinder
    {
        private GameOverControls _gameOverControls;
        
        private int _playerInputEnableStack;
        
        // CH3 Inventory bindings cache
        private bool _ch3InventoryBound;
        
        public InGameKeyBinder(GameOverControls gameOverControls)
        {
            _gameOverControls = gameOverControls;
        }
        
        // Title
        public void TitleKeyBinding(TitleKeyBinder keyBinder, SettingsUIView settingsUIView)
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.DialogueInput.performed += _ => keyBinder.ExitTitle();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
            _gameOverControls.UI.Cheat.performed += _ => keyBinder.GoToMain();
        }

        // Pacmom
        public void PMKeyBinding(PMKeyBinder keyBinder, SettingsUIView settingsUIView)
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
            _gameOverControls.UI.DialogueInput.performed += _ => keyBinder.LineView.OnContinueClicked();
            _gameOverControls.UI.Restart.performed += _ => keyBinder.RestartPacmom();

            _gameOverControls.Player.Enable();
            _gameOverControls.Player.Move.performed += keyBinder.Rapley.OnMove;
            _gameOverControls.Player.Move.started += keyBinder.Rapley.OnMove;
            _gameOverControls.Player.Move.canceled += keyBinder.Rapley.OnMove;
        }
        
        // SuperArio
        public void SAKeyBinding(SAKeyBinder keyBinder, SettingsUIView settingsUIView, LineView line)
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
            _gameOverControls.UI.GameSetting.performed += _ => keyBinder.PauseKeyInput();
            _gameOverControls.UI.Restart.performed += _ => keyBinder.RestartSuperArio();
            _gameOverControls.UI.ChangeScreen.performed += _ => keyBinder.ChangeScreenResolution();
            _gameOverControls.UI.Cheat.performed += keyBinder.CheatKeyInput;
            _gameOverControls.UI.DialogueInput.performed += _ => line.OnContinueClicked();

            _gameOverControls.Player.Enable();
            _gameOverControls.Player.Move.performed += keyBinder.EnterStoreKeyInput;
            _gameOverControls.Player.Move.performed += keyBinder.Ario.OnMove;
            _gameOverControls.Player.Move.canceled += keyBinder.Ario.OnMove;

            _gameOverControls.Player.Move.started += keyBinder.ArioStore.OnMove;
            _gameOverControls.Player.Move.performed += keyBinder.ArioStore.OnMove;
            _gameOverControls.Player.Move.canceled += keyBinder.ArioStore.OnMove;
            _gameOverControls.Player.Interaction.performed += _ => keyBinder.ItemKeyInput();
        }

        // CH0
        public void CH0UIKeyBinding(SettingsUIView settingsUIView, LineView line)
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
            _gameOverControls.UI.DialogueInput.performed += _ => line.OnContinueClicked();
        }

        // CH1
        public void CH1PlayerKeyBinding(TopDownPlayer player)
        {
            _gameOverControls.Player.Enable();
            _gameOverControls.Player.Move.performed += player.OnMove;
            _gameOverControls.Player.Move.started += player.OnMove;
            _gameOverControls.Player.Move.canceled += player.OnMove;
            // CH3: 스페이스(Interaction) 연타 채집 비활성화 - 즉시 상호작용 연결 제거
        }
        
        public void CH1UIKeyBinding(Ch1MainSystemController controller, LineView line)
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => controller.GameSettingToggle();
            _gameOverControls.UI.DialogueInput.performed += _ => line.OnContinueClicked();
        }

        // CH2
        public void CH2KeyBinding(SettingsUIView settingsUIView, CH2KeySetting keySetting, LineView line)
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
            _gameOverControls.UI.DialogueInput.performed += _ => keySetting.DialogueInput();
            _gameOverControls.UI.Hide.performed += _ => keySetting.HideUIToggle();
            // _gameOverControls.UI.Auto.performed += _ => keySetting.AutoDialogue();
            _gameOverControls.UI.DialogueInput.performed += _ => line.OnContinueClicked();
        }
        
        public void CH3UIKeyBinding(SettingsUIView settingsUIView, LineView line)
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
            //_gameOverControls.UI.DialogueInput.performed += _ => line.OnContinueClicked();
        }

        public void CH3PlayerKeyBinding(QuaterViewPlayer player, Runtime.CH3.Main.CH3KeyBinder binder)
        {
            _gameOverControls.Player.Enable();
            _gameOverControls.Player.Move.performed += player.OnMove;
            _gameOverControls.Player.Move.started += player.OnMove;
            _gameOverControls.Player.Move.canceled += player.OnMove;
            _gameOverControls.Player.Interaction.performed += _ => player.OnInteraction();

            // 아래는 CH3 전용 인벤토리/단축바 바인딩을 동일 함수 내에서 처리
            if (!_ch3InventoryBound)
            {
                _ch3InventoryBound = true;

                // 숫자 1~7 선택: 하나의 Hotbar 액션에 모두 묶여 있으므로 눌린 키를 판별
                _gameOverControls.Player.Hotbar.performed += _ =>
                {
                    var keyboard = Keyboard.current;
                    int selectedIndex = -1;
                    if (keyboard.digit1Key.wasPressedThisFrame) selectedIndex = 0;
                    else if (keyboard.digit2Key.wasPressedThisFrame) selectedIndex = 1;
                    else if (keyboard.digit3Key.wasPressedThisFrame) selectedIndex = 2;
                    else if (keyboard.digit4Key.wasPressedThisFrame) selectedIndex = 3;
                    else if (keyboard.digit5Key.wasPressedThisFrame) selectedIndex = 4;
                    else if (keyboard.digit6Key.wasPressedThisFrame) selectedIndex = 5;
                    else if (keyboard.digit7Key.wasPressedThisFrame) selectedIndex = 6;
                    if (selectedIndex >= 0)
                        binder.HotbarSelect(selectedIndex);
                };

                // 인벤토리 토글(I)
                _gameOverControls.Player.InvetoryToggle.performed += _ => binder.InventoryToggle();

                // 사용(E)
                _gameOverControls.Player.HotbarUse.performed += _ => binder.HotbarUse();
            }
        }

        public void TrpgKeyBinding(SettingsUIView settingsUIView, TrpgDialogue trpgDialogue) // 임시
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
            _gameOverControls.UI.DialogueInput.performed += _ => trpgDialogue.ContinueDialogue();
        }

        public void DancepaceKeyBinding(DPKeyBinder keyBinder, SettingsUIView settingsUIView)
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
            //_gameOverControls.UI.GameSetting.performed += _ => keyBinder.PauseKeyInput();
            
            _gameOverControls.Player.Enable();
            _gameOverControls.Player.Move.performed += keyBinder.OnMove;
            _gameOverControls.Player.Move.started += keyBinder.OnMove;
            _gameOverControls.Player.Move.canceled += keyBinder.OnMove;
            _gameOverControls.Player.Interaction.performed += _ => keyBinder.OnInteraction();
        }

        // CH4
        public void CH4KeyBinding(PlatformerPlayer player, SettingsUIView settingsUIView)
        {
            _gameOverControls.Player.Enable();
            _gameOverControls.Player.Move.performed += player.OnMove;
            // _gameOverControls.Player.Move.started += player.OnMove; // 필요 X
            _gameOverControls.Player.Move.canceled += player.OnMove;

            _gameOverControls.Player.Jump.performed += player.OnJump;
            _gameOverControls.Player.Interaction.performed += _ => player.OnInteraction();

            _gameOverControls.UI.Enable();
            _gameOverControls.UI.GameSetting.performed += _ => settingsUIView.GameSettingToggle();
        }

        // ETC
        public void GameControlReset()
        {
            _gameOverControls.Dispose();
            _gameOverControls = new GameOverControls();

            _playerInputEnableStack = 0;
        }

        public void PlayerInputDisable()
        {
            _playerInputEnableStack++;
            
            if (_playerInputEnableStack > 0)
            {
                _gameOverControls.Player.Disable();
            }
        }
        
        public void PlayerInputEnable()
        {
            if (_playerInputEnableStack > 0)
            {
                _playerInputEnableStack--;
            }
            
            if (_playerInputEnableStack == 0)
                _gameOverControls.Player.Enable();
        }
        
        public bool IsPlayerInputEnabled()
        {
            return _playerInputEnableStack == 0;
        }
    }
}