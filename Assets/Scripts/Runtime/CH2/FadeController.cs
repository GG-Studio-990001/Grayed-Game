using DG.Tweening;
using Runtime.InGameSystem;

namespace Runtime.CH2
{
    public class CH2FadeController : FadeController
    {
        private Tween _currentFadeTween;

        public override void StartFadeIn()
        {
            StopCurrentFadeTween();

            fadeImage.raycastTarget = true;
            _currentFadeTween = fadeImage.DOFade(0, FadeDuration)
                                         .SetEase(Ease.InOutQuad)
                                         .OnComplete(() => fadeImage.raycastTarget = false);
        }

        public override void StartFadeOut()
        {
            StopCurrentFadeTween();

            fadeImage.raycastTarget = true;
            _currentFadeTween = fadeImage.DOFade(1, FadeDuration)
                                         .SetEase(Ease.InOutQuad)
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
