using UnityEngine;
using DG.Tweening;

namespace Runtime.CH2.Location
{
    public class LocationUIAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform _locationUI;
        [SerializeField] private float _animationDuration = 0.5f; // 이동 시간
        [SerializeField] private float _stayDuration = 1f; // 멈춤 시간
        [SerializeField] private Vector3 _offScreenPos;
        [SerializeField] private Vector3 _onScreenPos;

        private void Start()
        {
            // 위치 초기화
            _onScreenPos = _locationUI.position; // 화면 안 위치
            _offScreenPos = _locationUI.position + Vector3.left * 2.5f; // 화면 밖 위치

            // 시작 위치 설정
            _locationUI.position = _offScreenPos;
        }

        public void AnimateUI()
        {
            _locationUI.DOMove(_onScreenPos, _animationDuration).SetEase(Ease.InQuad) // 화면 안으로 이동
                .OnComplete(() =>
                {
                    // 1초 대기 후 다시 화면 밖으로 이동
                    _locationUI.DOMove(_offScreenPos, _animationDuration).SetEase(Ease.InQuad).SetDelay(_stayDuration);
                });
        }
    }
}