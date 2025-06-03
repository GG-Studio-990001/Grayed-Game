using System.Collections;
using UnityEngine;

namespace Runtime.CH3.Dancepace
{
    public class SpeakerAnimation : MonoBehaviour
    {
        [Header("=Animation Settings=")]
        [SerializeField] private float _maxScale = 1.2f;
        [SerializeField] private float _minScale = 0.8f;
        [SerializeField] private float _animationSpeed = 2f;
        [SerializeField] private float _autoBeatInterval = 1f;

        private Vector3 _originalScale;
        private bool _isAnimating = false;
        private Coroutine _animationCoroutine;
        private Coroutine _autoAnimationCoroutine;
        private float _animationTime = 0f;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        private void Start()
        {
            StartAutoAnimation();
        }

        public void StartBeatAnimation()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }
            _isAnimating = true;
            _animationCoroutine = StartCoroutine(BeatAnimation());
        }

        public void StopBeatAnimation()
        {
            _isAnimating = false;
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }
            transform.localScale = _originalScale;
        }

        private void StartAutoAnimation()
        {
            if (_autoAnimationCoroutine != null)
            {
                StopCoroutine(_autoAnimationCoroutine);
            }
            _autoAnimationCoroutine = StartCoroutine(AutoAnimation());
        }

        private void StopAutoAnimation()
        {
            if (_autoAnimationCoroutine != null)
            {
                StopCoroutine(_autoAnimationCoroutine);
                _autoAnimationCoroutine = null;
            }
        }

        private IEnumerator BeatAnimation()
        {
            float startTime = Time.time;
            float duration = 1f / _animationSpeed;

            while (_isAnimating)
            {
                float elapsedTime = Time.time - startTime;
                float normalizedTime = (elapsedTime % duration) / duration;
                
                // 사인 함수를 사용하여 부드러운 움직임 구현
                float scale = Mathf.Lerp(_minScale, _maxScale, 
                    (Mathf.Sin(normalizedTime * Mathf.PI * 2) + 1f) * 0.5f);
                
                transform.localScale = _originalScale * scale;
                yield return null;
            }

            transform.localScale = _originalScale;
        }

        private IEnumerator AutoAnimation()
        {
            while (true)
            {
                _animationTime += Time.deltaTime * _animationSpeed;
                
                // 사인 함수를 사용하여 부드러운 움직임 구현
                float scale = Mathf.Lerp(_minScale, _maxScale, 
                    (Mathf.Sin(_animationTime * Mathf.PI * 2) + 1f) * 0.5f);
                
                transform.localScale = _originalScale * scale;
                yield return null;
            }
        }

        private void OnDisable()
        {
            StopBeatAnimation();
            StopAutoAnimation();
        }
    }
}
