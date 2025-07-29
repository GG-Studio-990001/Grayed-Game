using Runtime.ETC;
using System;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.Dancepace
{
    public class DPOpening : DialogueViewBase
    {
        private DialogueRunner _runner;

        [SerializeField]
        private RectTransform _speechBubbleA;
        [SerializeField]
        private TextMeshProUGUI _line;
        [SerializeField]
        private GameObject _timeline2;

        private void Awake()
        {
            _runner = GetComponent<DialogueRunner>();
            _runner.AddCommandHandler("NPCSpeak", NPCSpeak);
            _runner.AddCommandHandler("OpeningDialogueFin", OpeningDialogueFin);
        }

        public void StartDialogue()
        {
            if (!Managers.Data.CH3.IsDancepacePlayed)
                _runner.StartDialogue("FirstEnter");
            else
                _runner.StartDialogue("RandomEnter");   
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            Managers.Sound.StopSFX();
            ResizeSpeechBubble();
            onDialogueLineFinished();
        }

        private void ResizeSpeechBubble()
        {
            float height = _line.text.Length <= 12 ? 200f : 225f;
            Debug.Log($"height: {height}");
            _speechBubbleA.sizeDelta = new Vector2(_speechBubbleA.sizeDelta.x, height - 12.26f);
        }

        private void SetLinePos()
        {
            float xPos = -474;

            _line.transform.localPosition = new Vector3(xPos, _line.transform.localPosition.y, _line.transform.localPosition.z);
        }

        public void NPCSpeak()
        {
            ShowSpeechBubble();
            SetLinePos();
        }

        public void OpeningDialogueFin()
        {
            Managers.Sound.StopSFX();
            ShowSpeechBubble();
            _timeline2.SetActive(true);
        }

        private void ShowSpeechBubble()
        {
            if (_speechBubbleA.gameObject.activeSelf)
                _speechBubbleA.gameObject.SetActive(false);
            else
                _speechBubbleA.gameObject.SetActive(true);
        }
    }
}