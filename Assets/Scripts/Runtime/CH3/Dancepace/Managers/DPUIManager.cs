using UnityEngine;

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

        public void UpdateKeyGuide(string currentPoseId, string nextPoseId)
        {
            keyGuideUI.UpdateKeyGuide(currentPoseId, nextPoseId);
        }
    }
} 