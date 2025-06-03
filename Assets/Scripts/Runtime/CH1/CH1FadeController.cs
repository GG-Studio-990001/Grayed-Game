using Runtime.InGameSystem;
using System.Collections;
using UnityEngine;

namespace Runtime.CH1
{
    public class CH1FadeController : FadeController
    {
        [SerializeField] private float fadeDeltaTime = 0.01f;
        private Coroutine _currentFadeCoroutine;

        public override void StartFadeIn()
        {
            StopCurrentFadeCoroutine();

            _currentFadeCoroutine = StartCoroutine(FadeInCoroutine(FadeDuration));
        }

        public override void StartFadeOut()
        {
            StopCurrentFadeCoroutine();

            _currentFadeCoroutine = StartCoroutine(FadeOutCoroutine(FadeDuration));
        }

        private void StopCurrentFadeCoroutine()
        {
            if (_currentFadeCoroutine != null)
            {
                StopCoroutine(_currentFadeCoroutine);
                _currentFadeCoroutine = null;
            }
        }

        private IEnumerator FadeInCoroutine(float duration)
        {
            fadeImage.raycastTarget = true; // 터치 차단
            float time = 0;
            while (time < duration)
            {
                time += fadeDeltaTime;
                fadeImage.color = new Color(0, 0, 0, 1 - time / duration);
                yield return new WaitForSeconds(fadeDeltaTime);
            }
            fadeImage.raycastTarget = false;
        }

        private IEnumerator FadeOutCoroutine(float duration)
        {
            fadeImage.raycastTarget = true; // 터치 차단
            float time = 0;
            while (time < duration)
            {
                time += fadeDeltaTime;
                fadeImage.color = new Color(0, 0, 0, time / duration);
                yield return new WaitForSeconds(fadeDeltaTime);
            }
            fadeImage.raycastTarget = false;
        }
    }
}
