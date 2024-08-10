using Runtime.ETC;
using System;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public enum Character
    {
        rapley = 0,
        r2mon = 1,
    }

    public class CH2Dialogue : DialogueViewBase
    {
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private Image[] _characters = new Image[2];
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private GameObject _toBeContinued;
        [SerializeField] private TurnController _turnController;
        private string _speaker;

        private void Awake()
        {
            _runner.AddCommandHandler("DialogueFin", DialogueFin);
            _runner.AddCommandHandler("Ending", Ending);
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            _speaker = dialogueLine.CharacterName;
            SetNameTag(_speaker != "");

            if (_speaker.Equals("라플리"))
                StandingHighlight((int)Character.rapley);
            else if (_speaker.Equals("R2-Mon"))
                StandingHighlight((int)Character.r2mon);
            else
                StandingHighlight(2);

            onDialogueLineFinished();
        }

        private void SetNameTag(bool hasName)
        {
            _nameTag.SetActive(hasName);
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
            // 0 라플리 1 NPC 2 둘 다 어둡게
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