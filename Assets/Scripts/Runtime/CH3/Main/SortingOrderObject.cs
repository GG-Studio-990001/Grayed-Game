using System.Collections;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class SortingOrderObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private int baseOrder = 0;
        [SerializeField] private bool useYPosition = true;
        [SerializeField] private float heightOffset = 0.3f; // 높이 오프셋 추가
    
        protected virtual void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void LateUpdate()
        {
            UpdateSortingOrder();
        }

        protected virtual void UpdateSortingOrder()
        {
            if (spriteRenderer == null) return;
        
            // Y 위치에 높이 오프셋을 고려하여 계산
            float adjustedY = transform.position.y - heightOffset;
            int yOffset = useYPosition ? Mathf.RoundToInt(adjustedY * -100) : 0;
            int targetOrder = baseOrder + yOffset;
        
            //SmoothTransitionOrder(targetOrder);
            spriteRenderer.sortingOrder = targetOrder;

        }

        public void SetBaseOrder(int order)
        {
            baseOrder = order;
            UpdateSortingOrder();
        }
    
        // SortingOrderObject 클래스에 추가
        private void SmoothTransitionOrder(int targetOrder)
        {
            StartCoroutine(SmoothTransitionCoroutine(targetOrder));
        }

        private IEnumerator SmoothTransitionCoroutine(int targetOrder)
        {
            float duration = 0.2f; // 전환 시간
            float elapsed = 0;
            int startOrder = spriteRenderer.sortingOrder;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
        
                spriteRenderer.sortingOrder = Mathf.RoundToInt(
                    Mathf.Lerp(startOrder, targetOrder, t)
                );
        
                yield return null;
            }

            spriteRenderer.sortingOrder = targetOrder;
        }
    }
}