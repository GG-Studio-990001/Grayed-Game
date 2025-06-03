using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace.UI
{
    public class PoseIndicator : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image _poseIcon;
        [SerializeField] private TextMeshProUGUI _poseNameText;
        [SerializeField] private TextMeshProUGUI _judgmentText;

        private void Start()
        {
            // 리듬 매니저 이벤트 구독
            RhythmManager.Instance.OnPlayBeat += OnPlayBeat;
            RhythmManager.Instance.OnJudgment += OnJudgment;
        }

        private void OnPlayBeat(BeatData beat)
        {
            PoseData poseData = RhythmManager.Instance.GetPoseDataById(beat.poseId);
            if (poseData != null)
            {
                _poseNameText.text = poseData.poseName;
                if (poseData.poseImage != null)
                {
                    _poseIcon.sprite = poseData.poseImage;
                    _poseIcon.gameObject.SetActive(true);
                }
                else
                {
                    _poseIcon.gameObject.SetActive(false);
                }
            }
        }

        private void OnJudgment(JudgmentType judgment)
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

        private void OnDestroy()
        {
            if (RhythmManager.Instance != null)
            {
                RhythmManager.Instance.OnPlayBeat -= OnPlayBeat;
                RhythmManager.Instance.OnJudgment -= OnJudgment;
            }
        }
    }
} 