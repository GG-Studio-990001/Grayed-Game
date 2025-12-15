using DG.Tweening;
using Runtime.InGameSystem;
using UnityEngine;

namespace Runtime.CH3
{
    /// <summary>
    /// CH3용 페이드 컨트롤러 (DOTween 사용, CH2와 동일한 방식)
    /// </summary>
    public class CH3FadeController : FadeController
    {
        private Tween _currentFadeTween;

        public override void StartFadeIn()
        {
            StartFade(0f, 0.1f, 0.0f);
        }

        public override void StartFadeOut()
        {
            StartFade(1f, 0.0f, 0.1f);
        }

        private void StartFade(float targetAlpha, float delayBefore, float delayAfter)
        {
            StopCurrentFadeTween();

            if (fadeImage == null)
            {
                Debug.LogError("FadeController: fadeImage가 설정되지 않았습니다!");
                return;
            }

            fadeImage.raycastTarget = true;
            _currentFadeTween = DOTween.Sequence()
                .AppendInterval(delayBefore)
                .Append(fadeImage.DOFade(targetAlpha, FadeDuration - (delayBefore + delayAfter))
                                 .SetEase(Ease.InOutSine))
                .AppendInterval(delayAfter)
                .OnComplete(() => fadeImage.raycastTarget = false);
        }
        
        private void StopCurrentFadeTween()
        {
            if (_currentFadeTween != null && _currentFadeTween.IsActive())
            {
                _currentFadeTween.Kill();
                _currentFadeTween = null;
            }
        }

        private void OnDestroy()
        {
            StopCurrentFadeTween();
        }
    }
}

