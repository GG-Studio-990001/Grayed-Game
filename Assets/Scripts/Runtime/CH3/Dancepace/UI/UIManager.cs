using Runtime.ETC;
using UnityEngine;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TimeBarUI timeBarUI;
        [SerializeField] private KeyGuideUI keyGuideUI;
        [SerializeField] private TextBallonUI textBalloon;

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

        public void UpdateKeyGuide(string poseId)
        {
            keyGuideUI.UpdateKeyGuide(poseId);
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

        public void ShowTextBalloon(EPoseType poseType)
        {
            if (poseType == EPoseType.None)
            {
                textBalloon.gameObject.SetActive(false);
                return;
            }

            textBalloon.gameObject.SetActive(true);
            switch (poseType)
            {
                case EPoseType.Up:
                    textBalloon.SetText("위");
                    break;
                case EPoseType.Down:
                    textBalloon.SetText("아래");
                    break;
                case EPoseType.Left:
                    textBalloon.SetText("왼쪽");
                    break;
                case EPoseType.Right:
                    textBalloon.SetText("오른쪽");
                    break;
                default:
                    break;
            }
        }

        public void HideTextBalloon()
        {
            textBalloon.gameObject.SetActive(false);
        }

        // public void ShowTextBalloon(EJudgmentType type)
        // {
        //     switch (type)
        //     {
        //         case EJudgmentType.Perfect:
        //         case EJudgmentType.Great:
        //             textBalloon[0].SetActive(true);
        //             break;
        //         case EJudgmentType.Bad:
        //             textBalloon[1].SetActive(true);
        //             break;
        //     }
        // }
    }
} 