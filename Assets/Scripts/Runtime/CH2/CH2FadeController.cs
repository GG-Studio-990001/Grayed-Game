using DG.Tweening;
using Runtime.InGameSystem;

namespace Runtime.CH2
{
    public class CH2FadeController : FadeController
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
    }
}
