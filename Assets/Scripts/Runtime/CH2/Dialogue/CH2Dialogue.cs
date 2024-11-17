using Runtime.CH2.Location;
using Runtime.CH2.Main;
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
        [SerializeField] private LocationTransitionUI _locationSelectionUI;
        [Header("=Else=")]
        [SerializeField] private CanvasGroup _lineViewCanvas;
        [SerializeField] private LineView _lineView;
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private Image[] _characters = new Image[2];
        [SerializeField] private Image[] _npcs = new Image[5];
        [SerializeField] private FaceSpriteSwitcher _michael;
        [SerializeField] private GameObject _toBeContinued;
        [SerializeField] private bool _isAutoAdvanced = false;
        private string _speaker;
        // private string _dialogueText;
        // private int currentMaxVisibleCharacters;

        private void Awake()
        {
            _runner.AddCommandHandler<string>("StartLocation", StartLocation);
            _runner.AddCommandHandler<string>("SetLocation", SetLocation);
            _runner.AddCommandHandler("NextTurn", NextTurn);
            _runner.AddCommandHandler("ShowOptions", ShowOptions);
            _runner.AddCommandHandler<string>("PartnerAppear", PartnerAppear);
            _runner.AddCommandHandler("PartnerOut", PartnerOut);
            _runner.AddCommandHandler("DialogueFin", DialogueFin);
            //_runner.AddCommandHandler("Ending", Ending);
            //_runner.AddCommandHandler<int>("NpcFace", NpcFace);
        }

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
            else
                StandingHighlight(1);

            onDialogueLineFinished();
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
            _locationSelectionUI.StartLocation();
        }

        public void SetLocation(string location)
        {
            Managers.Data.CH2.Location = location;
            _locationSelectionUI.MoveLocation();
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
        public void AutoDialogueToggle()
        {
            _isAutoAdvanced = !_isAutoAdvanced;
            Debug.Log("_isAutoAdvanced: " + _isAutoAdvanced);
        }

        public void StartAutoDialogue()
        {
            Debug.Log("어");
            if (_isAutoAdvanced)
            {
                StartCoroutine(nameof(AutoDialogue));
                Debug.Log("기다료");
            }
                
        }

        IEnumerator AutoDialogue()
        {
            Debug.Log("기달");
            yield return new WaitForSeconds(1f);
            Debug.Log("끗");
            _lineView.OnContinueClicked();
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