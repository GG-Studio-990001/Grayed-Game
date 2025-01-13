using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Runtime.CH2.Tcg
{
    public class CardHoverEffectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private TcgController _tcgController;
        [SerializeField] private float _scaleMultiplier = 1.2f; // 확대 배율
        [SerializeField] private float _duration = 0.2f;       // 애니메이션 시간

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_tcgController.IsCardSelected)
                return;

            // 마우스가 올라오면 확대
            transform.DOScale(_scaleMultiplier, _duration).SetEase(Ease.OutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 마우스가 벗어나면 축소
            transform.DOScale(1f, _duration).SetEase(Ease.OutQuad);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 마우스 클릭 시 축소
            transform.DOScale(1f, _duration).SetEase(Ease.OutQuad);
        }
    }
}