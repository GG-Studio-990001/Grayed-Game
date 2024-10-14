using Runtime.CH1.Main.Dialogue;
using Runtime.CH1.Main.Npc;
using Runtime.CH1.Main.Player;
using Runtime.CH1.Main.Stage;
using Runtime.Common.View;
using Runtime.ETC;
using Runtime.InGameSystem;
using System;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Main.Controller
{
    public class Ch1MainSystemController : MonoBehaviour
    {
        [Header("System")]
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private Ch1StageController _ch1StageController;
        [SerializeField] private Ch1DialogueController _ch1DialogueController;
        [SerializeField] private FadeController _fadeController;
        
        [Header("Player")]
        [SerializeField] private TopDownPlayer _player;

        [Header("Init")] // 맵에서 초기화해줘야 하는 것 / 클래스 따로 빼기
        [SerializeField] private NpcPosition _npcPosition;
        [SerializeField] private LuckyPack _luckyPack;
        [SerializeField] private Bridge _bridge;
        [SerializeField] private Mamago _mamago;
        [SerializeField] private MamagoBubble _mamagoBubble;
        [SerializeField] private LineView _luckyDialogue;
        [NonSerialized] public bool IsLuckyOn = false;

        private void Start()
        {
            LoadGame();
            GameKeyBinding();
            GameInit();
            SetMap();
            SetPlayerPosition();
        }

        private void SetPlayerPosition()
        {
            // 진행도에 따른 플레이어 위치 재지정
            // TO DO: 클래스 따로 빼기?

            switch(Managers.Data.CH1.Stage)
            {
                case 1:
                    if (Managers.Data.CH1.Scene > 1)
                    {
                        // 팩맘 앞
                        _player.transform.position = new Vector3(22.07f, -6.94f, 0);
                    }
                    break;
                case 2:
                    if ((Managers.Data.CH1.Scene == 3 && Managers.Data.CH1.SceneDetail == 1) || Managers.Data.CH1.Scene > 3)
                    {
                        // 3Match 끝냈으면 오른쪽에 위치
                        _player.transform.position = new Vector3(50.4f, -11.6f, 0);
                    }
                    break;
                case 3:
                    break;
            }
        }

        private void SetMap()
        {
            // TODO: Check나 Load로 용어 통일
            _npcPosition.LoadNpcPosition();
            _luckyPack.ActiveLuckyPack();
            _mamago.CheckMamago();
            _mamagoBubble.CheckMamagoBubble();
        }

        // 각 컨트롤러 초기화
        private void GameInit()
        {
            _ch1StageController.Init(_fadeController, _player.transform);
        }

        // 인게임에 사용되는 키 이벤트 바인딩
        private void GameKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
            
            Managers.Data.InGameKeyBinder.CH1PlayerKeyBinding(_player);
            Managers.Data.InGameKeyBinder.CH1UIKeyBinding(this, _luckyDialogue);

            _ch1DialogueController.OnDialogueStart.AddListener(() => {
                Managers.Data.InGameKeyBinder.PlayerInputDisable();
                _player.PlayerIdle();
            });
            _ch1DialogueController.OnDialogueEnd.AddListener(() => Managers.Data.InGameKeyBinder.PlayerInputEnable());

            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        public void GameSettingToggle()
        {
            if (IsLuckyOn)
                return;

            _settingsUIView.GameSettingToggle();
        }

        private void LoadGame()
        {
            Managers.Data.LoadGame();
        }
    }
}