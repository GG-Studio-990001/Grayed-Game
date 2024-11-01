using Runtime.CH2.Location;
using Runtime.CH2.Main;
using Runtime.ETC;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Runtime.CH2.Dialogue
{
    public enum Npc
    {
        r2mon = 0,
        michael = 1,
    }

    public class CH2Dialogue : DialogueViewBase
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private TurnController _turnController;
        [SerializeField] private LocationTransitionUI _locationSelectionUI;
        [Header("=Else=")]
        [SerializeField] private CanvasGroup _lineViewCanvas;
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private Image[] _characters = new Image[2];
        [SerializeField] private Image[] _npcs = new Image[2];
        [SerializeField] private FaceSpriteSwitcher _michael;
        [SerializeField] private GameObject _toBeContinued;
        [SerializeField] private bool _isAutoAdvanced = false;
        private string _speaker;

        private void Awake()
        {
            _runner.AddCommandHandler<string>("StartLocation", StartLocation);
            _runner.AddCommandHandler<string>("SetLocation", SetLocation);
            _runner.AddCommandHandler("NextTurn", NextTurn);
            _runner.AddCommandHandler("ShowOptions", ShowOptions);
            _runner.AddCommandHandler<bool>("IsSpecialDialogue", IsSpecialDialogue);
            _runner.AddCommandHandler("Ending", Ending);
            //_runner.AddCommandHandler<int>("PartnerAppear", PartnerAppear);
            //_runner.AddCommandHandler("PartnerOut", PartnerOut);
            //_runner.AddCommandHandler<int>("NpcFace", NpcFace);
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
            // SetAuto(onDialogueLineFinished);
            _speaker = dialogueLine.CharacterName;
            SetNameTag(_speaker != "");

            /*
            if (_speaker.Equals("라플리"))
                StandingHighlight(0);
            else if (_speaker.Equals("R2-Mon") || (_speaker.Equals("미카엘")))
                StandingHighlight(1);
            else
                StandingHighlight(2);
            */
            onDialogueLineFinished();
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

        private void DialogueFin()
        {
            // 라플리는 밝게 처리하고 NPC가 있다면 끈다
            // StandingHighlight(0);
            // _characters[1].gameObject.SetActive(false);
        }

        public void AutoDialogueToggle()
        {
            _isAutoAdvanced = !_isAutoAdvanced;
        }

        private void IsSpecialDialogue(bool special)
        {
            Managers.Data.CH2.IsSpecialDialogue = special;
        }

        private void Ending()
        {
            // 개발한 부분까지 모두 출력 완료함
            _toBeContinued.SetActive(true);
        }

        private void SetNameTag(bool hasName)
        {
            _nameTag.SetActive(hasName);
        }

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

        /*
        private void SetAuto(Action onDialogueLineFinished)
        {
            if (_isAutoAdvanced)
            {
                onDialogueLineFinished += () => StartAutoDialogue();
            }
            else
            {
                onDialogueLineFinished -= () => StartAutoDialogue();
                StopCoroutine(nameof(AutoDialogue));
            }
        }

        private void StartAutoDialogue()
        {
            StartCoroutine(nameof(AutoDialogue));
        }

        IEnumerator AutoDialogue()
        {
            Debug.Log("기달");
            yield return new WaitForSeconds(1f);
            Debug.Log("끗");
            _runner.Dialogue.Continue();
        }

        private void PartnerAppear(int idx)
        {
            _characters[1] = _npcs[idx];
            _characters[1].gameObject.SetActive(true);
        }

        private void PartnerOut()
        {
            _characters[1].gameObject.SetActive(false);
        }

        private void NpcFace(int idx)
        {
            // 현재는 미카엘뿐이지만 추후 확대
            _michael.SetFace(idx);
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
        }*/
    }
}