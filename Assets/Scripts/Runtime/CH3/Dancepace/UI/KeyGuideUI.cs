using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace Runtime.CH3.Dancepace
{
    public class KeyGuideUI : MonoBehaviour
    {
        [System.Serializable]
        public class KeyGuideData
        {
            public string poseId;
            public Image keyImage;
            public Color normalColor = Color.white;
            public Color activeColor = Color.blue;
            public Color nextColor = Color.red;
        }

        [Header("Key Guide Components")]
        [SerializeField] private List<KeyGuideData> keyGuideDataList;
        [SerializeField] private float colorChangeDuration = 0.2f;

        private Dictionary<string, KeyGuideData> keyGuideDict;
        private string currentPoseId;
        private string nextPoseId;
        private bool isRehearsalMode = true;

        public void Initialize()
        {
            if (keyGuideDataList == null || keyGuideDataList.Count == 0)
            {
                Debug.LogError("Key Guide Data List is empty!");
                return;
            }

            // 딕셔너리 초기화
            keyGuideDict = new Dictionary<string, KeyGuideData>();
            foreach (var data in keyGuideDataList)
            {
                if (data.keyImage == null)
                {
                    Debug.LogError($"Key Image is missing for poseData: {data.poseId}");
                    continue;
                }
                keyGuideDict[data.poseId] = data;
                data.keyImage.color = data.normalColor;
            }
        }

        public void SetRehearsalMode(bool isRehearsal)
        {
            isRehearsalMode = isRehearsal;
            if (!isRehearsalMode)
            {
                // 본게임 모드에서는 모든 키 가이드 숨김
                foreach (var data in keyGuideDataList)
                {
                    data.keyImage.color = data.normalColor;
                }
            }
        }

        public void UpdateKeyGuide(string poseId)
        {
            if (!isRehearsalMode) return;

            // 모든 키를 기본 색상으로 초기화
            foreach (var data in keyGuideDataList)
                data.keyImage.DOColor(data.normalColor, colorChangeDuration);

            // 현재 입력할 키만 빨간색으로 강조
            if (!string.IsNullOrEmpty(poseId) && keyGuideDict.TryGetValue(poseId, out var targetData))
                targetData.keyImage.DOColor(targetData.nextColor, colorChangeDuration);
        }

        public void ResetKeyGuide()
        {
            foreach (var data in keyGuideDataList)
            {
                data.keyImage.DOColor(data.normalColor, colorChangeDuration);
            }
        }
    }
} 