using Runtime.ETC;
using System;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Pacmom
{
    public class InGameDialogue : DialogueViewBase
    {
        private DialogueRunner _runner;
        [NonSerialized] public PMController GameController;

        [Header("=DustA=")]
        [SerializeField] private GameObject _dustA;
        [SerializeField] private GameObject _bubbleA;
        [SerializeField] private TextMeshProUGUI _textA;
        [Header("=DustB=")]
        [SerializeField] private GameObject _dustB;
        [SerializeField] private GameObject _bubbleB;
        [SerializeField] private TextMeshProUGUI _textB;
        [Header("=Else=")]
        [SerializeField] private float _currentTime = 0f;
        [SerializeField] private float _targetTime = 15f;
        private int _dustID = 0;

        private void Awake()
        {
            _runner = GetComponent<DialogueRunner>();
        }

        private void Update()
        {
            CheckTime();

            if (_bubbleA.activeInHierarchy)
                SetBubble(_dustA, _bubbleA, _textA);

            if (_bubbleB.activeInHierarchy)
                SetBubble(_dustB, _bubbleB, _textB);
        }

        private void SetBubble(GameObject dust, GameObject bubble, TextMeshProUGUI text)
        {
            float xPos = (dust == _dustA ? -2.3f : 2.3f);
            float yRotate = (dust == _dustA ? 0f : -180f);

            if (dust.transform.position.x > 12)
            {
                xPos = -2.3f;
                yRotate = 0f;
            }
            else if (dust.transform.position.x < -12)
            {
                xPos = 2.3f;
                yRotate = -180f;
            }


            bubble.transform.SetPositionAndRotation(new Vector3(dust.transform.position.x + xPos,
                dust.transform.position.y + 1.3f, dust.transform.position.z), Quaternion.Euler(0f, yRotate, 0f));

            text.transform.rotation = Quaternion.Euler(0f, yRotate * 2f, 0f);
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            string speakerStr = dialogueLine.CharacterName;
            Speaker nowSpeaker = Speaker.none;

            if (_dustID != 0) // ID로 구별
            {
                nowSpeaker = (_dustID == 1 ? Speaker.dustA : Speaker.dustB);
            }
            else // 이름으로 구별
            {
                if (speakerStr == GlobalConst.DustAStr)
                    nowSpeaker = Speaker.dustA;
                else if (speakerStr == GlobalConst.DustBStr)
                    nowSpeaker = Speaker.dustB;
            }

            if (nowSpeaker == Speaker.dustA)
            {
                ShowBubbleA();
                _textA.text = dialogueLine.TextWithoutCharacterName.Text;
            }
            else if (nowSpeaker == Speaker.dustB)
            {
                ShowBubbleB();
                _textB.text = dialogueLine.TextWithoutCharacterName.Text;
            }

            if (_dustID != 0) // 초기화
                _dustID = 0;

            onDialogueLineFinished();
        }

        private void ShowBubbleA()
        {
            _bubbleA.SetActive(true);

            CancelInvoke(nameof(HideBubbleA));

            if (!GameController.IsGameOver)
                Invoke(nameof(HideBubbleA), 3f);
        }

        private void ShowBubbleB()
        {
            _bubbleB.SetActive(true);

            CancelInvoke(nameof(HideBubbleB));

            if (!GameController.IsGameOver)
                Invoke(nameof(HideBubbleB), 3f);
        }

        public void StopDialogue(float delayTime = 0f)
        {
            Invoke(nameof(HideBubbleA), delayTime);
            Invoke(nameof(HideBubbleB), delayTime);
        }

        private void HideBubbleA()
        {
            if (_bubbleA.activeSelf)
                _bubbleA.SetActive(false);
        }

        private void HideBubbleB()
        {
            if (_bubbleB.activeSelf)
                _bubbleB.SetActive(false);
        }

        #region Time
        private void CheckTime()
        {
            if (!GameController.IsGameOver && !GameController.IsVacuumMode)
                _currentTime += Time.deltaTime;

            if (_targetTime < _currentTime)
                RandomDialogue();
        }
        #endregion

        #region Dialogue With ID
        public void BlockedDialogue(int ID)
        {
            _dustID = ID;
            InGameDialogueStart("PMBlocked");
        }

        public void BeCaughtDialogue(int ID)
        {
            _dustID = ID;
            InGameDialogueStart("PMBeCaught");
        }

        public void ChasedDialogue(int ID)
        {
            _dustID = ID;
            InGameDialogueStart("PMChased");
        }

        private void RandomDialogue()
        {
            _dustID = UnityEngine.Random.Range(1, 3);

            bool dustABloked = _dustA.GetComponent<DustBlocked>().IsBlocked;
            bool dustBBloked = _dustB.GetComponent<DustBlocked>().IsBlocked;

            if (dustABloked && dustBBloked)
                return;
            else if (_dustID == 1 && dustABloked)
                _dustID = 2;
            else if (_dustID == 2 && dustBBloked)
                _dustID = 1;

            InGameDialogueStart("PMRandom");

            _targetTime = UnityEngine.Random.Range(20f, 30f);
        }
        #endregion

        #region Dialogue Both
        public void VacuumDialogue(bool WasVaccumMode)
        {
            if (!WasVaccumMode)
                InGameDialogueStart("PMVacuumMode");
            else
                InGameDialogueStart("PMVacuumModeAgain");
        }

        public void GameOverDialogue()
        {
            InGameDialogueStart("PMGameClear");
        }

        private void InGameDialogueStart(string dialogueName)
        {
            _runner.Stop();
            _runner.StartDialogue(dialogueName);
            _currentTime = 0;
        }
        #endregion
    }
}