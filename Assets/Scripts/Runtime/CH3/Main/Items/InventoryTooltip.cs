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
        [SerializeField] private Vector2 tooltipOffset = new Vector2(16f, -16f);

        private Canvas canvas;
        private Camera canvasCamera;
        private RectTransform canvasRectTransform;
        private readonly Vector3[] worldCornersBuffer = new Vector3[4];

        private void Awake()
        {
            instance = this;
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (text == null) text = GetComponentInChildren<TextMeshProUGUI>(true);
            canvas = GetComponentInParent<Canvas>();
            canvasRectTransform = canvas != null ? canvas.transform as RectTransform : null;
            if (canvas != null)
            {
                canvasCamera = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            }
            else
            {
                Debug.LogWarning($"{nameof(InventoryTooltip)} requires a parent Canvas to position correctly.", this);
            }
            Hide();
        }

        public static void Show(string message, Vector2 screenPosition)
        {
            if (instance == null || instance.canvasGroup == null || instance.rectTransform == null || instance.canvasRectTransform == null)
            {
                return;
            }

            instance.text.text = message;
            var targetScreenPosition = screenPosition + instance.tooltipOffset;

            switch (instance.canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    instance.rectTransform.position = targetScreenPosition;
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            instance.canvasRectTransform,
                            targetScreenPosition,
                            instance.canvasCamera,
                            out var localPoint))
                    {
                        instance.rectTransform.anchoredPosition = localPoint;
                    }
                    else
                    {
                        instance.rectTransform.position = targetScreenPosition;
                    }
                    break;
            }

            instance.ClampToCanvas();
            instance.canvasGroup.alpha = 1f;
            instance.canvasGroup.blocksRaycasts = false;
        }

        public static void Hide()
        {
            if (instance == null || instance.canvasGroup == null) return;
            instance.canvasGroup.alpha = 0f;
        }

        private void ClampToCanvas()
        {
            if (canvasRectTransform == null || rectTransform == null) return;

            canvasRectTransform.GetWorldCorners(worldCornersBuffer);
            var canvasBottomLeft = worldCornersBuffer[0];
            var canvasTopRight = worldCornersBuffer[2];

            rectTransform.GetWorldCorners(worldCornersBuffer);
            var tooltipBottomLeft = worldCornersBuffer[0];
            var tooltipTopRight = worldCornersBuffer[2];

            var position = rectTransform.position;
            var offset = Vector3.zero;

            if (tooltipTopRight.x > canvasTopRight.x)
            {
                offset.x = canvasTopRight.x - tooltipTopRight.x;
            }
            else if (tooltipBottomLeft.x < canvasBottomLeft.x)
            {
                offset.x = canvasBottomLeft.x - tooltipBottomLeft.x;
            }

            if (tooltipTopRight.y > canvasTopRight.y)
            {
                offset.y = canvasTopRight.y - tooltipTopRight.y;
            }
            else if (tooltipBottomLeft.y < canvasBottomLeft.y)
            {
                offset.y = canvasBottomLeft.y - tooltipBottomLeft.y;
            }

            if (offset != Vector3.zero)
            {
                rectTransform.position = position + offset;
            }
        }
    }
}


