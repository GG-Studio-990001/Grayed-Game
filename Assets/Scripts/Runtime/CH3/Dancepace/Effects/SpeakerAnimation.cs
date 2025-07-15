using System.Collections;
using UnityEngine;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace
{
    public class SpeakerAnimation : MonoBehaviour
    {
        [Header("=Animation Settings=")]
        [SerializeField] private float _maxScale = 1.2f;
        [SerializeField] private float _minScale = 0.8f;
        [SerializeField] private float _animationSpeed = 2f;
        //[SerializeField] private float _autoBeatInterval = 1f;
        [SerializeField] private float _sensitivity = 10f; // 볼륨 반응 민감도

        private Vector3 _originalScale;
        private bool _isAnimating = false;
        private Coroutine _animationCoroutine;
        private Coroutine _autoAnimationCoroutine;
        private AudioSource _bgmSource;
        private float _currentScale = 1f;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        private void Start()
        {
            // SoundManager에서 BGM AudioSource 가져오기
            _bgmSource = Managers.Sound.BGM;
            StopBeatAnimation();
        }

        private void Update()
        {
            if (!_isAnimating) return;

            if (_bgmSource != null && _bgmSource.isPlaying)
            {
                float amplitude = GetBGMAmplitude();
                float targetScale = Mathf.Lerp(_minScale, _maxScale, amplitude * _sensitivity);
                // 이전 스케일과 부드럽게 섞기 (관성 효과)
                _currentScale = Mathf.Lerp(_currentScale, targetScale, Time.deltaTime * 8f);
                transform.localScale = _originalScale * _currentScale;
            }
            else
            {
                // 자동 애니메이션(음악 없을 때)
                float t = Mathf.Sin(Time.time * _animationSpeed) * 0.5f + 0.5f;
                float scale = Mathf.Lerp(_minScale, _maxScale, t);
                transform.localScale = _originalScale * scale;
            }
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

        private void OnDisable()
        {
            StopBeatAnimation();
            StopAutoAnimation();
        }

        // BGM의 실시간 볼륨(Amplitude) 측정
        private float GetBGMAmplitude()
        {
            if (_bgmSource == null) return 0f;

            float[] samples = new float[64];
            _bgmSource.GetOutputData(samples, 0);
            float sum = 0f;
            for (int i = 0; i < samples.Length; i++)
                sum += samples[i] * samples[i];
            return Mathf.Sqrt(sum / samples.Length); // RMS
        }
    }
}
