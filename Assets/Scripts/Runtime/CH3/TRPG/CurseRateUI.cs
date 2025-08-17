using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH3.TRPG
{
    public class CurseRateUI : MonoBehaviour
    {
        [Range(-100, 100)]
        [SerializeField] private int CurseRemovalRate = 0;
        [SerializeField] private Slider _slider;

        private void Start()
        {
            // 슬라이더 초기값 설정 (CurseRemovalRate 기준)
            _slider.value = MapRateToSlider(CurseRemovalRate);
        }

        public void ChangeCurseRate(int rate)
        {
            // 값 합산 및 Clamp
            CurseRemovalRate = Mathf.Clamp(CurseRemovalRate + rate, -100, 100);

            // Tween으로 슬라이더 값 변경, 딜레이 후 값 변경 애니메이션
            float targetValue = MapRateToSlider(CurseRemovalRate);
            float changeTime = 1f;
            float delayTime = 0.5f;

            _slider.DOValue(targetValue, changeTime)
                   .SetEase(Ease.OutCubic)
                   .SetDelay(delayTime);
        }

        // -100~100 -> 0~1 변환 (0이 0.5)
        private float MapRateToSlider(int rate)
        {
            return (rate + 100f) / 200f;
        }
    }
}