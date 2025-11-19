using UnityEngine;
using TMPro;

namespace Runtime.CH3.Main
{
    public class InventoryTooltip : MonoBehaviour
    {
        private static InventoryTooltip instance;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TextMeshProUGUI text;

        private void Awake()
        {
            instance = this;
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (text == null) text = GetComponentInChildren<TextMeshProUGUI>(true);
            Hide();
        }

        public static void Show(string message, Vector2 screenPos)
        {
            if (instance == null) return;
            instance.text.text = message;
            instance.rectTransform.position = screenPos + new Vector2(10, -10);
            instance.canvasGroup.alpha = 1f;
            instance.canvasGroup.blocksRaycasts = false;
        }

        public static void Hide()
        {
            if (instance == null) return;
            instance.canvasGroup.alpha = 0f;
        }
    }
}


