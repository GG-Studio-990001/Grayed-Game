using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace
{
    public class TimeBarUI : MonoBehaviour
    {
        [Header("Time Bar Components")]
        [SerializeField] private Slider timeBarSlider;
        [SerializeField] private Image handleImage;
        //[SerializeField] private float fillDuration = 0.3f;
        
        [Header("Handle Sprites")]
        [SerializeField] private Sprite normalHandleSprite;
        [SerializeField] private Sprite warningHandleSprite;
        [SerializeField] private Sprite dangerHandleSprite;

        private float currentTime;
        private float maxTime;
        private Tween fillTween;

        public void Initialize()
        {
            if (timeBarSlider == null || handleImage == null)
            {
                Debug.LogError("Required components are missing!");
                return;
            }

            timeBarSlider.value = 0f;
            handleImage.sprite = normalHandleSprite;
        }

        public void UpdateTimeBar(float currentTime, float maxTime)
        {
            this.currentTime = currentTime;
            this.maxTime = maxTime;

            float fillAmount = Mathf.Clamp01(currentTime / maxTime);
            timeBarSlider.value = fillAmount;

            // 3등분하여 구간별로 핸들 스프라이트 변경
            if (fillAmount < 1f / 3f)
            {
                handleImage.sprite = normalHandleSprite;
            }
            else if (fillAmount < 2f / 3f)
            {
                handleImage.sprite = warningHandleSprite;
            }
            else
            {
                handleImage.sprite = dangerHandleSprite;
            }
        }

        public void ResetTimeBar()
        {
            timeBarSlider.value = 0f;
            handleImage.sprite = normalHandleSprite;
        }

        private void OnDestroy()
        {
            fillTween?.Kill();
        }
    }
} 