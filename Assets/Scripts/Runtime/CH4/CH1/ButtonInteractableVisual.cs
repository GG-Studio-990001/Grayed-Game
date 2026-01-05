using UnityEngine;
using UnityEngine.UI;

namespace CH4.CH1
{
    // ButtonInteractableController로 관리
    [RequireComponent(typeof(Button), typeof(CanvasGroup))]
    public class ButtonInteractableVisual : MonoBehaviour
    {
        private Button _button;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetBtnAlpha(bool isFullfill, float disabledAlpha = 0.7f)
        {
            if (_button == null)
                _button = GetComponent<Button>();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            _button.interactable = isFullfill;
            _canvasGroup.alpha = isFullfill ? 1f : disabledAlpha;
        }
    }
}