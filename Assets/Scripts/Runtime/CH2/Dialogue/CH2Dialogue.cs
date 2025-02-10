using Runtime.CH2.Location;
using Runtime.CH2.Main;
using Runtime.CH2.Tcg;
using Runtime.ETC;
using Runtime.Middle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Runtime.CH2.Dialogue
{
    public class CH2Dialogue : DialogueViewBase
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private TurnController _turnController;
        [SerializeField] private LocationUIController _locationUiController;
        [SerializeField] private TcgController _tcgController;
        [SerializeField] private ConnectionController _connectionController;
        [SerializeField] private CharacterManager _characterManager;
        [Header("=Else=")]
        [SerializeField] private CanvasGroup _lineViewCanvas;
        [SerializeField] private LineView _lineView;
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private FaceSpriteSwitcher _michael;
        [SerializeField] private GameObject _toBeContinued;
        [SerializeField] private GameObject _autoTxt;
        [SerializeField] private GameObject _continueBtn;
        [SerializeField] private GameObject _illerstBg;
        [SerializeField] private Image _illerstImg;
        [SerializeField] private Sprite[] _illerstSprs;
        [SerializeField] private GameObject _darkImg;
        [SerializeField] private DialogueRunner _luckyDialogueRunner;
        private string _speaker;
        private bool _isAutoAdvanced = false;
        private Coroutine _autoDialogueCoroutine;

        private void Awake()
        {
            _runner.AddCommandHandler<string>("StartLocation", StartLocation);
            _runner.AddCommandHandler<string>("SetLocation", SetLocation);
            _runner.AddCommandHandler("NextTurn", NextTurn);
            _runner.AddCommandHandler("ShowOptions", ShowOptions);
            _runner.AddCommandHandler("DialogueFin", DialogueFin);
            _runner.AddCommandHandler<int>("ShowIllerstration", ShowIllerstration);
            _runner.AddCommandHandler("HideIllerstration", HideIllerstration);
            _runner.AddCommandHandler("GetTcgPack", GetTcgPack);
            _runner.AddCommandHandler("StartSuperArio", StartSuperArio);
            _runner.AddCommandHandler<string>("ConnectScene", ConnectScene);
            _runner.AddCommandHandler<bool>("SetDarkness", SetDarkness);
            _runner.AddCommandHandler("Ch2End", Ch2End);

            _runner.AddCommandHandler<int>("ChangeBGM", ChangeBGM);
            _runner.AddCommandHandler<int>("PlaySFX", PlaySFX);

            // Character
            _runner.AddCommandHandler<string, string>("SetCharacterPos", _characterManager.SetCharacterPos);
            _runner.AddCommandHandler<string>("HideCharacter", _characterManager.HideCharacter);
            _runner.AddCommandHandler<string, int>("SetCharacterExpression", _characterManager.SetCharacterExpression);

            // TCG
            _runner.AddCommandHandler("StartTcg", _tcgController.StartTcg);
            _runner.AddCommandHandler("StartLastTcg", _tcgController.StartLastTcg);
            _runner.AddCommandHandler("DialogueAfterTCG", _tcgController.DialogueAfterTCG);
            _runner.AddCommandHandler("ShowScore", _tcgController.ShowScore);
            _runner.AddCommandHandler("HideScore", _tcgController.HideScore);
            //_runner.AddCommandHandler<int>("NpcFace", NpcFace);
        }

        #region 기본
        private void Update()
        {
            if (_lineViewCanvas.alpha == 0)
            {
                _lineTxt.text = "";
            }
            // 원래는 새로운 다이얼로그 시작하면 비워주는 용도로 쓴 코드...
            // 알파값 건들일 시 주의
            // TODO: 더 나은 방법 찾기
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _speaker = dialogueLine.CharacterName;

            SetNameTag(_speaker != "");
            _characterManager.HighlightSpeakingCharacter(_speaker);

            onDialogueLineFinished();
        }
        #endregion

        #region 사운드
        private void ChangeBGM(int idx = 1)
        {
            string[] bgmPaths =
            {
                "CH2/BGM_01_Normal",
                "CH2/BGM_02_Serious",
                "CH2/BGM_03_Exciting",
                "CH2/BGM_04_Wariness",
                "CH2/BGM_05_Faint",
                "CH2/BGM_06_Micael's Riddle",
                "CH2/BGM_07_R2IsComing"
            };

            string prefix = $"CH2/BGM_{idx:D2}";

            string path = bgmPaths.FirstOrDefault(bgm => bgm.Substring(0, 10) == prefix);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError($"BGM not found for index {idx}");
                return;
            }

            Managers.Sound.Play(Sound.BGM, path);
        }


        private void PlaySFX(int idx)
        {
            string[] sfxPaths =
            {
                "CH2/SFX_01_Foot_Farmer",
                "CH2/SFX_02_Foot_Gruop",
                "CH2/SFX_03_Foot_Run",
                "CH2/SFX_04_Hit",
                "CH2/SFX_05_R2_Ouch",
                "CH2/SFX_06_Bell",
                "CH2/SFX_07_Build",
                "CH2/SFX_08_Heavens",
                "CH2/SFX_09_Knock_Big",
                "CH2/SFX_10_Door_Stone",
                "CH2/SFX_11_Knock",
                "CH2/SFX_12_slime",
                "CH2/SFX_13_Foot_R2",
            };

            string prefix = $"CH2/SFX_{idx:D2}";

            string path = sfxPaths.FirstOrDefault(sfx => sfx.Substring(0, 10) == prefix);

            Debug.Log($"SFX {path}");
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError($"SFX not found for index {idx}");
                return;
            }

            Managers.Sound.Play(Sound.SFX, path);
        }
        #endregion

        private void ConnectScene(string scene)
        {
            Managers.Sound.StopBGM();
            _connectionController.ConnectScene(scene);
        }

        private void StartSuperArio()
        {
            _connectionController.ConnectScene("SuperArio");
        }

        private void GetTcgPack()
        {
            _luckyDialogueRunner.StartDialogue("LuckyTCG_pack");
        }

        private void ShowIllerstration(int val)
        {
            _illerstBg.SetActive(true);
            _illerstImg.sprite = _illerstSprs[val];
        }

        private void HideIllerstration()
        {
            _illerstBg.SetActive(false);
        }

        private void DialogueFin()
        {
            // 자동진행 끄기
            _isAutoAdvanced = false;
        }

        public void StartLocation(string location)
        {
            _characterManager.HighlightSpeakingCharacter("라플리");

            Managers.Data.CH2.Location = location;
            _locationUiController.StartLocation();
        }

        public void SetLocation(string location)
        {
            Managers.Sound.StopSFX();
            _characterManager.HighlightSpeakingCharacter("라플리");

            Managers.Data.CH2.Location = location;
            _locationUiController.MoveLocation();
        }

        public void NextTurn()
        {
            _turnController.NextTurn();
        }

        private void ShowOptions()
        {
            _characterManager.HighlightSpeakingCharacter("라플리");

            // 이동 가능한 장소 목록을 띄운다
            _turnController.DisplayAvailableLocations();
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

        private void SetNameTag(bool hasName)
        {
            _nameTag.SetActive(hasName);
        }

        private void SetDarkness(bool dark)
        {
            _darkImg.SetActive(dark);
        }

        private void Ch2End()
        {
            Managers.Data.Chapter = 3;
            Managers.Data.SaveGame();
        }

        #region Auto Dialgue
        public void AutoDialogueToggle(bool isAutoAdvanced)
        {
            _isAutoAdvanced = isAutoAdvanced;
            _autoTxt.SetActive(_isAutoAdvanced);

            // 토글 켰을 때 대사가 다 출력된 상태라면 바로 자동진행 시작
            if (_continueBtn.activeSelf && _isAutoAdvanced)
            {
                StartAutoDialogue();
            }
        }

        public void StartAutoDialogue()
        {
            // 자동진행 코루틴 호출
            if (_isAutoAdvanced)
            {
                _autoDialogueCoroutine = StartCoroutine(AutoDialogue());
            }
        }

        private IEnumerator AutoDialogue()
        {
            yield return new WaitForSeconds(1.5f);
            _lineView.OnContinueClicked();
        }

        public void CancelDialogueCoroutine()
        {
            if (_autoDialogueCoroutine is not null)
            {
                StopCoroutine(_autoDialogueCoroutine);
                _autoDialogueCoroutine = null; // 인스턴스 초기화
            }
        }
        #endregion
    }
}