using Codice.Client.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Runtime.InGameSystem
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDeltaTime = 0.01f;

        private float FadeDuration { get; set; } = 1f;
        private Coroutine _currentFadeCoroutine;
        private Tween _currentFadeTween;

        public void SetFadeImg()
        {
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        public void StartFadeIn()
        {
            StopCurrentFadeCoroutine();
            
            _currentFadeCoroutine = StartCoroutine(FadeInCoroutine(FadeDuration));
        }

        public void StartFadeOut()
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
