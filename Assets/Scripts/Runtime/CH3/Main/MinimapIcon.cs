using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH3.Main
{
    public class MinimapIcon : MonoBehaviour
    {
        private Image iconImage;
        private Transform targetTransform;
        private RectTransform iconRectTransform;
        private Camera minimapCamera;
        private RectTransform minimapRect;

        public void Initialize(Color color, Transform target, Camera miniMapCam, RectTransform mapRect)
        {
            iconImage = GetComponent<Image>();
            iconRectTransform = GetComponent<RectTransform>();
            targetTransform = target;
            minimapCamera = miniMapCam;
            minimapRect = mapRect;
            
            if (iconImage != null)
            {
                iconImage.color = color;
                iconImage.raycastTarget = false;
            }
        }

        private void LateUpdate()
        {
            if (targetTransform == null || minimapCamera == null || minimapRect == null) return;

            // 월드 좌표를 뷰포트 좌표로 변환
            Vector3 targetViewportPos = minimapCamera.WorldToViewportPoint(targetTransform.position);
            
            // 뷰포트 좌표를 UI 좌표로 변환
            float x = ((targetViewportPos.x - 0.5f) * minimapRect.rect.width);
            float y = ((targetViewportPos.y - 0.5f) * minimapRect.rect.height);

            // 클램핑 없이 직접 위치 설정
            iconRectTransform.anchoredPosition = new Vector2(x, y);
        }
    }
}