using UnityEngine;
using DG.Tweening;

namespace Runtime.CH2.Location
{
    public class LocationUIAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform uiElement; // UI의 RectTransform
        [SerializeField] private float animationDuration = 0.5f; // 이동 시간
        [SerializeField] private float stayDuration = 1f; // 멈춤 시간
        [SerializeField] private Vector3 offScreenPos;
        [SerializeField] private Vector3 onScreenPos;

        private void Start()
        {
            // 위치 초기화
            onScreenPos = uiElement.position; // 화면 안 위치
            offScreenPos = uiElement.position + Vector3.left * 2.5f; // 화면 밖 위치
            
            // 시작 위치 설정
            uiElement.position = offScreenPos;
        }

        public void AnimateUI()
        {
            uiElement.DOMove(onScreenPos, animationDuration).SetEase(Ease.InQuad) // 화면 안으로 이동
                .OnComplete(() =>
                {
                    // 1초 대기 후 다시 화면 밖으로 이동
                    uiElement.DOMove(offScreenPos, animationDuration).SetEase(Ease.InQuad).SetDelay(stayDuration);
                });
        }
    }
}