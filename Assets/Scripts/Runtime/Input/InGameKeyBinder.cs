using Runtime.CH0;
using Runtime.CH1.Main.Controller;
using Runtime.CH1.Main.Player;
using Runtime.CH1.Pacmom;
using Runtime.CH1.Title;
using Runtime.CH2.Main;
using Runtime.CH2.SuperArio;
using Runtime.CH3;
using Runtime.CH3.Dancepace;
using Runtime.CH4;
using Runtime.Common.View;
using Yarn.Unity;

namespace Runtime.Input
{
    // 현재 플랫폼에 맞게 키 바인딩을 설정하는 클래스
    public class InGameKeyBinder
    {
        private GameOverControls _gameOverControls;
        
        private int _playerInputEnableStack;
        
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
            _gameOverControls.Player.Interaction.performed += _ => player.OnInteraction();
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

        public void CH3PlayerKeyBinding(QuaterViewPlayer player)
        {
            _gameOverControls.Player.Enable();
            _gameOverControls.Player.Move.performed += player.OnMove;
            _gameOverControls.Player.Move.started += player.OnMove;
            _gameOverControls.Player.Move.canceled += player.OnMove;
            _gameOverControls.Player.Interaction.performed += _ => player.OnInteraction();
        }

        public void TrpgKeyBinding(LineView line) // 임시
        {
            _gameOverControls.UI.Enable();
            _gameOverControls.UI.DialogueInput.performed += _ => line.OnContinueClicked();
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
    }
}