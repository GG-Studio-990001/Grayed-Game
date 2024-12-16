using UnityEngine;
using DG.Tweening;

namespace Runtime.CH2.Location
{
    public class LocationUIAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform _locationUI;
        [SerializeField] private Vector2 _offScreenPos;
        [SerializeField] private Vector2 _onScreenPos;
        private readonly float _animationDuration = 0.5f; // 이동 시간
        private readonly float _stayDuration = 2f; // 멈춤 시간

        private void Start()
        {
            // RectTransform의 로컬 좌표를 사용하여 위치 초기화
            _onScreenPos = _locationUI.anchoredPosition; // 화면 안 위치
            _offScreenPos = _onScreenPos + Vector2.left * 250f; // 화면 밖 위치 (단위: 로컬 좌표)

            // 시작 위치 설정
            _locationUI.anchoredPosition = _offScreenPos;
        }

        public void AnimateUI()
        {
            // 화면 안으로 이동
            _locationUI.DOAnchorPos(_onScreenPos, _animationDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    // 1초 대기 후 다시 화면 밖으로 이동
                    _locationUI.DOAnchorPos(_offScreenPos, _animationDuration).SetEase(Ease.InQuad).SetDelay(_stayDuration);
                });
        }
    }
}
