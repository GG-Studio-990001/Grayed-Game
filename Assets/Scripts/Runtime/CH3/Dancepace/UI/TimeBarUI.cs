using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Runtime.CH3.Dancepace
{
    public class TimeBarUI : MonoBehaviour
    {
        [Header("Time Bar Components")]
        [SerializeField] private Image fillImage;
        [SerializeField] private Image handleImage;
        [SerializeField] private float fillDuration = 0.3f;
        
        [Header("Handle Sprites")]
        [SerializeField] private Sprite normalHandleSprite;
        [SerializeField] private Sprite warningHandleSprite;
        [SerializeField] private Sprite dangerHandleSprite;
        
        [Header("Animation Settings")]
        [SerializeField] private float warningThreshold = 0.3f;
        [SerializeField] private float dangerThreshold = 0.1f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color dangerColor = Color.red;

        private float currentTime;
        private float maxTime;
        private Tween fillTween;

        public void Initialize()
        {
            if (fillImage == null || handleImage == null)
            {
                Debug.LogError("Required components are missing!");
                return;
            }

            fillImage.fillAmount = 0f;
            fillImage.color = normalColor;
            handleImage.sprite = normalHandleSprite;
        }

        public void UpdateTimeBar(float currentTime, float maxTime)
        {
            this.currentTime = currentTime;
            this.maxTime = maxTime;

            float fillAmount = currentTime / maxTime;
            
            // 이전 트윈이 있다면 중지
            fillTween?.Kill();

            // 새로운 트윈 생성
            fillTween = fillImage.DOFillAmount(fillAmount, fillDuration)
                .SetEase(Ease.OutQuad);

            // 상태에 따른 색상과 핸들 스프라이트 변경
            if (fillAmount <= dangerThreshold)
            {
                fillImage.color = dangerColor;
                handleImage.sprite = dangerHandleSprite;
            }
            else if (fillAmount <= warningThreshold)
            {
                fillImage.color = warningColor;
                handleImage.sprite = warningHandleSprite;
            }
            else
            {
                fillImage.color = normalColor;
                handleImage.sprite = normalHandleSprite;
            }
        }

        public void ResetTimeBar()
        {
            fillTween?.Kill();
            fillImage.fillAmount = 0f;
            fillImage.color = normalColor;
            handleImage.sprite = normalHandleSprite;
        }

        private void OnDestroy()
        {
            fillTween?.Kill();
        }
    }
} 