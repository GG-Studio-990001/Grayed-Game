using Runtime.CH2.Main;
using Runtime.ETC;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Runtime.CH2.Dialogue
{
    public class CH2Dialogue : DialogueViewBase
    {
        // 클래스명 서로 바뀜...
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private TurnController2 _turnController;
        [Header("=Else=")]
        [SerializeField] private Image[] _characters = new Image[2];
        [SerializeField] private Image[] _npcs = new Image[2];
        [SerializeField] private FaceSpriteSwitcher _michael;
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private CanvasGroup _lineViewCanvas;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private GameObject _toBeContinued;
        [SerializeField] private bool _isAutoAdvanced = false;
        private string _speaker;

        private void Awake()
        {
            _runner.AddCommandHandler("NextProgress", NextProgress);
            _runner.AddCommandHandler<int>("PartnerAppear", PartnerAppear);
            _runner.AddCommandHandler("PartnerOut", PartnerOut);
            _runner.AddCommandHandler<int>("NpcFace", NpcFace);
            _runner.AddCommandHandler("DialogueFin", DialogueFin);
            _runner.AddCommandHandler("Ending", Ending);
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

            if (_speaker.Equals("라플리"))
                StandingHighlight(0);
            else if (_speaker.Equals("R2-Mon") || (_speaker.Equals("미카엘")))
                StandingHighlight(1);
            else
                StandingHighlight(2);

            onDialogueLineFinished();
        }

        public void NextProgress()
        {
            Managers.Data.CH2.Progress++;
        }

        public void AutoDialogueToggle()
        {
            _isAutoAdvanced = !_isAutoAdvanced;
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
        }*/

        private void SetNameTag(bool hasName)
        {
            _nameTag.SetActive(hasName);
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

        public void SkipDialogue()
        {
            Debug.Log("Skip");
            _runner.Stop();
            DialogueFin();

            if (Managers.Data.CH2.Location == "미카엘의 신전")
                Ending();
        }

        private void DialogueFin()
        {
            // 라플리는 밝게 처리하고 NPC가 있다면 끈다
            StandingHighlight(0);
            _characters[1].gameObject.SetActive(false);

            // 오른쪽에는 이동 가능한 장소 목록을 띄운다
            _turnController.DisplayAvailableLocations();
        }

        private void Ending()
        {
            // 개발한 부분까지 모두 출력 완료함
            _toBeContinued.SetActive(true);
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
    }
}