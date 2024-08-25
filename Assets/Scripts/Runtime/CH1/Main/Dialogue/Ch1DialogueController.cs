using Runtime.ETC;
using Runtime.InGameSystem;
using Runtime.Middle;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Yarn.Unity;
using Sound = Runtime.ETC.Sound;

namespace Runtime.CH1.Main.Dialogue
{
    // CH1 대화 컨트롤러 Yarn Spinner를 사용하여 대화를 관리하는 클래스
    public class Ch1DialogueController : DialogueViewBase
    {
        private CutSceneDialogue _cutScene;
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private SLGActionComponent SLGAction;
        [SerializeField] private NpcDialogueController _npcDialogue;
        [SerializeField] private FadeController _fadeController;
        [SerializeField] private ConnectionController _connectionController;
        [Header("=Else=")]
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private CanvasGroup _lineViewCanvas;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        private string _speaker;
        private bool _isR2MonSpeaking = false;

        public UnityEvent OnDialogueStart => _runner.onDialogueStart;
        public UnityEvent OnDialogueEnd => _runner.onDialogueComplete;

        private void Awake()
        {
            _cutScene = GetComponent<CutSceneDialogue>();
            _runner.AddCommandHandler("GetSLGPack", SLGAction.GetSLGPack);
            _runner.AddCommandHandler("InitSLG", SLGAction.OnSLGInit);
            _runner.AddCommandHandler<int>("NpcDialogueFin", _npcDialogue.NpcDialogueFin);

            _runner.AddCommandHandler("FadeIn", _fadeController.StartFadeIn);
            _runner.AddCommandHandler("FadeOut", _fadeController.StartFadeOut);

            // _runner.AddCommandHandler<string>("StartTimeline", (timelineName) => _timelineController.PlayTimeline(timelineName));        
            _runner.AddCommandHandler<string>("SceneChange", SceneChange);
            _runner.AddCommandHandler("NewSceneStart", NewSceneStart);
            _runner.AddCommandHandler("NextSceneStart", NextSceneStart);
            _runner.AddCommandHandler("SceneEnd", SceneEnd);
            _runner.AddCommandHandler("ReverseConnection", _connectionController.ReverseConnection);

            _runner.AddCommandHandler("ClearLineText", ClearLineText);

            // CutScene
            _runner.AddCommandHandler("SetSpeakerR2Mon", SetSpeakerR2Mon);
            _runner.AddCommandHandler<int>("ShowIllustration", _cutScene.ShowIllustration);
            _runner.AddCommandHandler<int>("CharactersMove", _cutScene.CharactersMove);
            _runner.AddCommandHandler<int>("CharactersStop", _cutScene.CharactersStop);
            _runner.AddCommandHandler<int>("NpcJump", _cutScene.NpcJump);
            _runner.AddCommandHandler("GetLucky", _cutScene.GetLucky);
            _runner.AddCommandHandler("MeetLucky", _cutScene.MeetLucky);
            _runner.AddCommandHandler<bool>("ShakeMap", _cutScene.ShakeMap);
            _runner.AddCommandHandler("BreakBridge", _cutScene.BreakBridge);
            _runner.AddCommandHandler<int>("SetNpcPosition", _cutScene.SetNpcPosition);
            // CutScene / Michael
            _runner.AddCommandHandler<int>("MichaelAction", _cutScene.MichaelAction);
            _runner.AddCommandHandler("DallarRun", _cutScene.DallarRun);
            _runner.AddCommandHandler("NpcsMove", _cutScene.NpcsMove);
            _runner.AddCommandHandler("Scene4End", _cutScene.Scene4End);
            // CutScene / Mamago
            _runner.AddCommandHandler("RebuildBridge", _cutScene.RebuildBridge);
            _runner.AddCommandHandler("BuildCompany", _cutScene.BuildCompany);
            _runner.AddCommandHandler("EndMainConstruction", _cutScene.EndMainConstruction);
            _runner.AddCommandHandler("EndSLGMode", _cutScene.EndSLGMode);
            _runner.AddCommandHandler("CompleteSFX", _cutScene.CompleteSFX);
            _runner.AddCommandHandler("MamagoJump", _cutScene.MamagoJump);
            _runner.AddCommandHandler("MamagoMove1", _cutScene.MamagoMove1);
            _runner.AddCommandHandler("MamagoMove2", _cutScene.MamagoMove2);
            _runner.AddCommandHandler("MamagoEnter", _cutScene.MamagoEnter);
            // CutScene / R2mon
            _runner.AddCommandHandler("SetR2MonPosition", _cutScene.SetR2MonPosition);
        }

        private void Update()
        {
            if (_lineViewCanvas.alpha == 0)
                ClearLineText();
        }

        private void ClearLineText()
        {
            _lineTxt.text = "";
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _speaker = dialogueLine.CharacterName;
            SetNameTag(_speaker != "");

            CheckR2MonSpeaking();

            onDialogueLineFinished();
        }

        private void SetNameTag(bool hasName)
        {
            _nameTag.SetActive(hasName);
        }

        private void Start()
        {
            if (Managers.Data.Scene == 0)
            {
                _runner.StartDialogue("S1");
            }
            else if (Managers.Data.Scene == 1 && Managers.Data.IsPacmomCleared)
            {
                _cutScene.NpcPos.SetNpcPosition(3);
                _cutScene.SetPlayerPosition(1);
                _runner.StartDialogue("S2");
            }
        }

        public void StartCh1MainDialogue(string startNode)
        {
            // Runner 가 여러개가 되어 메인Runner 실행 함수 따로 생성
            if(_runner != null)
            {
                _runner.StartDialogue(startNode);
            }
        }

        public void MamagoThanks()
        {
            if (Managers.Data.Scene == 4)
            {
                _runner.StartDialogue("S5");
            }
        }

        private void NewSceneStart()
        {
            Managers.Data.Scene++;
            Managers.Data.SceneDetail = 0;
            _cutScene.Player.IsDirecting = true;
        }

        private void NextSceneStart()
        {
            Managers.Data.SceneDetail++;
            _cutScene.Player.IsDirecting = true;
        }

        private void SceneEnd()
        {
            _cutScene.Player.IsDirecting = false;
            Managers.Data.SaveGame();
            Debug.Log(Managers.Data.Scene + " " + Managers.Data.SceneDetail);
        }

        private void SceneChange(string sceneName)
        {
            // 접속
            _connectionController.ConnectScene(sceneName);
        }

        public void SetDialogueData(string value)
        {
            var variableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();

            if (value == "ThreeMatchPuzzle")
            {
                //variableStorage.TryGetValue("$ThreeMatchPuzzle", out lvalue);
                variableStorage.SetValue("$ThreeMatchPuzzle", true);
            }
        }

        // TODO: 달러와 파머의 음성도 각각 생긴다면 방식 변경할 듯
        private void CheckR2MonSpeaking()
        {
            if (_isR2MonSpeaking)
                _speaker = "R2-Mon";
            _isR2MonSpeaking = false;
        }

        private void SetSpeakerR2Mon()
        {
            _isR2MonSpeaking = true;
        }

        public void TextSFX()
        {
            if (_speaker == "R2-Mon")
            {
                Managers.Sound.Play(Sound.Speech, "R2-Mon_Text_SFX_04");
            }
            else
            {
                Managers.Sound.Play(Sound.Speech, "Text_SFX");
            }
        }

        /*
        T Fetch<T>(string spriteName) where T : UnityEngine.Object
        {
            if (typeof(T) == typeof(Sprite))
            {
                foreach (var sprite in Sprites)
                {
                    if (sprite.name == spriteName)
                    {
                        return sprite as T;
                    }
                }
            }

            return null;
        }
        */
    }
}
