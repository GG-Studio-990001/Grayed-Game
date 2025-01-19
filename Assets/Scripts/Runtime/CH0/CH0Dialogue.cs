using Runtime.ETC;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Runtime.CH0
{
    public class CH0Dialogue : DialogueViewBase
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [Header("=Else=")]
        [SerializeField] private GameObject _nameTag;
        [SerializeField] private CanvasGroup _lineViewCanvas;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [Header("=Lucky=")]
        [SerializeField] private GameObject _luckyPack;
        [SerializeField] private DialogueRunner _luckyDialogue;

        public UnityEvent OnDialogueStart => _runner.onDialogueStart;
        public UnityEvent OnDialogueEnd => _runner.onDialogueComplete;
        private string _speaker;

        private void Awake()
        {
            _runner.AddCommandHandler("GetLucky", GetLucky);
            _runner.AddCommandHandler("MeetLucky", MeetLucky);
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

            onDialogueLineFinished();
        }

        private void SetNameTag(bool hasName)
        {
            _nameTag.SetActive(hasName);
        }

        public void GetLucky()
        {
            Managers.Sound.Play(Sound.SFX, "CH1/Lucky_Dog&Key_SFX");
            _luckyPack.SetActive(false);
        }

        public void MeetLucky()
        {
            _luckyDialogue.StartDialogue("LuckyFirstMeet");
        }
    }
}