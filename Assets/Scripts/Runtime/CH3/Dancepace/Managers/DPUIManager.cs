using UnityEngine;
using Runtime.ETC;

namespace Runtime.CH3.Dancepace
{
    public class DPUIManager : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TimeBarUI timeBarUI;
        [SerializeField] private KeyGuideUI keyGuideUI;

        [Header("Panels")]
        [SerializeField] private GameObject rehearsalPanel;
        [SerializeField] private GameObject moreRehearsalPanel;
        [SerializeField] private GameObject successPanel;
        [SerializeField] private GameObject failPanel;

        private void Awake()
        {
            ValidateComponents();
        }

        private void ValidateComponents()
        {
            if (timeBarUI == null) Debug.LogError("TimeBarUI is missing!");
            if (keyGuideUI == null) Debug.LogError("KeyGuideUI is missing!");
            if (rehearsalPanel == null) Debug.LogError("RehearsalPanel is missing!");
            if (moreRehearsalPanel == null) Debug.LogError("MoreRehearsalPanel is missing!");
        }

        public void InitializeUI()
        {
            timeBarUI.Initialize();
            keyGuideUI.Initialize();
            
            rehearsalPanel.SetActive(false);
            moreRehearsalPanel.SetActive(false);
        }

        public void ShowRehearsalPanel(bool show)
        {
            rehearsalPanel.SetActive(show);
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

        public void UpdateKeyGuide(EPoseType nextPoseId)
        {
            keyGuideUI.UpdateKeyGuide(nextPoseId.ToString());
        }

        public void ShowSuccessPanel(bool show)
        {
            if (successPanel != null)
            {
                successPanel.SetActive(show);
            }
        }

        public void ShowFailPanel(bool show)
        {
            if (failPanel != null)
            {
                failPanel.SetActive(show);
            }
        }

        public void ShowKeyGuide(bool show)
        {
            if (keyGuideUI != null)
                keyGuideUI.gameObject.SetActive(show);
        }

        public void ShowResultPanel(int score)
        {
            Debug.Log($"최종 점수: {score}");
            // TODO: 실제 결과 UI 구현
        }
    }
} 