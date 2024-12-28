using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Runtime.CH2.Main;

namespace Runtime.CH2
{
    public class CardHoverEffectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TcgController _tcgController;
        [SerializeField] private float _scaleMultiplier = 1.2f; // 확대 배율
        [SerializeField] private float _duration = 0.2f;       // 애니메이션 시간
        private Vector3 _originalScale; // 원래 크기 저장

        private void Start()
        {
            // 초기 크기 저장
            _originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_tcgController.IsCardSelected)
                return;

            // 마우스가 올라오면 확대
            transform.DOScale(_originalScale * _scaleMultiplier, _duration).SetEase(Ease.OutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 마우스가 벗어나면 축소
            transform.DOScale(_originalScale, _duration).SetEase(Ease.OutQuad);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 마우스 클릭 시 축소
            transform.DOScale(_originalScale, _duration).SetEase(Ease.OutQuad);
        }
    }
}