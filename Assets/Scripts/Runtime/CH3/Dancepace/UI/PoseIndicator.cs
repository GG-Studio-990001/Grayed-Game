using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Runtime.CH3.Dancepace
{
    public class PoseIndicator : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image _poseIcon;
        [SerializeField] private TextMeshProUGUI _poseNameText;
        [SerializeField] private TextMeshProUGUI _judgmentText;

        // 포즈 표시
        public void ShowPose(string poseId, Sprite poseSprite, string poseName)
        {
            _poseNameText.text = poseName;
            if (poseSprite != null)
            {
                _poseIcon.sprite = poseSprite;
                _poseIcon.gameObject.SetActive(true);
            }
            else
            {
                _poseIcon.gameObject.SetActive(false);
            }
            _judgmentText.text = "";
        }

        // 판정 표시
        public void OnJudgment(JudgmentType judgment)
        {
            _judgmentText.text = judgment.ToString();
            _judgmentText.color = GetJudgmentColor(judgment);
        }

        private Color GetJudgmentColor(JudgmentType judgment)
        {
            switch (judgment)
            {
                case JudgmentType.Great:
                    return Color.green;
                case JudgmentType.Good:
                    return Color.yellow;
                case JudgmentType.Bad:
                    return Color.red;
                default:
                    return Color.white;
            }
        }
    }
} 