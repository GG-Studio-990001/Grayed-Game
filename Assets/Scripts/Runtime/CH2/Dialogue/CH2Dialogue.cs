using Runtime.CH2.Location;
using Runtime.CH2.Main;
using Runtime.CH2.Tcg;
using Runtime.ETC;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Runtime.CH2.Dialogue
{
    public enum Npc
    {
        Michael = 0,
        R2mon = 1,
        Dollar = 2,
        Farmer = 3,
        JindoBeagle = 4,
    }

    public class CH2Dialogue : DialogueViewBase
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private TurnController _turnController;
        [SerializeField] private LocationUIController _locationUiController;
        [SerializeField] private TcgController _tcgController;
        [Header("=Else=")]
        [SerializeField] private CanvasGroup _lineViewCanvas;
        [SerializeField] private LineView _lineView;
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private Image[] _characters = new Image[2];
        [SerializeField] private Image[] _npcs = new Image[5];
        [SerializeField] private FaceSpriteSwitcher _michael;
        [SerializeField] private GameObject _toBeContinued;
        [SerializeField] private GameObject _autoTxt;
        [SerializeField] private GameObject _continueBtn;
        [SerializeField] private GameObject _illerstBg;
        [SerializeField] private Image _illerstImg;
        [SerializeField] private Sprite[] _illerstSprs;
        [SerializeField] private DialogueRunner _luckyDialogueRunner;
        [SerializeField] private GameObject _tcgPack;
        [SerializeField] private GameObject _SuperArioPack;
        private string _speaker;
        private bool _isAutoAdvanced = false;
        private Coroutine _autoDialogueCoroutine;

        private void Awake()
        {
            _runner.AddCommandHandler<string>("StartLocation", StartLocation);
            _runner.AddCommandHandler<string>("SetLocation", SetLocation);
            _runner.AddCommandHandler("NextTurn", NextTurn);
            _runner.AddCommandHandler("ShowOptions", ShowOptions);
            _runner.AddCommandHandler<string>("PartnerAppear", PartnerAppear);
            _runner.AddCommandHandler("PartnerOut", PartnerOut);
            _runner.AddCommandHandler("DialogueFin", DialogueFin);
            _runner.AddCommandHandler("StartTcg", _tcgController.StartTcg);
            _runner.AddCommandHandler<int>("ShowIllerstration", ShowIllerstration);
            _runner.AddCommandHandler("HideIllerstration", HideIllerstration);
            _runner.AddCommandHandler("SetTcgPack", SetTcgPack);
            _runner.AddCommandHandler("SetSuperArioPack", SetSuperArioPack);
            _runner.AddCommandHandler<int>("ChangeBGM", ChangeBGM);

            // Yarn Spinner 함수 등록
            _runner.AddCommandHandler("DialogueAfterTCG", _tcgController.DialogueAfterTCG);
            _runner.AddCommandHandler("ShowScore", _tcgController.ShowScore);
            _runner.AddCommandHandler("HideScore", _tcgController.HideScore);
            //_runner.AddCommandHandler("Ending", Ending);
            //_runner.AddCommandHandler<int>("NpcFace", NpcFace);
        }

        #region 기본
        private void Update()
        {
            if (_lineViewCanvas.alpha == 0)
                ClearLineText();
            // 원래는 새로운 다이얼로그 시작하면 비워주는 용도로 쓴 코드...
            // 알파값 건들일 시 주의
        }
        
        private void ClearLineText()
        {
            _lineTxt.text = "";
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _speaker = dialogueLine.CharacterName;
            SetNameTag(_speaker != "");

            if (_speaker.Equals("라플리"))
                StandingHighlight(0);
            else if (_speaker.Equals(""))
                StandingHighlight(2);
            else // TODO: NPC 여러명일 때 처리
                StandingHighlight(1);

            onDialogueLineFinished();
        }

        #endregion

        private void ChangeBGM(int idx = 1)
        {
            switch(idx)
            {
                case 1:
                    Managers.Sound.Play(Sound.BGM, "CH2/BGM_01_Normal");
                    break;
                case 2:
                    Managers.Sound.Play(Sound.BGM, "CH2/BGM_02_Serious");
                    break;
                case 3:
                    Managers.Sound.Play(Sound.BGM, "CH2/BGM_03_Exciting");
                    break;
                case 4:
                    Managers.Sound.Play(Sound.BGM, "CH2/BGM_04_Wariness");
                    break;
                case 5:
                    Managers.Sound.Play(Sound.BGM, "CH2/BGM_05_Faint");
                    break;
                case 6:
                    Managers.Sound.Play(Sound.BGM, "CH2/BGM_06_Micael's Riddle");
                    break;
                case 7:
                    Managers.Sound.Play(Sound.BGM, "CH2/BGM_07_R2IsComing");
                    break;
            }
        }

        private void SetSuperArioPack()
        {
            Managers.Sound.StopBGM();
            _SuperArioPack.SetActive(true);
        }

        private void SetTcgPack()
        {
            _tcgPack.SetActive(true);
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

        private void PartnerAppear(string partner)
        {
            if (Enum.TryParse(partner, true, out Npc npc)) // Parse the string to enum
            {
                int idx = (int)npc; // Convert enum to int
                _characters[1] = _npcs[idx];
                _characters[1].gameObject.SetActive(true);
            }
        }

        private void PartnerOut()
        {
            _characters[1].gameObject.SetActive(false);
        }

        private void DialogueFin()
        {
            // 자동진행 끄기
            _isAutoAdvanced = false;

            // 라플리는 밝게 처리
            StandingHighlight(0);
            
            // NPC가 있다면 끈다
            _characters[1].gameObject.SetActive(false);
        }

        private void StandingHighlight(int num)
        {
            // 0 라플리 1 NPC 2 모두 어둡게
            Color bright = new Color32(255, 255, 255, 255);
            Color dark = new Color32(157, 157, 157, 255);

            if (num <= 1)
            {
                _characters[num].color = bright;
                _characters[1 - num].color = dark;
            }
            else
            {
                for (int i = 0; i < _characters.Length; i++)
                {
                    _characters[i].color = dark;
                }
            }
        }

        public void StartLocation(string location)
        {
            Managers.Data.CH2.Location = location;
            _locationUiController.StartLocation();
        }

        public void SetLocation(string location)
        {
            Managers.Data.CH2.Location = location;
            _locationUiController.MoveLocation();
        }

        public void NextTurn()
        {
            _turnController.NextTurn();
        }

        private void ShowOptions()
        {
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

        /*
        public void SkipDialogue()
        {
            Debug.Log("Skip");
            _runner.Stop();

            if (Managers.Data.CH2.IsSpecialDialogue)
            {
                _runner.StartDialogue("EndS");
            }
            else
            {
                _runner.StartDialogue("EndN");
            }

            if (Managers.Data.CH2.Turn == 7)
                Ending();
        }

        private void Ending()
        {
            // 개발한 부분까지 모두 출력 완료함
            _toBeContinued.SetActive(true);
        }

        private void NpcFace(int idx)
        {
            // 현재는 미카엘뿐이지만 추후 확대
            _michael.SetFace(idx);
        }
        */
    }
}