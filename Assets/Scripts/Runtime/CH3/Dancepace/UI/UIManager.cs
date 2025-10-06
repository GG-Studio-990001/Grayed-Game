using Runtime.ETC;
using UnityEngine;
using System.Collections;
using TMPro;

namespace Runtime.CH3.Dancepace
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TimeBarUI timeBarUI;
        [SerializeField] private KeyGuideUI keyGuideUI;
        [SerializeField] private TextBallonUI textBalloon;
        [SerializeField] private ResultPanel resultPanel;
        [SerializeField] private TextMeshProUGUI waveIdText;

        [Header("Panels")]
        [SerializeField] private GameObject moreRehearsalPanel;
        [SerializeField] private TextMeshProUGUI mcText;

        private LocalizedText[] localizedTexts;

        private void Awake()
        {
            ValidateComponents();
            localizedTexts = GetComponentsInChildren<LocalizedText>(true);
            foreach (var loc in localizedTexts)
                loc.Refresh();
        }

        private void ValidateComponents()
        {
            if (timeBarUI == null) Debug.LogError("TimeBarUI is missing!");
            if (keyGuideUI == null) Debug.LogError("KeyGuideUI is missing!");
            if (moreRehearsalPanel == null) Debug.LogError("MoreRehearsalPanel is missing!");
        }

        public void InitializeUI()
        {
            timeBarUI.Initialize();
            keyGuideUI.Initialize();

            moreRehearsalPanel.SetActive(false);
            if (waveIdText != null) waveIdText.gameObject.SetActive(false);
        }
        public void ShowMoreRehearsalPanel(bool show)
        {
            moreRehearsalPanel.SetActive(show);
        }

        public void UpdateTimeBar(float currentTime, float maxTime)
        {
            timeBarUI.UpdateTimeBar(currentTime, maxTime);
        }

        public void SetRehearsalMode(bool isRehearsal)
        {
            keyGuideUI.SetRehearsalMode(isRehearsal);
        }

        public void UpdateKeyGuide(string poseId)
        {
            keyGuideUI.UpdateKeyGuide(poseId);
        }

        public void ShowKeyGuide(bool show)
        {
            if (keyGuideUI != null)
                keyGuideUI.gameObject.SetActive(show);
        }

        public void ShowResultPanel(int score, int perfect, int great, int bad)
        {
            if (resultPanel != null)
            {
                resultPanel.gameObject.SetActive(true);
                resultPanel.SetText(score, perfect, great, bad);
            }
        }

        public void ShowTextBalloon(EPoseType poseType)
        {
            if (poseType == EPoseType.None)
            {
                textBalloon.gameObject.SetActive(false);
                return;
            }

            textBalloon.gameObject.SetActive(true);
            string push = "";
            switch (poseType)
            {
                case EPoseType.Up:
                    push = StringTableManager.Get("TextBallon_Push_0");
                    textBalloon.SetText(push);
                    break;
                case EPoseType.Down:
                    push = StringTableManager.Get("TextBallon_Push_2");
                    textBalloon.SetText(push);
                    break;
                case EPoseType.Left:
                    push = StringTableManager.Get("TextBallon_Push_1");
                    textBalloon.SetText(push);
                    break;
                case EPoseType.Right:
                    push = StringTableManager.Get("TextBallon_Push_3");
                    textBalloon.SetText(push);
                    break;
                default:
                    break;
            }
        }
        public void ShowTextBalloon(string text, float duration)
        {
            textBalloon?.SetText(text);
            textBalloon?.gameObject.SetActive(true);
            StartCoroutine(HideTextBalloonAfterDelay(duration));
        }

        public void HideTextBalloon()
        {
            textBalloon.gameObject.SetActive(false);
        }

        public void ShowTimeBar(bool show)
        {
            if (timeBarUI != null)
                timeBarUI.gameObject.SetActive(show);
        }

        public void ShowWaveId(string id, float duration = 1.5f)
        {
            if (waveIdText == null) return;
            waveIdText.text = id;
            waveIdText.gameObject.SetActive(true);
            // duration 인자는 유지하되, 자동 숨김은 하지 않음 (지속 표시)
        }

        public void ShowWaveId(string id)
        {
            if (waveIdText == null) return;
            waveIdText.text = id;
            waveIdText.gameObject.SetActive(true);
        }

        private IEnumerator HideWaveIdAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (waveIdText != null)
                waveIdText.gameObject.SetActive(false);
        }


        private IEnumerator HideTextBalloonAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            textBalloon.gameObject.SetActive(false);
        }

        public void ShowMcText()
        {
            mcText.gameObject.SetActive(true);
            mcText.text = StringTableManager.Get("StageStart");
            StartCoroutine(ShowMcTextCoroutine());
        }

        private IEnumerator ShowMcTextCoroutine()
        {
            yield return new WaitForSeconds(2f);
            mcText.text = "3!";
            yield return new WaitForSeconds(1f);
            mcText.text = "2!";
            yield return new WaitForSeconds(1f);
            mcText.text = "1!";
            yield return new WaitForSeconds(1f);
            mcText.gameObject.SetActive(false);
        }
    }
}