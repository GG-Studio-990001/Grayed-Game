using Runtime.ETC;
using System;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Pacmom
{
    public class InGameDialogue : DialogueViewBase
    {
        private DialogueRunner runner;
        [SerializeField]
        PMGameController controller;

        [Header("=DustA=")]
        [SerializeField]
        private GameObject dustA;
        [SerializeField]
        private GameObject bubbleA;
        [SerializeField]
        private TextMeshProUGUI textA;
        [Header("=DustB=")]
        [SerializeField]
        private GameObject dustB;
        [SerializeField]
        private GameObject bubbleB;
        [SerializeField]
        private TextMeshProUGUI textB;
        [Header("=Else=")]
        [SerializeField]
        private float currentTime = 0f;
        [SerializeField]
        private float targetTime = 15f;
        private int dustID = 0;

        private void Awake()
        {
            runner = GetComponent<DialogueRunner>();
            runner.AddCommandHandler("HideBubble", HideBubble);
        }

        private void Update()
        {
            CheckTime();

            if (bubbleA.activeInHierarchy)
                SetBubble(dustA, bubbleA, textA);

            if (bubbleB.activeInHierarchy)
                SetBubble(dustB, bubbleB, textB);
        }

        private void SetBubble(GameObject dust, GameObject bubble, TextMeshProUGUI text)
        {
            float xPos = (dust == dustA ? -2.3f : 2.3f);
            float yRotate = (dust == dustA ? 0f : -180f);

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

            bubble.transform.position = new Vector3(dust.transform.position.x + xPos,
            dust.transform.position.y + 1.3f, dust.transform.position.z);

            bubble.transform.rotation = Quaternion.Euler(0f, yRotate, 0f);
            text.transform.rotation = Quaternion.Euler(0f, yRotate * 2f, 0f);
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            string speakerStr = dialogueLine.CharacterName;
            Speaker nowSpeaker = Speaker.none;

            if (dustID != 0) // ID로 구별
            {
                nowSpeaker = (dustID == 1 ? Speaker.dustA : Speaker.dustB);
            }
            else // 이름으로 구별
            {
                if (speakerStr == GlobalConst.DustAStr)
                    nowSpeaker = Speaker.dustA;
                else if (speakerStr == GlobalConst.DustBStr)
                    nowSpeaker = Speaker.dustB;
            }

            TextMeshProUGUI text = (nowSpeaker == Speaker.dustA ? textA : textB);

            text.text = dialogueLine.TextWithoutCharacterName.Text;

            if (nowSpeaker == Speaker.dustA)
            {
                bubbleA.SetActive(true);
                textA.text = text.text;
            }
            else if (nowSpeaker == Speaker.dustB)
            {
                bubbleB.SetActive(true);
                textB.text = text.text;
            }
            
            if (dustID != 0) // 초기화
                dustID = 0;

            onDialogueLineFinished();
        }

        public void HideBubble()
        {
            if (bubbleA.activeSelf)
                bubbleA.SetActive(false);
            if (bubbleB.activeSelf)
                bubbleB.SetActive(false);
        }

        #region Time
        private void CheckTime()
        {
            if (!controller.isGameOver && dustA.GetComponent<AI>().isStronger)
                currentTime += Time.deltaTime;

            if (targetTime < currentTime)
                ShowRandom();
        }

        private void ShowRandom()
        {
            RandomDialogue();

            currentTime = 0;
            targetTime = UnityEngine.Random.Range(15f, 20f);
        }
        #endregion

        #region Dialogue With ID
        public void BlockedDialogue(int ID)
        {
            dustID = ID;
            runner.Stop();
            runner.StartDialogue("PMBlocked");
        }

        public void CatchDialogue(int ID)
        {
            dustID = ID;
            runner.Stop();
            runner.StartDialogue("PMCatch");
        }

        public void BeCaughtDialogue(int ID)
        {
            dustID = ID;
            runner.Stop();
            runner.StartDialogue("PMBeCaught");
        }

        private void RandomDialogue()
        {
            dustID = UnityEngine.Random.Range(1, 3);

            bool dustABloked = dustA.GetComponent<DustBlocked>().isBlocked;
            bool dustBBloked = dustB.GetComponent<DustBlocked>().isBlocked;

            if (dustABloked && dustBBloked)
                return;
            else if (dustID == 1 && dustABloked)
                dustID = 2;
            else if (dustID == 2 && dustBBloked)
                dustID = 1;

            runner.Stop();
            runner.StartDialogue("PMRandom");
        }
        #endregion

        #region Dialogue Both
        public void VacuumDialogue(bool WasVaccumMode)
        {
            runner.Stop();

            if (!WasVaccumMode)
                runner.StartDialogue("PMVacuumMode");
            else
                runner.StartDialogue("PMVacuumModeAgain");

            if (targetTime < 5f)
                targetTime += 5f; // 청소기모드 직후 랜덤대사 출력 방지
        }

        public void GameOverDialogue()
        {
            runner.Stop();
            runner.StartDialogue("PMGameClear");
        }
        #endregion
    }
}