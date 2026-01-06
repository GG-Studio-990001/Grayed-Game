using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Runtime.ETC;

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
            float previousRate = CurseRemovalRate;
            CurseRemovalRate = Mathf.Clamp(CurseRemovalRate + rate, -100, 100);

            float changeTime = 1f;
            float delayTime = 0.5f;

            // 사운드 딜레이
            if (previousRate != CurseRemovalRate)
            {
                DOVirtual.DelayedCall(delayTime, () =>
                {
                    if (previousRate < CurseRemovalRate)
                        Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Curse_Gauge_Up");
                    else
                        Managers.Sound.Play(Sound.SFX, "CH3/CoC/CH3_SFX_CoC_Curse_Gauge_Down");
                });
            }

            // 슬라이더 Tween
            float targetValue = MapRateToSlider(CurseRemovalRate);

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